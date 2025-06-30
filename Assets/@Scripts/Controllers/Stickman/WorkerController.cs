using System.Collections;
using UnityEngine;
using static Define;

// A) MainCounter
// - BurgerGrill
// - Counter : Burger
// - Counter : Cash
// - Table : Cleanup
// - TrashBin 

[RequireComponent(typeof(CharacterController))]
public class WorkerController : StickmanController
{
	protected CharacterController _controller;
	public SystemBase CurrentSystem;

	public Coroutine WorkerJob;
	public void DoJob(IEnumerator job)
	{
		if (WorkerJob != null)
			StopCoroutine(WorkerJob);

		WorkerJob = StartCoroutine(job);
	}

	protected override void Awake()
	{
		base.Awake();

		_controller = GetComponent<CharacterController>();
	}

	private void Start()
	{
		State = Define.EAnimState.Move;
	}

	protected override void Update()
	{
		base.Update();

		if (HasArrivedAtDestination)
		{
			_navMeshAgent.isStopped = true;
			State = EAnimState.Idle;
			if (_particle != null && _particle.isPlaying)
			{
				_particle.Stop();
			}
		}
		else
		{
			State = EAnimState.Move;
			if (_particle != null && !_particle.isPlaying)
			{
				_particle.Play();
			}
			LookAtDestination();
		}
	}

	public void IncreaseSpeed()
	{
		_navMeshAgent.speed *= 1.2f;
	}

}
