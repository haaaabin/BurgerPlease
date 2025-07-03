using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
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

	public int Level
	{
		get { return SaveData.Level; }
		set
		{
			SaveData.Level = value;
		}
	}

	public float CurrentExp
	{
		get { return SaveData.CurrentExp; }
		set
		{
			SaveData.CurrentExp = value;
			BroadcastEvent(EEventType.ExpChanged);
		}
	}

	public float GetMaxExp()
	{
		return 50 + (Level - 1) * 50;
	}

	public void AddExp(float amount)
	{
		CurrentExp += amount;
		Debug.Log("CurrentExp" + CurrentExp);

		while (CurrentExp >= GetMaxExp())
		{
			CurrentExp -= GetMaxExp();
			Level++;
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

		Player.transform.position = SaveData.PlayerPosition;

		StartCoroutine(CoSaveData());
	}

	IEnumerator CoSaveData()
	{
		while (true)
		{
			yield return new WaitForSeconds(2);

			SaveData.RestaurantIndex = Restaurant.StageNum;
			SaveData.PlayerPosition = Player.transform.position;

			SaveData.Level = Level;
			SaveData.CurrentExp = CurrentExp;
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

	public GameObject PackingBoxPrefab;
	public GameObject SpawnPackingBox() { return PoolManager.Instance.Pop(PackingBoxPrefab); }
	public void DespawnPackingBox(GameObject packingBox) { PoolManager.Instance.Push(packingBox); }

	public GameObject CarPrefab;
	public GameObject SpawnCar() { return PoolManager.Instance.Pop(CarPrefab); }
	public void Despawncar(GameObject car) { PoolManager.Instance.Push(car); }

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
