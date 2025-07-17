using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorkerSystemBase<TJobType> : SystemBase where TJobType : Enum
{
    public WorkerController[] Jobs;

    protected virtual void Awake()
    {
        Jobs = new WorkerController[Enum.GetValues(typeof(TJobType)).Length];
    }

    protected virtual void Update()
    {
        foreach (var worker in Workers)
        {
            if (worker.WorkerJob != null) continue;
            var job = DoWorkerJob(worker);
            worker.DoJob(job);
        }
    }

    public override void AddWorker(WorkerController worker)
    {
        base.AddWorker(worker);
    }

    protected abstract IEnumerator DoWorkerJob(WorkerController wc);
    protected abstract bool ShouldDoJob(TJobType jobType);
}