using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static Define;
using System.Collections;
using Unity.VisualScripting;

public class Table : UnlockableBase
{
    public List<Transform> Chairs = new List<Transform>();

    public List<GuestController> Guests = new List<GuestController>();

    private TrashPile _trashPile;
    private BurgerPile _burgerPile;
    private MoneyPile _moneyPile;

    public Transform WorkerPos;

    public int SpawnMoneyRemaining = 0;
    public int SpawnTrashRemaining = 0;

    private ETableState _tableState = ETableState.None;
    public ETableState TableState
    {
        get { return _tableState; }
        set
        {
            _tableState = value;
        }
    }

    public bool IsOccupied
    {
        get
        {
            if (_trashPile.ObjectCount > 0)
                return true;

            return TableState != ETableState.None;
        }
    }

    void Start()
    {
        _burgerPile = Utils.FindChild<BurgerPile>(gameObject);
        _moneyPile = Utils.FindChild<MoneyPile>(gameObject);
        _trashPile = Utils.FindChild<TrashPile>(gameObject);

        // 쓰레기 인터랙션.
        _trashPile.GetComponent<WorkerInteraction>().InteractInterval = 0.02f;
        _trashPile.GetComponent<WorkerInteraction>().OnInteraction = OnTrashInteraction;

        // 돈 인터랙션.
        _moneyPile.GetComponent<WorkerInteraction>().InteractInterval = 0.02f;
        _moneyPile.GetComponent<WorkerInteraction>().OnInteraction = OnMoneyInteraction;

        StartCoroutine(CoSpawnTrash());
        StartCoroutine(CoSpawnMoney());
    }

    void Update()
    {
        UpdateGuestAI();
    }

    float _eatingTimeRemaining = 0;

    private void UpdateGuestAI()
    {
        if (TableState == ETableState.Reversed)
        {
            // 손님이 모두 착석하기 기다린다.
            foreach (GuestController guest in Guests)
            {
                if (guest.HasArrivedAtDestination == false)
                    return;
            }

            // 식사 시작
            for (int i = 0; i < Guests.Count; i++)
            {
                GuestController guest = Guests[i];
                guest.GuestState = Define.EGuestState.Eating;
                guest.transform.rotation = Chairs[i].rotation;

                _burgerPile.TrayToPile(guest.Tray);
            }

            _eatingTimeRemaining = Random.Range(5, 11);
            TableState = ETableState.Eating;
        }
        else if (TableState == ETableState.Eating)
        {
            _eatingTimeRemaining -= Time.deltaTime;
            if (_eatingTimeRemaining < 0)
            {
                _eatingTimeRemaining = 0;

                // 버거 제거
                for (int i = 0; i < Guests.Count; i++)
                {
                    _burgerPile.DeSpawnObject();
                }

                // 쓰레기 생성
                SpawnTrashRemaining = Guests.Count;

                SpawnMoneyRemaining = Guests.Count;

                // 손님 퇴장
                foreach (GuestController guest in Guests)
                {
                    guest.GuestState = EGuestState.Leaving;
                    guest.SetDestination(GUEST_LEAVE_POS, () =>
                    {
                        GameManager.Instance.DeSpawnGuest(guest.gameObject);
                    });
                }

                Guests.Clear();
                TableState = ETableState.Dirty;
            }
        }
        else if (TableState == ETableState.Dirty)
        {
            if (SpawnTrashRemaining == 0 && _trashPile.ObjectCount == 0)
                TableState = ETableState.None;

        }

    }

    private IEnumerator CoSpawnTrash()
    {
        while (true)
        {
            yield return new WaitForSeconds(Define.TRASH_SPAWN_INTERVAL);

            if (SpawnTrashRemaining <= 0)
                continue;

            SpawnTrashRemaining--;

            _trashPile.SpawnObject();
        }
    }

    private IEnumerator CoSpawnMoney()
    {
        while (true)
        {
            yield return new WaitForSeconds(Define.MONEY_SPAWN_INTERVAL);

            if (SpawnMoneyRemaining <= 0)
                continue;

            SpawnMoneyRemaining--;

            _moneyPile.SpawnObject();
        }
    }

    #region Interaction

    private void OnTrashInteraction(WorkerController wc)
    {
        if (wc.Tray.CurrentTrayObjectType == Define.EObjectType.Burger)
            return;

        _trashPile.PileToTray(wc.Tray);
    }

    private void OnMoneyInteraction(WorkerController wc)
    {
        if (wc.Tray.IsPlayer == false) return;

        _moneyPile.DeSpawnObjectWithJump(wc.transform.position, () =>
        {
            GameManager.Instance.Money += 100;
        });
    }
    #endregion
}
