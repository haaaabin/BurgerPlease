using UnityEngine;

public class DriveThruCounter : UnlockableBase
{
    private PakingPile _pakingPile;
    private MoneyPile _moneyPile;

    public DriveThruSystem Owner;

    int _spawnMoneyRemaining = 0;

    int _nextOrderBurgerCount = 0;

    private WorkerInteraction _pakingInteraction;
    public WorkerController CurrentTakingWorker => _pakingInteraction.CurrentWorker;
    public Transform PakingWorkerPos;
    public int PakingCount => _pakingPile.ObjectCount;
    public bool NeedMorePaking => (_spawnMoneyRemaining > 0 && PakingCount < _spawnMoneyRemaining);

    private WorkerInteraction _cashierInteraction;
    public WorkerController CurrentCashierWorker => _cashierInteraction.CurrentWorker;

    public Transform CashierWorkerPos;
    public bool NeedCashier => (CurrentCashierWorker == null);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _pakingPile = Utils.FindChild<PakingPile>(gameObject);
        _moneyPile = Utils.FindChild<MoneyPile>(gameObject);

        // 포장 인터랙션.
        _pakingInteraction = _pakingPile.GetComponent<WorkerInteraction>();
        _pakingInteraction.InteractInterval = 0.1f;
        _pakingInteraction.OnInteraction = OnPakingInteraction;
    }

    private void OnPakingInteraction(WorkerController wc)
    {
        _pakingPile.TrayToPile(wc.Tray);
    }
}