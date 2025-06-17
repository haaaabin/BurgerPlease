using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

// 1. 햄버거 쌓이는 Pile (ok)
// 2. 햄버거 쌓이는 Trigger (ok)
// 3. 돈 쌓이는 Pile (ok)
// 4. 돈 먹는 Trigger (ok)
// 5. 손님 줄 
// 6. 손님 계산 받기 Trigger (손님 있어야 함. 햄버거 있어야 함. 자리 있어야 함)
public class Counter : UnlockableBase
{
    private BurgerPile _burgerPile;
    private MoneyPile _moneyPile;
    public MainCounterSystem Owner;

    int _spawnMoneyRemaining = 0;

    // 주문하는 햄버거 수
    int _nextOrderBurgerCount = 0;

    private List<Vector3> _queuePoints = new List<Vector3>();
    List<GuestController> _queueGuests = new List<GuestController>();

    public List<WorkerController> Workers = new List<WorkerController>();
    public List<Table> Tables => Owner?.Tables;

    private WorkerInteraction _burgerInteraction;
    public WorkerController CurrentBurgerWorker => _burgerInteraction.CurrentWorker;
    public Transform BurgerWorkerPos;
    public int BurgerCount => _burgerPile.ObjectCount;
    public bool NeedMoreBurgers => (_nextOrderBurgerCount > 0 && BurgerCount < _nextOrderBurgerCount);

    private WorkerInteraction _casherInteraction;
    public WorkerController CurrentCasherWorker => _casherInteraction.CurrentWorker;
    public Transform CashierWorkerPos;
    public bool NeedCashier => (CurrentCasherWorker == null);

    void Start()
    {
        _burgerPile = Utils.FindChild<BurgerPile>(gameObject);
        _moneyPile = Utils.FindChild<MoneyPile>(gameObject);
        _queuePoints = Utils.FindChild<Waypoints>(gameObject).GetPoints();

        // 햄버거 인터렉션
        _burgerInteraction = _burgerPile.GetComponent<WorkerInteraction>();
        _burgerInteraction.InteractInterval = 0.1f;
        _burgerInteraction.OnInteraction = OnBurgerInteraction;

        // 돈 인터렉션
        _moneyPile.GetComponent<WorkerInteraction>().InteractInterval = 0.1f;
        _moneyPile.GetComponent<WorkerInteraction>().OnInteraction = OnMoneyInteraction;

        // 손님 인터렉션
        GameObject machine = Utils.FindChild(gameObject, "Machine");
        _casherInteraction = machine.GetComponent<WorkerInteraction>();
        _casherInteraction.InteractInterval = 1f;
        _casherInteraction.OnInteraction = OnGuestInteraction;

        _spawnMoneyRemaining = 50;
        StartCoroutine(CoSpawnMoney());
        StartCoroutine(CoSpawnGuest());
    }

    void Update()
    {
        // 손님 AI
        UpdateGuestQueueAI();
        UpdateGuestOrderAI();
    }

    private IEnumerator CoSpawnMoney()
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

    private IEnumerator CoSpawnGuest()
    {
        while (true)
        {
            yield return new WaitForSeconds(Define.GUEST_SPAWN_INTERVAL);

            if (_queueGuests.Count == _queuePoints.Count)
                continue;

            GameObject go = GameManager.Instance.SpawnGuest();
            GuestController guest = go.GetComponent<GuestController>();
            guest.CurrentDestQueueIndex = _queuePoints.Count - 1;
            guest.GuestState = Define.EGuestState.Queuing;
            guest.SetDestination(_queuePoints.Last());

            _queueGuests.Add(guest);
        }
    }

    private void UpdateGuestQueueAI()
    {
        for (int i = 0; i < _queueGuests.Count; i++)
        {
            int guestIndex = i;
            GuestController guest = _queueGuests[guestIndex];
            if (guest.HasArrivedAtDestination == false)
                continue;

            // 내가 위치해야 할 인덱스로 가기 위해 다음 지점으로 이동
            if (guest.CurrentDestQueueIndex > guestIndex)
            {
                guest.CurrentDestQueueIndex--;

                Vector3 dest = _queuePoints[guest.CurrentDestQueueIndex];
                guest.Destination = dest;
            }
        }
    }

    private void UpdateGuestOrderAI()
    {
        //  현재 손님이 주문한 버거를 아직 다 만들지 않았다면 새 주문을 받지 않음
        if (_nextOrderBurgerCount > 0)
            return;

        // 손님이 없다면 리턴
        int maxOrderCount = Mathf.Min(Define.GUEST_MAX_ORDER_BURGER_COUNT, _queueGuests.Count);
        if (maxOrderCount == 0)
            return;

        // 이동 중인지 확인
        GuestController guest = _queueGuests[0];
        if (guest.HasArrivedAtDestination == false)
            return;

        // 맨 앞자리 도착
        if (guest.CurrentDestQueueIndex != 0)
            return;

        // 주문 진행
        int orderCount = Random.Range(1, maxOrderCount + 1);
        _nextOrderBurgerCount = orderCount;
        guest.OrderCount = orderCount;

    }

    private void OnBurgerInteraction(WorkerController wc)
    {
        if (wc.Tray.CurrentTrayObjectType == Define.EObjectType.Trash)
            return;

        _burgerPile.TrayToPile(wc.Tray);
    }

    private void OnMoneyInteraction(WorkerController wc)
    {
        if (wc.Tray.IsPlayer == false) return;
        _moneyPile.DeSpawnObjectWithJump(wc.transform.position, () =>
        {

            GameManager.Instance.Money += 100;
            // Debug.Log("GameManager.Instance.Money :" + GameManager.Instance.Money);
        });
    }

    private void OnGuestInteraction(WorkerController wc)
    {
        // 자리 수가 맞는 테이블이 있어야 함
        Table destTable = FindTableToServeGuest();
        if (destTable == null)
            return;

        for (int i = 0; i < _nextOrderBurgerCount; i++)
        {
            GuestController guest = _queueGuests[i];
            guest.SetDestination(destTable.Chairs[i].position);
            guest.GuestState = Define.EGuestState.Serving;
            guest.OrderCount = 0;

            _burgerPile.PileToTray(guest.Tray);

            _spawnMoneyRemaining = 10;
            StartCoroutine(CoSpawnMoney());
        }

        // 점유한다.
        destTable.Guests = _queueGuests.GetRange(0, _nextOrderBurgerCount);
        destTable.TableState = Define.ETableState.Reversed;

        // 줄에서 제거
        _queueGuests.RemoveRange(0, _nextOrderBurgerCount);

        // 주문 처리 끝났으므로 0으로 리셋
        _nextOrderBurgerCount = 0;
    }

    public Table FindTableToServeGuest()
    {
        // 손님 있어야 함
        if (_nextOrderBurgerCount == 0)
            return null;

        if (_burgerPile.ObjectCount < _nextOrderBurgerCount)
            return null;

        foreach (Table table in Tables)
        {
            if (table.IsOccupied)
                continue;

            if (_nextOrderBurgerCount > table.Chairs.Count)
                continue;

            return table;
        }

        return null;
    }
}