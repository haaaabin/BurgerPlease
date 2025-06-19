using System;
using System.Collections;
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

    // 한 프레임 기다려서 모든 오브젝트 초기화 끝나고 실행
    public IEnumerator CoInitialize()
    {
        yield return new WaitForEndOfFrame();

        Player = FindAnyObjectByType<PlayerController>();
        Restaurant = FindAnyObjectByType<Restaurant>();

        // int index = Restaurant.StageNum;
        // Restaurant.SetInfo(SaveData.Restaurants[index]);

        // StartCoroutine(CoSaveData());
    }

    private IEnumerator CoSaveData()
    {
        while (true)
        {
            yield return new WaitForSeconds(10);

            SaveData.RestaurantIndex = Restaurant.StageNum;
            SaveData.PlayerPosition = Player.transform.position;

            SaveManager.Instance.SaveGame();
        }
    }

    #region UI
    public UI_UpgradeEmployeePopup UpgradeEmployeePopup;
    public UI_GameScene GameSceneUI;
    #endregion

    #region Event
    Action[] _events = new Action[(int)EEventType.MaxCount];

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
    #endregion

    #region Burger
    public GameObject BurgerPrefab;

    private Transform _burgerRoot;
    public Transform BurgerRoot
    {
        get
        {
            if (_burgerRoot == null)
            {
                GameObject go = new GameObject("@BurgerRoot");
                _burgerRoot = go.transform;
            }
            return _burgerRoot;
        }
    }

    public GameObject SpawnBurger()
    {
        GameObject go = Instantiate(BurgerPrefab);
        go.name = BurgerPrefab.name;
        go.transform.parent = BurgerRoot;
        return go;
    }

    public void DeSpawnBurger(GameObject burger)
    {
        Destroy(burger);
    }
    #endregion

    #region Money
    public GameObject MoneyPrefab;

    private Transform _moneyRoot;
    public Transform MoneyRoot
    {
        get
        {
            if (_moneyRoot == null)
            {
                GameObject go = new GameObject("@MoneyRoot");
                _moneyRoot = go.transform;
            }
            return _moneyRoot;
        }
    }

    public GameObject SpawnMoney()
    {
        GameObject go = Instantiate(MoneyPrefab);
        go.name = MoneyPrefab.name;
        go.transform.parent = MoneyRoot;
        return go;
    }

    public void DeSpawnMoney(GameObject money)
    {
        Destroy(money);
    }
    #endregion

    #region Guest
    public GameObject GuestPrefab;

    private Transform _guestRoot;
    public Transform GuestRoot
    {
        get
        {
            if (_guestRoot == null)
            {
                GameObject go = new GameObject("@GuestRoot");
                _guestRoot = go.transform;
            }
            return _guestRoot;
        }
    }

    public GameObject SpawnGuest()
    {
        GameObject go = Instantiate(GuestPrefab);
        go.name = GuestPrefab.name;
        go.transform.parent = GuestRoot;
        return go;
    }

    public void DeSpawnGuest(GameObject guest)
    {
        Destroy(guest);
    }
    #endregion

    #region Trash
    public GameObject TrashPrefab;

    private Transform _trashRoot;
    public Transform TrashRoot
    {
        get
        {
            if (_trashRoot == null)
            {
                GameObject go = new GameObject("@TrashRoot");
                _trashRoot = go.transform;
            }
            return _trashRoot;
        }
    }

    public GameObject SpawnTrash()
    {
        GameObject go = Instantiate(TrashPrefab);
        go.name = TrashPrefab.name;
        go.transform.parent = TrashRoot;
        return go;
    }

    public void DeSpawnTrash(GameObject trash)
    {
        Destroy(trash);
    }
    #endregion

    #region Worker
    public GameObject WorkerPrefab;

    private Transform _workerRoot;
    public Transform WorkerRoot
    {
        get
        {
            if (_workerRoot == null)
            {
                GameObject go = new GameObject("@WorkerRoot");
                _workerRoot = go.transform;
            }
            return _workerRoot;
        }
    }

    public GameObject SpawnWorker()
    {
        GameObject go = Instantiate(WorkerPrefab);
        go.name = WorkerPrefab.name;
        go.transform.parent = WorkerRoot;
        return go;
    }

    public void DeSpawnWorker(GameObject worker)
    {
        Destroy(worker);
    }
    #endregion
}