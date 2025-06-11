using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static Define;
using System.Collections;

public class Table : MonoBehaviour
{
    public List<Transform> Chairs = new List<Transform>();

    public List<GuestController> Guests = new List<GuestController>();

    private TrashPile _trashPile;
    private BurgerPile _burgerPile;
    private MoneyPile _moneyPile;

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

            return Guests.Count > 0;
        }
    }

    void Start()
    {
        _burgerPile = Utils.FindChild<BurgerPile>(gameObject);
        _moneyPile = Utils.FindChild<MoneyPile>(gameObject);
        _trashPile = Utils.FindChild<TrashPile>(gameObject);

        // 쓰레기 인터랙션.
        _trashPile.GetComponent<PlayerInteraction>().InteractInterval = 0.02f;
        _trashPile.GetComponent<PlayerInteraction>().OnPlayerInteraction = OnPlayerTrashInteraction;

        // 돈 인터랙션.
        _moneyPile.GetComponent<PlayerInteraction>().InteractInterval = 0.02f;
        _moneyPile.GetComponent<PlayerInteraction>().OnPlayerInteraction = OnPlayerMoneyInteraction;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGuestAI();
    }

    float _eatingTimeRemaining = 0;

    private void UpdateGuestAI()
    {
        if (IsOccupied == false)
            return;

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

                Transform burger = guest.Tray.RemoveFromTray();
                _burgerPile.AddToPile(burger.gameObject, true);
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

                for (int i = 0; i < Guests.Count; i++)
                {
                    GameObject burger = _burgerPile.RemoveFromPile();
                    if (burger == null)
                        return;

                    GameManager.Instance.DeSpawnBurger(burger);
                }

                SpawnTrashRemaining = Guests.Count;

                StartCoroutine(CoSpawnTrash());

                foreach (GuestController guest in Guests)
                {
                    guest.GuestState = EGuestState.Leaving;
                    guest.Destination = Define.GUEST_LEAVE_POS;
                }

                Guests.Clear();
                TableState = ETableState.Dirty;
            }
            else if (TableState == ETableState.Dirty)
            {
                if (_trashPile.ObjectCount == 0)
                    TableState = ETableState.None;
            }
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

            GameObject go = GameManager.Instance.SpawnTrash();
            _trashPile.AddToPile(go, jump: true);
        }
    }

    #region Interaction
    private void OnPlayerMoneyInteraction(PlayerController pc)
    {
        GameObject money = _moneyPile.RemoveFromPile();
        if (money == null)
            return;

        Vector3 targetPos = pc.transform.position + Vector3.up * 0.8f;

        money.transform.DOJump(
                targetPos,
                1.8f,
                1,
                0.5f
                ).OnComplete(() =>
                {
                    GameManager.Instance.DeSpawnMoney(money);
                });
    }

    private void OnPlayerTrashInteraction(PlayerController pc)
    {
        if (pc.Tray.ETrayObject == Define.ETrayObject.Burger)
            return;

        GameObject trash = _trashPile.RemoveFromPile();
        if (trash == null)
            return;

        pc.Tray.AddToTray(trash.transform);
    }
    #endregion
}
