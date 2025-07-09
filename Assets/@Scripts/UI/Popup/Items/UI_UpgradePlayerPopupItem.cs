using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Define;

public enum EUpgradePlayerPopupItemType
{
    None,
    Speed,
    Capacity,
    Profit,
}
public class UI_UpgradePlayerPopupItem : MonoBehaviour
{
    [SerializeField]
    private Button _purchaseButton;

    [SerializeField]
    private TextMeshProUGUI _costText;

    [SerializeField]
    private Slider _slider;

    [SerializeField]
    private GameObject _upgradeEffectArrow;
    EUpgradePlayerPopupItemType _type = EUpgradePlayerPopupItemType.None;

    long _money = 0;

    void Start()
    {
        _purchaseButton.onClick.AddListener(OnClickPurchaseButton);
    }

    void Update()
    {
        if (GameManager.Instance.Money < _money)
        {
            _purchaseButton.interactable = false;
        }
        else
        {
            _purchaseButton.interactable = true;
        }
    }

    public void SetInfo(EUpgradePlayerPopupItemType type, long money)
    {
        _type = type;
        _money = money;
        RefreshUI();
    }

    public void RefreshUI()
    {
        _costText.text = Utils.GetMoneyText(_money);
    }

    public void OnClickPurchaseButton()
    {
        if (GameManager.Instance.Money < _money)
            return;

        GameManager.Instance.Money -= _money;

        switch (_type)
        {
            case EUpgradePlayerPopupItemType.Speed:
                {
                    GameManager.Instance.BroadcastEvent(EEventType.UpgradePlayerSpeed);
                    if (_slider != null && _slider.value <= 1.0f)
                        _slider.value += 0.2f;
                    ShowUpgradeEffect();
                }
                break;
            case EUpgradePlayerPopupItemType.Capacity:
                {
                    GameManager.Instance.BroadcastEvent(EEventType.UpgradePlayerCapacity);
                    if (_slider != null && _slider.value <= 1.0f)
                        _slider.value += 0.2f;
                    ShowUpgradeEffect();
                }
                break;
            case EUpgradePlayerPopupItemType.Profit:
                {
                    GameManager.Instance.BroadcastEvent(EEventType.UpgradePlayerProfit);
                    if (_slider != null && _slider.value <= 1.0f)
                        _slider.value += 0.2f;
                    ShowUpgradeEffect();
                }
                break;
        }
    }

    private void ShowUpgradeEffect()
    {
        if (_upgradeEffectArrow != null)
        {
            _upgradeEffectArrow.SetActive(true);
            _upgradeEffectArrow.GetComponent<UI_PurchaseArrowEffect>().PlayEffect();
        }
    }
}