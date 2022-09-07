using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

public class Unit : MonoBehaviour
{
    private void Start()
    {
        Task1();
        Task2();
    }

    private void Update()
    {
        Task3();
    }

    private void OnDestroy()
    {
        if (array.IsCreated)
            array.Dispose();

        if (positions.IsCreated)
            positions.Dispose();
        if (velocities.IsCreated)
            velocities.Dispose();
        if (finalPositions.IsCreated)
            finalPositions.Dispose();

        if (transformAccessArray.isCreated)
            transformAccessArray.Dispose();
    }

    // ����� 1: �������� ������ ���� IJob, ������� ��������� ������ � ������� NativeArray<int>
    // � � ���������� ���������� ��� �������� ����� ������ ������ ������� ����.
    // �������� ���������� ���� ������ �� �������� ������ � �������� � ������� ���������.
    private NativeArray<int> array;
    private JobHandle handle;
    void Task1()
    {
        array = new NativeArray<int>(new int[] { 6, 89, 9, -6, 0, 34, 1, 56, 8, 99 }, Allocator.Persistent);
        ResetOverTenJob job = new ResetOverTenJob();
        job.array = array;
        this.handle = job.Schedule();
        this.handle.Complete();
        foreach (var item in array)
        {
            Debug.Log(item);
        }
    }


    // ����� 2. C������� ������ ���� IJobParallelFor, ������� ����� ��������� ������ � ���� ���� �����������:
    // Positions � Velocities � ���� NativeArray<Vector3>.����� �������� ������ FinalPositions ���� NativeArray<Vector3>.
    // �������� ���, ����� � ���������� ���������� ������ � �������� ������� FinalPositions ���� �������� ����� ���������������
    // ��������� �������� Positions � Velocities.
    // �������� ���������� ��������� ������ �� �������� ������ � �������� � ������� ���������.
    private NativeArray<Vector3> positions;
    private NativeArray<Vector3> velocities;
    private NativeArray<Vector3> finalPositions;
    private JobHandle parallelHandle;
    void Task2()
    {
        positions = new NativeArray<Vector3>(
            new Vector3[] { new Vector3(1, 1, 1), new Vector3(0, 5, -4), new Vector3(6, 7, 1) }, Allocator.Persistent);
        velocities = new NativeArray<Vector3>(
            new Vector3[] { Vector3.up, Vector3.one, Vector3.back }, Allocator.Persistent);
        finalPositions = new NativeArray<Vector3>(new Vector3[3], Allocator.Persistent);

        CalcFinalPositionJob job = new CalcFinalPositionJob();
        job.Positions = positions;
        job.Velocities = velocities;
        job.FinalPositions = finalPositions;
        this.parallelHandle = job.Schedule(finalPositions.Length, -1);
        this.parallelHandle.Complete();
        foreach (var item in finalPositions)
        {
            Debug.Log(item);
        }
    }


    // ����� 3*: �������� ������ ���� IJobForTransform, ������� ����� ������� ��������� Transform ������ ����� ��� � �������� ���������.
    private TransformAccessArray transformAccessArray;
    private JobHandle rotateObjectHandele;
    void Task3()
    {
        RotateObjectJob job = new RotateObjectJob()
        {
            DeltaTime = Time.deltaTime,
            Speed = 10,
            Directive = new Vector3(0, 1, 1)
        };
        transformAccessArray = new TransformAccessArray(new Transform[] { transform });

        this.rotateObjectHandele = job.Schedule(transformAccessArray);
        this.rotateObjectHandele.Complete();
        transformAccessArray.Dispose();
    }
}

