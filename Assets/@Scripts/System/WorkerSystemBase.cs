using System;
using System.Collections;

public abstract class WorkerSystemBase<TJobType> : SystemBase where TJobType : Enum
{
    public WorkerController[] Jobs;

    protected virtual void Awake()
    {
        // 직원들이 담당하는 일들
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

    protected abstract IEnumerator DoWorkerJob(WorkerController wc); // 직원이 해당 일을 하는 동작
    protected abstract bool ShouldDoJob(TJobType jobType); // 직원이 해당 일을 해야하는지 여부를 반환
}

