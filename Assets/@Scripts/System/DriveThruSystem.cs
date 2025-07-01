using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class DriveThruSystem : SystemBase
{
    public PakingDesk PakingDesk;
    public DriveThruCounter DriveThruCounter;

    // 직원들이 담당하는 일들.
    public WorkerController[] Jobs = new WorkerController[(int)EDriveThruJob.MaxCount];

    // public override bool HasJob
    // {
    //     get
    //     {
    //         for (int i = 0; i < (int)EDriveThruJob.MaxCount; i++)
    //         {
    //             EDriveThruJob type = (EDriveThruJob)i;
    //             if (ShouldDoJob(type))
    //                 return true;
    //         }

    //         return false;
    //     }
    // }

    // bool ShouldDoJob(EDriveThruJob jobType)
    // {
    //     // 이미 다른 직원이 점유중이라면 스킵.
    //     WorkerController wc = Jobs[(int)jobType];
    //     if (wc != null)
    //         return false;

    //     // 일감이 있는지 확인.
    //     switch (jobType)
    //     {
    //         case EDriveThruJob.MoveBurger:
    //             {
    //                 if (PakingDesk == null)
    //                     return false;
    //                 if (PakingDesk.CurrentBurgerWorker != null)
    //                     return false;
    //                 if (PakingDesk.BurgerCount == 0)
    //                     return false;
    //                 if (PakingDesk.NeedMoreBurgers == false)
    //                     return false;
    //                 return true;
    //             }
    //         case EDriveThruJob.PakingBurger:
    //             {
    //                 if (PakingDesk == null)
    //                     return false;
    //                 if (PakingDesk.CurrentPakingBoxWorker != null)
    //                     return false;
    //                 if (PakingDesk.BurgerCount == 0)
    //                     return false;
    //                 if (PakingDesk.NeedMoreBurgers == false)
    //                     return false;
    //                 return true;
    //             }
    //         case EDriveThruJob.MovePakingBox:
    //             {
    //                 if (DriveThruCounter == null)
    //                     return false;
    //                 if (PakingDesk.CurrentTakingBoxWorker != null)
    //                     return false;
    //                 if (DriveThruCounter.CurrentTakingWorker != null)
    //                     return false;
    //                 if (PakingDesk.BurgerCount == 0)
    //                     return false;
    //                 return false;
    //             }
    //         case EDriveThruJob.CounterCashier:
    //             {
    //                 if (DriveThruCounter == null)
    //                     return false;
    //                 if (DriveThruCounter.CurrentCashierWorker != null)
    //                     return false;
    //                 if (DriveThruCounter.NeedCashier == false)
    //                     return false;
    //                 // if (DriveThruCounter.FindTableToServeGuests() == null)
    //                 //     return false;

    //                 return true;
    //             }
    //     }

    //     return false;
    // }

}