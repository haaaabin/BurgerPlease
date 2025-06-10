using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

// 1. 햄버거 쌓이는 Pile (ok)
// 2. 햄버거 쌓이는 Trigger (ok)
// 3. 돈 쌓이는 Pile (ok)
// 4. 돈 먹는 Trigger (ok)
// 5. 손님 줄 
// 6. 손님 계산 받기 Trigger (손님 있어야 함. 햄버거 있어야 함. 자리 있어야 함)
public class Counter : MonoBehaviour
{
    private BurgerPile _burgerPile;
    private MoneyPile _moneyPile;

    int _spawnMoneyRemaining = 0;

    private List<Vector3> _queuePoints = new List<Vector3>();

    void Start()
    {
        _burgerPile = Utils.FindChild<BurgerPile>(gameObject);
        _moneyPile = Utils.FindChild<MoneyPile>(gameObject);

        // 햄버거 인터렉션
        _burgerPile.GetComponent<PlayerInteraction>().InteractInterval = 0.1f;
        _burgerPile.GetComponent<PlayerInteraction>().OnPlayerInteraction = OnPlayerBurgerInteraction;

        // 돈 인터렉션
        _moneyPile.GetComponent<PlayerInteraction>().InteractInterval = 0.1f;
        _moneyPile.GetComponent<PlayerInteraction>().OnPlayerInteraction = OnPlayerMoneyInteraction;

        StartCoroutine(CoSpawnMoney());

        _spawnMoneyRemaining = 30;
    }

    private IEnumerator CoSpawnMoney()
    {
        while (true)
        {
            yield return new WaitForSeconds(Define.MONEY_SPAWN_INTERVAL);

            if (_spawnMoneyRemaining <= 0)
                continue;

            _spawnMoneyRemaining--;

            GameObject go = GameManager.Instance.SpawnMoney();
            _moneyPile.AddToPile(go, true);
        }
    }
    private void OnPlayerBurgerInteraction(PlayerController pc)
    {
        Transform t = pc.Tray.RemoveFromTray();

        if (t == null)
            return;

        _burgerPile.AddToPile(t.gameObject, true);
    }

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

}