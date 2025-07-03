using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.AI;
using Unity.VisualScripting;

public class DriveThruCounter : UnlockableBase
{
    private PackingPile _burgerBoxPile;
    private MoneyPile _moneyPile;

    public DriveThruSystem Owner;

    int _spawnMoneyRemaining = 0;

    int _orderBurgerCount = 0;

    private List<Transform> _queuePoints = new List<Transform>();
    List<CarController> _queueCars = new List<CarController>();

    [SerializeField]
    private Transform CarSpawnPos;

    private WorkerInteraction _burgerBoxInteraction;
    public WorkerController CurrentTakingWorker => _burgerBoxInteraction.CurrentWorker;
    public Transform PakingWorkerPos;
    public int PakingCount => _burgerBoxPile.ObjectCount;
    public bool NeedMorePaking => (_spawnMoneyRemaining > 0 && PakingCount < _spawnMoneyRemaining);

    private WorkerInteraction _cashierInteraction;
    public WorkerController CurrentCashierWorker => _cashierInteraction.CurrentWorker;

    public Transform CashierWorkerPos;
    public bool NeedCashier => (CurrentCashierWorker == null);

    public bool IsSellBurgerBox = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _burgerBoxPile = Utils.FindChild<PackingPile>(gameObject);
        _moneyPile = Utils.FindChild<MoneyPile>(gameObject);
        _queuePoints = Utils.FindChild<Waypoints>(gameObject).GetPoints();

        // 포장 인터랙션.
        _burgerBoxInteraction = _burgerBoxPile.GetComponent<WorkerInteraction>();
        _burgerBoxInteraction.InteractInterval = 0.1f;
        _burgerBoxInteraction.OnInteraction = OnBurgerBoxInteraction;

        // 돈 인터랙션.
        _moneyPile.GetComponent<WorkerInteraction>().InteractInterval = 0.02f;
        _moneyPile.GetComponent<WorkerInteraction>().OnInteraction = OnMoneyInteraction;

        GameObject machine = Utils.FindChild(gameObject, "SM_POS_01");
        _cashierInteraction = machine.GetComponent<WorkerInteraction>();
        _cashierInteraction.InteractInterval = 1f;
        _cashierInteraction.OnInteraction = OnCarInteraction;
    }

    private void OnEnable()
    {
        StartCoroutine(CoSpawnCar());
        StartCoroutine(CoSpawnMoney());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void Update()
    {
        UpdateCarQueueAI();
        UpdateCarOrderAI();
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

    private IEnumerator CoSpawnCar()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            if (_queueCars.Count >= _queuePoints.Count)
                continue;

            GameObject go = GameManager.Instance.SpawnCar();
            CarController car = go.GetComponent<CarController>();
            car.GetComponent<NavMeshAgent>().Warp(CarSpawnPos.position);

            Transform dest = _queuePoints.Last();

            car.CurrentDestQueueIndex = _queuePoints.Count - 1;
            car.CarState = Define.ECarState.Queuing;
            car.SetDestination(dest.position, () =>
            {
                car.transform.rotation = dest.rotation;
            });

            _queueCars.Add(car);
        }
    }

    #region CarAI
    private void UpdateCarQueueAI()
    {
        // 줄서기 관리.
        for (int i = 0; i < _queueCars.Count; i++)
        {
            int guestIndex = i;
            CarController car = _queueCars[guestIndex];
            if (car.HasArrivedAtDestination == false)
                continue;

            // 다음 지점으로 이동.
            if (car.CurrentDestQueueIndex > guestIndex)
            {
                car.CurrentDestQueueIndex--;

                Transform dest = _queuePoints[car.CurrentDestQueueIndex];
                car.SetDestination(dest.position, () =>
                {
                    car.transform.rotation = dest.rotation;
                });
            }
        }
    }

    private void UpdateCarOrderAI()
    {
        if (_orderBurgerCount > 0)
            return;

        // 손님이 없다면 리턴.
        int maxOrderCount = Mathf.Min(Define.GUEST_MAX_ORDER_BURGER_COUNT, _queueCars.Count);
        if (maxOrderCount == 0)
            return;

        CarController car = _queueCars[0];
        if (car.HasArrivedAtDestination == false)
            return;

        if (car.CurrentDestQueueIndex != 0)
            return;

        int orderCount = Random.Range(1, maxOrderCount + 1);
        _orderBurgerCount = orderCount;
        car.OrderCount = orderCount;
    }
    #endregion

    #region Interaction
    private void OnBurgerBoxInteraction(WorkerController wc)
    {
        _burgerBoxPile.TrayToPile(wc.Tray);
    }

    private void OnMoneyInteraction(WorkerController wc)
    {
        if (!wc.Tray.IsPlayer)
            return;

        _moneyPile.DespawnObjectWithJump(wc.transform.position, () =>
        {
            GameManager.Instance.Money += 50;
            GameManager.Instance.AddExp(1f);
        });
    }

    private void OnCarInteraction(WorkerController wc)
    {
        if (_orderBurgerCount == 0)
            return;

        CarController car = _queueCars[0];

        // 현재 박스에 있는 버거 수 확인
        int availableBurgerCount = _burgerBoxPile.ObjectCount;
        if (availableBurgerCount < _orderBurgerCount)
            return;

        for (int i = 0; i < _orderBurgerCount; i++)
        {
            _burgerBoxPile.PileToTray(car.Tray);
        }

        _spawnMoneyRemaining = _orderBurgerCount * 10;

        car.SetDestination(Define.CAR_LEAVE_POS);
        car.CarState = Define.ECarState.Leaving;
        car.OrderCount = 0;
        _queueCars.Remove(car);
        _orderBurgerCount = 0;

        IsSellBurgerBox = true;
    }
    #endregion
}