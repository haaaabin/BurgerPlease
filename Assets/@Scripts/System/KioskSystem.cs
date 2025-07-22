using System.Collections;
using UnityEngine;
using static Define;

public class KioskSystem : WorkerSystemBase<EKioskJob>
{
    public KioskCounter Kiosk;
    public MainCounterSystem MainCounter;

    public override bool HasJob
    {
        get
        {
            for (int i = 0; i < (int)EKioskJob.MaxCount; i++)
            {
                EKioskJob type = (EKioskJob)i;
                if (ShouldDoJob(type))
                    return true;
            }

            return false;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        Kiosk.Owner = this;
    }

    protected override IEnumerator DoWorkerJob(WorkerController wc)
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            bool foundJob = false;

            if (ShouldDoJob(EKioskJob.MoveBurger))
            {
                foundJob = true;
                Jobs[(int)EKioskJob.MoveBurger] = wc;

                wc.SetDestination(MainCounter.Grill.WorkerPos.position, () =>
                {
                    wc.transform.rotation = MainCounter.Grill.WorkerPos.rotation;
                });
                yield return new WaitUntil(() => wc.HasArrivedAtDestination);

                wc.transform.rotation = MainCounter.Grill.WorkerPos.rotation;
                yield return new WaitForSeconds(3);

                wc.SetDestination(Kiosk.BurgerWorkerPos.position, () =>
                {
                    wc.transform.rotation = Kiosk.BurgerWorkerPos.rotation;
                });
                yield return new WaitUntil(() => wc.HasArrivedAtDestination);

                wc.transform.rotation = Kiosk.BurgerWorkerPos.rotation;
                yield return new WaitForSeconds(2);

                Jobs[(int)EKioskJob.MoveBurger] = null;
            }

            // 카운터 계산대 작업
            if (ShouldDoJob(EKioskJob.CounterCashier))
            {
                foundJob = true;
                Jobs[(int)EKioskJob.CounterCashier] = wc;

                wc.SetDestination(Kiosk.CashierWorkerPos.position, () =>
                {
                    wc.transform.rotation = Kiosk.CashierWorkerPos.rotation;
                });
                yield return new WaitUntil(() => wc.HasArrivedAtDestination);

                wc.transform.rotation = Kiosk.CashierWorkerPos.rotation;
                yield return new WaitForSeconds(2);

                Jobs[(int)EKioskJob.CounterCashier] = null;
            }

            if (foundJob == false)
            {
                yield return new WaitForSeconds(1f);
                RemoveWorker(wc);
                yield break;
            }
        }
    }

    protected override bool ShouldDoJob(EKioskJob jobType)
    {
        int idx = (int)jobType;
        if (idx < 0 || idx >= Jobs.Length)
            return false;

        WorkerController wc = Jobs[idx];
        if (wc != null)
            return false;

        switch (jobType)
        {
            case EKioskJob.MoveBurger:
                if (MainCounter.Grill.BurgerCount == 0)
                    return false;
                if (Kiosk == null || !Kiosk.gameObject.activeInHierarchy)
                    return false;
                if (Kiosk.NeedMoreBurgers == false)
                    return false;
                if (Kiosk.CurrentBurgerWorker != null)
                    return false;
                return true;

            case EKioskJob.CounterCashier:
                if (Kiosk == null || !Kiosk.gameObject.activeInHierarchy)
                    return false;
                if (Kiosk.BurgerCount == 0)
                    return false;
                if (Kiosk.NeedCashier == false)
                    return false;
                if (Kiosk.IsEnoughSellBurger == false)
                    return false;
                return true;
        }
        return false;
    }
}