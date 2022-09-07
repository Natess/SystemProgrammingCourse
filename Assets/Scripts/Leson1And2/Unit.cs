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

    // Часть 1: Создайте задачу типа IJob, которая принимает данные в формате NativeArray<int>
    // и в результате выполнения все значения более десяти делает равными нулю.
    // Вызовите выполнение этой задачи из внешнего метода и выведите в консоль результат.
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


    // Часть 2. Cоздайте задачу типа IJobParallelFor, которая будет принимать данные в виде двух контейнеров:
    // Positions и Velocities — типа NativeArray<Vector3>.Также создайте массив FinalPositions типа NativeArray<Vector3>.
    // Сделайте так, чтобы в результате выполнения задачи в элементы массива FinalPositions были записаны суммы соответствующих
    // элементов массивов Positions и Velocities.
    // Вызовите выполнение созданной задачи из внешнего метода и выведите в консоль результат.
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


    // Часть 3*: создайте задачу типа IJobForTransform, которая будет вращать указанные Transform вокруг своей оси с заданной скоростью.
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

