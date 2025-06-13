using TMPro;
using UnityEngine;

public class UI_GameScene : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _moneyCountText;

    private void OnEnable()
    {
        RefreshUI();
        GameManager.Instance.AddEventListener(Define.EEventType.MoneyChanged, RefreshUI);
        GameManager.Instance.OnMoneyChanged += RefreshUI;
    }

    private void OnDisable()
    {
        GameManager.Instance.RemoveEventListener(Define.EEventType.MoneyChanged, RefreshUI);
        GameManager.Instance.OnMoneyChanged -= RefreshUI;
    }

    public void RefreshUI()
    {
        long money = GameManager.Instance.Money;
        _moneyCountText.text = Utils.GetMoneyText(money);
    }
}