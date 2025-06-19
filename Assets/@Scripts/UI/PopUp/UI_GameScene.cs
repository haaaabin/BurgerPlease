using TMPro;
using UnityEngine;

public class UI_GameScene : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _moneyCountText;

    [SerializeField]
    private TextMeshProUGUI _toastMessageText;

    private void OnEnable()
    {
        RefreshUI();
        GameManager.Instance.AddEventListener(Define.EEventType.MoneyChanged, RefreshUI);
    }

    private void OnDisable()
    {
        GameManager.Instance.RemoveEventListener(Define.EEventType.MoneyChanged, RefreshUI);
    }

    public void RefreshUI()
    {
        long money = GameManager.Instance.Money;
        _moneyCountText.text = Utils.GetMoneyText(money);
    }

    public void SetToastMessage(string message)
    {
        _toastMessageText.text = message;
        _toastMessageText.enabled = (string.IsNullOrEmpty(message) == false);
    }
}