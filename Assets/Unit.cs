using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private int health = 0;
    private bool healingProcess = false;

    private async void Start()
    {
        using (var cancelationSource = new CancellationTokenSource())
        {
            using (var cancellationTokenSource1 = new CancellationTokenSource())
            {
                var task1 = Task1(cancellationTokenSource1.Token);
                var task2 = Task2(cancellationTokenSource1.Token);

                Debug.Log(await WhatTaskFasterAsync(cancelationSource.Token, task1, task2, cancellationTokenSource1));
            }
        }
        ReceiveHealing();
    }

    private void Update()
    {
        //ReceiveHealing();
    }

    // 1.������: ����������� ��������, ������� ����� ���������� �� ������ RecieveHealing,
    // ����� ���� ������� ��������� 5 ������ ������ ���������� � ������� 3 ������ ��� �� ��� ���,
    // ���� ���������� ������ �� ������ ������ 100. �� ���� �� ����� ����������� ����� ������ ������� ��������� ������������.
    public void ReceiveHealing()
    {
        StartCoroutine(ReceiveHealingCoroutine());
    }

    private IEnumerator ReceiveHealingCoroutine()
    {
        if (healingProcess || health >= 100)
            yield break;

        healingProcess = true;
        for (int i = 0; i < 6; i++)
        {
            if (health >= 100)
                break;
            health += Mathf.Min(5, 100 - health);
            Debug.Log(health);
            yield return new WaitForSeconds(0.5f);
        }
        healingProcess = false;
    }

    //    ������� 2. ��������� async/await.
    // ����������� ��� ������: Task1 � Task2.� �������� ���������� ������ ������ ��������� CancellationToken.
    // ������ ������ ������ ������� ���� �������, � ����� �������� � ������� ��������� � ���� ����������.
    // ������ ������ ������ ������� 60 ������, � ����� � �������� ��������� � �������.
    private async Task Task1(CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken);
        if (cancellationToken.IsCancellationRequested)
            return;
        Debug.Log("Task1 was completed.");
    }

    private async Task Task2(CancellationToken cancellationToken)
    {
        var frame = 60;
        while (frame > 0)
        {
            if (cancellationToken.IsCancellationRequested)
                return;
            frame--;
            await Task.Yield();
        }
        Debug.Log("Task2 was completed.");
    }

    //    ������� 3 (��������������).
    // ����������� ������ WhatTaskFasterAsync, ������� ����� ��������� � �������� ���������� CancellationToken,
    // � ����� ��� ������ � ���� ���������� ���� Task.
    // ������ ������ ������� ���������� ���� �� ����� �� �����, ������������� ������ � ���������� ���������.
    // ���� ������ ������ ��������� ������, ������� true, ���� ������ � false. ���� �������� CancellationToken, ����� ������� false.
    // ��������� ����������������� � ������� ����� �� ������� 2.

    public static async Task<bool> WhatTaskFasterAsync(CancellationToken ct, Task task1, Task task2, CancellationTokenSource tokenSource)
    {
        var finishedTask = await Task.WhenAny(task1, task2);
        tokenSource.Cancel();
        var result = finishedTask == task1 && !ct.IsCancellationRequested;
        return result;
    }
}
