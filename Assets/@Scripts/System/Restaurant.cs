using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class Restaurant : MonoBehaviour
{
	public List<SystemBase> RestaurantSystems = new List<SystemBase>();

	public int StageNum = 0;
	public List<UnlockableBase> Props = new List<UnlockableBase>();
	public List<WorkerController> Workers = new List<WorkerController>();

	private RestaurantData _data;

	private void OnEnable()
	{
		GameManager.Instance.AddEventListener(EEventType.HireWorker, OnHireWorker);
		GameManager.Instance.AddEventListener(EEventType.UpgradeEmployeeSpeed, OnUpgradeEmployeeSpeed);
		GameManager.Instance.AddEventListener(EEventType.UpgradeEmployeeCapacity, OnUpgradeEmployeeCapacity);
		GameManager.Instance.AddEventListener(EEventType.UpgradePlayerSpeed, OnUpgradePlayerSpeed);
		GameManager.Instance.AddEventListener(EEventType.UpgradePlayerCapacity, OnUpgradePlayerCapacity);
		GameManager.Instance.AddEventListener(EEventType.UpgradePlayerProfit, OnUpgradePlayerProfit);

		StartCoroutine(CoDistributeWorkerAI());
	}

	private void OnDisable()
	{
		GameManager.Instance.RemoveEventListener(EEventType.HireWorker, OnHireWorker);
		GameManager.Instance.RemoveEventListener(EEventType.UpgradeEmployeeSpeed, OnUpgradeEmployeeSpeed);
		GameManager.Instance.RemoveEventListener(EEventType.UpgradeEmployeeCapacity, OnUpgradeEmployeeCapacity);
		GameManager.Instance.RemoveEventListener(EEventType.UpgradePlayerSpeed, OnUpgradePlayerSpeed);
		GameManager.Instance.RemoveEventListener(EEventType.UpgradePlayerCapacity, OnUpgradePlayerCapacity);
		GameManager.Instance.RemoveEventListener(EEventType.UpgradePlayerProfit, OnUpgradePlayerProfit);

	}

	public void SetInfo(RestaurantData data)
	{
		_data = data;

		RestaurantSystems = GetComponentsInChildren<SystemBase>().ToList();
		Props = GetComponentsInChildren<UnlockableBase>().ToList();

		for (int i = 0; i < Props.Count; i++)
		{
			UnlockableStateData stateData = data.UnlockableStates[i];
			Props[i].SetInfo(stateData);
		}

		Tutorial tutorial = GetComponent<Tutorial>();
		if (tutorial != null)
			tutorial.SetInfo(data);

		for (int i = 0; i < data.WorkerCount; i++)
			OnHireWorker();
	}

	void OnHireWorker()
	{
		GameObject go = GameManager.Instance.SpawnWorker();
		WorkerController wc = go.GetComponent<WorkerController>();
		go.transform.position = Define.WORKER_SPAWN_POS;

		Workers.Add(wc);

		// 필요하면 세이브 파일 갱신.
		_data.WorkerCount = Mathf.Max(_data.WorkerCount, Workers.Count);
	}

	void OnUpgradeEmployeeSpeed()
	{
		foreach (WorkerController worker in Workers)
		{
			worker.IncreaseSpeed();
		}
	}

	void OnUpgradeEmployeeCapacity()
	{
		foreach (WorkerController worker in Workers)
		{
			worker.Tray.IncreaseCapacity();
		}
	}

	void OnUpgradePlayerSpeed()
	{
		GameManager.Instance.Player.IncreaseSpeed();
	}

	void OnUpgradePlayerCapacity()
	{
		GameManager.Instance.Player.Tray.IncreaseCapacity();
	}

	void OnUpgradePlayerProfit()
	{
		// GameManager.Instance.Player.IncreaseProfit();
	}

	IEnumerator CoDistributeWorkerAI()
	{
		while (true)
		{
			yield return new WaitForSeconds(1);
			yield return new WaitUntil(() => Workers.Count > 0);

			foreach (WorkerController worker in Workers)
			{
				if (worker.CurrentSystem != null)
					continue;

				foreach (SystemBase system in RestaurantSystems)
				{
					if (system.HasJob)
					{
						system.AddWorker(worker);
						// worker.WorkerJob = null;
						break;
					}
				}
			}
		}
	}
}
