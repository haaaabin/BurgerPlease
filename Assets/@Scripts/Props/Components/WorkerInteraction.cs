using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class WorkerInteraction : MonoBehaviour
{
    public Action<WorkerController> OnTriggerStart;
    public Action<WorkerController> OnTriggerEnd;
    public Action<WorkerController> OnInteraction;  //Worker가 범위에 머무는 동안 주기적으로 호출되는 이벤트

    public float InteractInterval = 0.5f;
    public WorkerController CurrentWorker;  // 현재 범위 내에 있는 Worker
    private Coroutine _coWorkerInteraction;

    private void OnEnable()
    {
        if (_coWorkerInteraction != null)
            StopCoroutine(_coWorkerInteraction);

        _coWorkerInteraction = StartCoroutine(CoPlayerInteraction());
    }

    private void OnDisable()
    {
        if (_coWorkerInteraction != null)
            StopCoroutine(_coWorkerInteraction);

        _coWorkerInteraction = null;
    }

    IEnumerator CoPlayerInteraction()
    {
        while (true)
        {
            yield return new WaitForSeconds(InteractInterval);

            if (CurrentWorker != null)
                OnInteraction?.Invoke(CurrentWorker);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        WorkerController wc = other.GetComponent<WorkerController>();
        if (wc == null)
            return;

        CurrentWorker = wc;
        OnTriggerStart?.Invoke(wc);
    }

    private void OnTriggerExit(Collider other)
    {
        WorkerController wc = other.GetComponent<WorkerController>();
        if (wc == null)
            return;

        CurrentWorker = null;
        OnTriggerEnd?.Invoke(wc);
    }

}