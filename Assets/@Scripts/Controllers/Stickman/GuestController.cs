using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;
using static Define;

public class GuestController : StickmanController
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
	public GameObject bubbleGameObject;
	public bool IsOrderingNow = false;
	public bool IsBubbleShown = false;
	public bool IsWaitingForBurger = false;

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void Update()
	{
		base.Update();

		if (GuestState == EGuestState.Eating)
		{
			return;
		}

		if (HasArrivedAtDestination)
		{
			switch (GuestState)
			{
				case EGuestState.Kiosk:
					State = EAnimState.Kiosk;
					break;

				case EGuestState.Queuing:
					State = EAnimState.Idle; 
					break;

				default:
					State = EAnimState.Idle;
					break;
			}
		}
		else
		{
			State = EAnimState.Move;
			LookAtDestination();
		}

	}
}
