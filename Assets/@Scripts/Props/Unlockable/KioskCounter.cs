using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class KioskCounter : UnlockableBase
{
    public KioskSystem Owner;
    private BurgerPile _burgerPile;
    private MoneyPile _moneyPile;

    [SerializeField] private Transform _queuePoint1;
    [SerializeField] private Transform _queuePoint2;
    private GuestController[] _queueSlots = new GuestController[2]; // 0번: queuePoint1, 1번: queuePoint2

    [SerializeField] private Transform _guestSpawnPos;

    private WorkerInteraction _burgerInteraction;
    public WorkerController CurrentBurgerWorker => _burgerInteraction.CurrentWorker;
    public Transform BurgerWorkerPos;
    public int BurgerCount => _burgerPile.ObjectCount;
    public bool NeedMoreBurgers => (_orderBurgerCount > 0 && BurgerCount < _orderBurgerCount);

    private int _spawnMoneyRemaining = 0;
    private int _orderBurgerCount = 0;

    private WorkerInteraction _cashierInteraction;
    public WorkerController CurrentCashierWorker => _cashierInteraction.CurrentWorker;
    public Transform CashierWorkerPos;
    public bool NeedCashier => (CurrentCashierWorker == null);
    public bool IsEnoughSellBurger => BurgerCount >= _orderBurgerCount;
    public bool IsSelling = false;

    void Start()
    {
        _burgerPile = Utils.FindChild<BurgerPile>(gameObject);
        _moneyPile = Utils.FindChild<MoneyPile>(gameObject);

        _burgerInteraction = _burgerPile.GetComponent<WorkerInteraction>();
        _burgerInteraction.InteractInterval = 0.1f;
        _burgerInteraction.OnInteraction = OnBurgerInteraction;

        // 돈 인터랙션.
        _moneyPile.GetComponent<WorkerInteraction>().InteractInterval = 0.02f;
        _moneyPile.GetComponent<WorkerInteraction>().OnInteraction = OnMoneyInteraction;

        // 손님 인터랙션.
        GameObject pickUpDesk = Utils.FindChild(gameObject, "PickUpDesk");
        _cashierInteraction = pickUpDesk.GetComponent<WorkerInteraction>();
        _cashierInteraction.InteractInterval = 1;
        _cashierInteraction.OnInteraction = OnGuestInteraction;
    }


    private void OnEnable()
    {
        StartCoroutine(CoSpawnGuest());
        StartCoroutine(CoSpawnMoney());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator CoSpawnGuest()
    {
        while (true)
        {
            yield return new WaitForSeconds(15);

            for (int i = 0; i < 2; i++)
            {
                if (_queueSlots[i] != null)
                    continue;

                GameObject go = GameManager.Instance.SpawnGuest();
                go.transform.position = _guestSpawnPos.position;

                GuestController guest = go.GetComponent<GuestController>();
                // guest.GuestState = Define.EGuestState.Queuing;

                Transform dest = (i == 0) ? _queuePoint1 : _queuePoint2;
                guest.CurrentDestQueueIndex = i;
                guest.SetDestination(dest.position, () =>
                {
                    guest.transform.rotation = dest.rotation;
                    guest.GuestState = Define.EGuestState.Kiosk;
                    guest.bubbleGameObject.SetActive(true);
                });

                _queueSlots[i] = guest;
                break; // 한 명만 스폰하고 대기
            }
        }
    }
    IEnumerator CoSpawnMoney()
    {
        while (true)
        {
            yield return new WaitForSeconds(Define.MONEY_SPAWN_INTERVAL);

            if (_spawnMoneyRemaining <= 0)
                continue;

            _spawnMoneyRemaining--;

            _moneyPile.SpawnObject();
        }
    }

    private void Update()
    {
        UpdateGuestOrderAI();
    }

    private bool _isOrdering = false;

    private void UpdateGuestOrderAI()
    {
        // 이미 주문 진행 중이면 무시
        if (_isOrdering)
            return;

        for (int i = 0; i < _queueSlots.Length; i++)
        {
            GuestController guest = _queueSlots[i];
            if (guest == null || !guest.HasArrivedAtDestination)
                continue;

            StartCoroutine(AutoOrderRoutine(guest));
            break; // 한 명만 주문 처리
        }
    }

    private IEnumerator AutoOrderRoutine(GuestController guest)
    {
        _isOrdering = true;

        float waitTime = Random.Range(2f, 5f);
        yield return new WaitForSeconds(waitTime);

        guest.GuestState = Define.EGuestState.Queuing;
        guest.bubbleGameObject.SetActive(false);
        // 주문 처리
        int orderCount = Random.Range(4, Define.KIOSK_MAX_BURGER_COUNT + 1);
        _orderBurgerCount = orderCount;
        guest.OrderCount = orderCount;
        guest.IsWaitingForBurger = true;
    }

    private void OnBurgerInteraction(WorkerController wc)
    {
        _burgerPile.TrayToPile(wc.Tray);
    }

    private void OnMoneyInteraction(WorkerController wc)
    {
        if (!wc.Tray.IsPlayer)
            return;

        _moneyPile.DeSpawnObjectWithJump(wc.transform.position, () =>
        {
            GameManager.Instance.AddMoney(Define.MONEY_PER_DRIVE_THRU_BURGER);
            GameManager.Instance.AddExp(1f);
        });
    }

    void OnGuestInteraction(WorkerController wc)
    {
        if (_orderBurgerCount == 0)
            return;

        GuestController guest = null;
        int guestIndex = -1;

        // 도착했고 주문한 손님을 찾기
        for (int i = 0; i < _queueSlots.Length; i++)
        {
            if (_queueSlots[i] == null)
                continue;

            if (_queueSlots[i].HasArrivedAtDestination && _queueSlots[i].IsWaitingForBurger)
            {
                guest = _queueSlots[i];
                guestIndex = i;
                break;
            }
        }

        if (guest == null)
            return;

        int availableBurgerCount = _burgerPile.ObjectCount;
        if (availableBurgerCount < _orderBurgerCount)
            return;

        for (int i = 0; i < _orderBurgerCount; i++)
        {
            _burgerPile.PileToTray(guest.Tray);
        }

        _spawnMoneyRemaining = _orderBurgerCount * 10;

        guest.SetDestination(Define.GUEST_LEAVE_POS, () =>
        {
            guest.Tray.ClearTray();
            GameManager.Instance.DeSpawnGuest(guest.gameObject);
        });

        guest.GuestState = Define.EGuestState.Leaving;
        guest.OrderCount = 0;
        guest.IsWaitingForBurger = false;
        _queueSlots[0] = null;
        _orderBurgerCount = 0;
        _isOrdering = false;
        
        IsSelling = true;
    }

}