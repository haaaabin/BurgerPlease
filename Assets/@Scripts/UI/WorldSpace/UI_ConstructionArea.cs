using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(WorkerInteraction))]
public class UI_ConstructionArea : MonoBehaviour
{
    [SerializeField]
    Slider _slider;

    [SerializeField]
    TextMeshProUGUI _moneyText;

    public UnlockableBase Owner;
    public long SpentMoney;
    public long TotalUpgradeMoney;
    public long MoneyRemaining => TotalUpgradeMoney - SpentMoney;

    private void Start()
    {
        GetComponent<WorkerInteraction>().OnInteraction = OnWorkerInteraction;
        GetComponent<WorkerInteraction>().InteractInterval = Define.CONSTRUCTION_UPGRADE_INTERVAL;

        if (Owner == null)
            Owner = Utils.FindChild<UnlockableBase>(transform.root.gameObject);

        Owner.gameObject.SetActive(false);

        SpentMoney = 0;
        TotalUpgradeMoney = 10000;

        RefreshUI();
    }

    public void OnWorkerInteraction(WorkerController wc)
    {
        if (Owner == null)
            return;

        long money = (long)(TotalUpgradeMoney / (1 / Define.CONSTRUCTION_UPGRADE_INTERVAL));

        if (GameManager.Instance.Money < money)
            return;

        GameManager.Instance.Money -= money;
        SpentMoney += money;

        if (SpentMoney >= TotalUpgradeMoney)
        {
            SpentMoney = TotalUpgradeMoney;

            Owner.gameObject.SetActive(true);
            gameObject.SetActive(false);

            GameManager.Instance.BroadcastEvent(Define.EEventType.UnlockProp);
        }

        RefreshUI();
    }

    private void RefreshUI()
    {
        _slider.value = SpentMoney / (float)TotalUpgradeMoney;
        _moneyText.text = Utils.GetMoneyText(MoneyRemaining);
    }
}