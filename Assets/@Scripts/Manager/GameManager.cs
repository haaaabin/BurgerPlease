using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Overlays;
using UnityEngine;
using static Define;

public class GameManager : Singleton<GameManager>
{
	public Vector2 JoystickDir { get; set; } = Vector2.zero;

	public PlayerController Player;

	public Restaurant Restaurant;
	private GameSaveData SaveData
	{
		get
		{
			return SaveManager.Instance.SaveData;
		}
	}

	public long Money
	{
		get { return SaveData.Money; }
		set
		{
			SaveData.Money = value;
			BroadcastEvent(EEventType.MoneyChanged);
		}
	}

	private void Start()
	{
		UpgradeEmployeePopup = Utils.FindChild<UI_UpgradeEmployeePopup>(gameObject);
		UpgradeEmployeePopup.gameObject.SetActive(false);
		StartCoroutine(CoInitialize());
	}

	// 한 프레임 기다려서 모든 오브젝트 초기화 끝나고 실행.
	public IEnumerator CoInitialize()
	{
		yield return new WaitForEndOfFrame();

		Player = GameObject.FindAnyObjectByType<PlayerController>();
		Restaurant = GameObject.FindAnyObjectByType<Restaurant>();

		int index = Restaurant.StageNum;
		Restaurant.SetInfo(SaveData.Restaurants[index]);

		StartCoroutine(CoSaveData());
	}

	IEnumerator CoSaveData()
	{
		while (true)
		{
			yield return new WaitForSeconds(10);

			SaveData.RestaurantIndex = Restaurant.StageNum;
			SaveData.PlayerPosition = Player.transform.position;

			SaveManager.Instance.SaveGame();
		}
	}

	#region UIManager
	public UI_UpgradeEmployeePopup UpgradeEmployeePopup;
	public UI_GameScene GameSceneUI;
	#endregion

	#region ObjectManager
	public GameObject WorkerPrefab;
	public GameObject SpawnWorker() { return PoolManager.Instance.Pop(WorkerPrefab); }
	public void DespawnWorker(GameObject worker) { PoolManager.Instance.Push(worker); }

	public GameObject BurgerPrefab;
	public GameObject SpawnBurger() { return PoolManager.Instance.Pop(BurgerPrefab); }
	public void DespawnBurger(GameObject burger) { PoolManager.Instance.Push(burger); }

	public GameObject MoneyPrefab;
	public GameObject SpawnMoney() { return PoolManager.Instance.Pop(MoneyPrefab); }
	public void DespawnMoney(GameObject money) { PoolManager.Instance.Push(money); }

	public GameObject TrashPrefab;
	public GameObject SpawnTrash() { return PoolManager.Instance.Pop(TrashPrefab); }
	public void DespawnTrash(GameObject trash) { PoolManager.Instance.Push(trash); }

	public GameObject GuestPrefab;
	public GameObject SpawnGuest() { return PoolManager.Instance.Pop(GuestPrefab); }
	public void DespawnGuest(GameObject guest) { PoolManager.Instance.Push(guest); }
	#endregion

	#region Events
	public void AddEventListener(EEventType type, Action action)
	{
		int index = (int)type;
		if (_events.Length < index)
			return;

		_events[index] += action;
	}

	public void RemoveEventListener(EEventType type, Action action)
	{
		int index = (int)type;
		if (_events.Length < index)
			return;

		_events[index] -= action;
	}

	public void BroadcastEvent(EEventType type)
	{
		int index = (int)type;
		if (_events.Length < index)
			return;

		_events[index]?.Invoke();
	}

	Action[] _events = new Action[(int)EEventType.MaxCount];
	#endregion
}
