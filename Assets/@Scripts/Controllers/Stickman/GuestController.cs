using TMPro;
using UnityEngine;
using UnityEngine.AI;
using static Define;

public class GuestController : StickManController
{
    private EGuestState _guestState = EGuestState.None;
    public EGuestState GuestState
    {
        get { return _guestState; }
        set
        {
            _guestState = value;

            if (value == EGuestState.Eating)
                State = EAnimState.Eating;

            UpdateAnimation();
        }
    }

    public int CurrentDestQueueIndex;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();

        if (HasArrivedAtDestination)
        {
            _navMeshAgent.isStopped = true;
            State = EAnimState.Idle;
        }
        else
        {
            State = EAnimState.Move;
            LookAtDestination();
        }

        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

}
