using System.Collections;
using UnityEngine;
using static Define;

public class DriveThruSystem : SystemBase
{
    public PackingDesk PackingDesk;
    public DriveThruCounter DriveThruCounter;

    public MainCounterSystem MainCounter;

    // 직원들이 담당하는 일들.
    public WorkerController[] Jobs = new WorkerController[(int)EDriveThruJob.MaxCount];

    public override bool HasJob
    {
        get
        {
            for (int i = 0; i < (int)EDriveThruJob.MaxCount; i++)
            {
                EDriveThruJob type = (EDriveThruJob)i;
                if (ShouldDoJob(type))
                    return true;
            }

            return false;
        }
    }

    private void Awake()
    {
        DriveThruCounter.Owner = this;
    }

    private void Update()
    {
        foreach (WorkerController worker in Workers)
        {
            if (worker.WorkerJob != null)
                continue;

            IEnumerator job = DoDriveThruWorkerJob(worker);
            worker.DoJob(job);
        }
    }

    #region Worker
    public override void AddWorker(WorkerController worker)
    {
        base.AddWorker(worker);
    }

    bool ShouldDoJob(EDriveThruJob jobType)
    {
        int idx = (int)jobType;
        if (idx < 0 || idx >= Jobs.Length)
            return false; // 잘못된 인덱스면 false 반환

        // 이미 다른 직원이 점유중이라면 스킵.
        WorkerController wc = Jobs[idx];
        if (wc != null)
            return false;

        // 일감이 있는지 확인.
        switch (jobType)
        {
            case EDriveThruJob.MoveBurger:
                {
                    if (MainCounter.Grill.BurgerCount == 0)
                        return false;
                    if (PackingDesk == null)
                        return false;
                    if (PackingDesk.NeedMoreBurgers == false)
                        return false;
                    if (PackingDesk.CurrentBurgerWorker != null)
                        return false;

                    return true;
                }
            case EDriveThruJob.PackingBurger:
                {
                    if (PackingDesk == null)
                        return false;
                    if (PackingDesk.BurgerCount < Define.PACKING_BOX_MAX_BURGER_COUNT)
                        return false;
                    if (DriveThruCounter.NeedMorePacking == false)
                        return false;
                    if (PackingDesk.CurrentPackingBoxWorker != null)
                        return false;

                    return true;
                }
            case EDriveThruJob.MovePackingBox:
                {
                    if (DriveThruCounter == null)
                        return false;
                    if (DriveThruCounter.NeedMorePacking == false)
                        return false;
                    if (PackingDesk.PackingCount == 0)
                        return false;
                    if (PackingDesk.CurrentTakingBoxWorker != null)
                        return false;

                    return true;
                }

            case EDriveThruJob.CounterCashier:
                {
                    if (DriveThruCounter == null)
                        return false;
                    if (DriveThruCounter.PackingCount == 0)
                        return false;
                    if (DriveThruCounter.NeedCashier == false)
                        return false;
                    if (DriveThruCounter.IsEnoughSellBurger == false)
                        return false;

                    return true;
                }

        }
        return false;
    }

    IEnumerator DoDriveThruWorkerJob(WorkerController wc)
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            bool foundJob = false;

            if (ShouldDoJob(EDriveThruJob.MoveBurger))
            {
                foundJob = true;
                Jobs[(int)EDriveThruJob.MoveBurger] = wc;

                wc.SetDestination(MainCounter.Grill.WorkerPos.position, () =>
                {
                    wc.transform.rotation = MainCounter.Grill.WorkerPos.rotation;
                });

                yield return new WaitUntil(() => wc.HasArrivedAtDestination);

                wc.transform.rotation = MainCounter.Grill.WorkerPos.rotation;
                yield return new WaitForSeconds(3);

                wc.SetDestination(PackingDesk.BurgerWorkerPos.position, () =>
                {
                    wc.transform.rotation = PackingDesk.BurgerWorkerPos.rotation;
                });

                yield return new WaitUntil(() => wc.HasArrivedAtDestination);

                wc.transform.rotation = PackingDesk.BurgerWorkerPos.rotation;
                yield return new WaitForSeconds(2);

                Jobs[(int)EDriveThruJob.MoveBurger] = null;
            }

            if (ShouldDoJob(EDriveThruJob.PackingBurger))
            {
                foundJob = true;
                Jobs[(int)EDriveThruJob.PackingBurger] = wc;

                wc.SetDestination(PackingDesk.PackingWorkerPos.position, () =>
                {
                    wc.transform.rotation = PackingDesk.PackingWorkerPos.rotation;
                });

                yield return new WaitUntil(() => wc.HasArrivedAtDestination);

                wc.transform.rotation = PackingDesk.PackingWorkerPos.rotation;
                yield return new WaitForSeconds(2);

                Jobs[(int)EDriveThruJob.PackingBurger] = null;
            }

            if (ShouldDoJob(EDriveThruJob.MovePackingBox))
            {
                foundJob = true;
                Jobs[(int)EDriveThruJob.MovePackingBox] = wc;

                wc.SetDestination(PackingDesk.MovePackingBoxWorkerPos.position, () =>
                {
                    wc.transform.rotation = PackingDesk.MovePackingBoxWorkerPos.rotation;
                });

                yield return new WaitUntil(() => wc.HasArrivedAtDestination);

                wc.transform.rotation = PackingDesk.MovePackingBoxWorkerPos.rotation;
                yield return new WaitForSeconds(2);

                wc.SetDestination(DriveThruCounter.PackingWorkerPos.position, () =>
                {
                    wc.transform.rotation = DriveThruCounter.PackingWorkerPos.rotation;
                });

                yield return new WaitUntil(() => wc.HasArrivedAtDestination);

                wc.transform.rotation = DriveThruCounter.PackingWorkerPos.rotation;
                yield return new WaitForSeconds(2);

                Jobs[(int)EDriveThruJob.MovePackingBox] = null;
            }

            if (ShouldDoJob(EDriveThruJob.CounterCashier))
            {
                foundJob = true;
                Jobs[(int)EDriveThruJob.CounterCashier] = wc;

                wc.SetDestination(DriveThruCounter.CashierWorkerPos.position, () =>
                {
                    wc.transform.rotation = DriveThruCounter.CashierWorkerPos.rotation;
                });

                yield return new WaitUntil(() => wc.HasArrivedAtDestination);

                wc.transform.rotation = DriveThruCounter.CashierWorkerPos.rotation;
                yield return new WaitForSeconds(2);

                Jobs[(int)EDriveThruJob.CounterCashier] = null;
            }

            if (foundJob == false)
            {
                yield return new WaitForSeconds(1f);
                RemoveWorker(wc);
                yield break;
            }
        }
    }

    #endregion
}