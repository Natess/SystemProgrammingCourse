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

    // 1.Задача: реализовать корутину, которая будет вызываться из метода RecieveHealing,
    // чтобы юнит получал исцеление 5 жизней каждые полсекунды в течение 3 секунд или до тех пор,
    // пока количество жизней не станет равным 100. На юнит не может действовать более одного эффекта исцеления одновременно.
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

    //    Задание 2. Применить async/await.
    // Реализовать две задачи: Task1 и Task2.В качестве параметров задачи должны принимать CancellationToken.
    // Первая задача должна ожидать одну секунду, а после выводить в консоль сообщение о своём завершении.
    // Вторая задача должна ожидать 60 кадров, а после — выводить сообщение в консоль.
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

    //    Задание 3 (дополнительное).
    // Реализовать задачу WhatTaskFasterAsync, которая будет принимать в качестве параметров CancellationToken,
    // а также две задачи в виде переменных типа Task.
    // Задача должна ожидать выполнения хотя бы одной из задач, останавливать другую и возвращать результат.
    // Если первая задача выполнена первой, вернуть true, если вторая — false. Если сработал CancellationToken, также вернуть false.
    // Проверить работоспособность с помощью задач из Задания 2.

    public static async Task<bool> WhatTaskFasterAsync(CancellationToken ct, Task task1, Task task2, CancellationTokenSource tokenSource)
    {
        var finishedTask = await Task.WhenAny(task1, task2);
        tokenSource.Cancel();
        var result = finishedTask == task1 && !ct.IsCancellationRequested;
        return result;
    }
}
