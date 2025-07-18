using UnityEngine;
using static Define;

public enum EUpgradePlayerPopupItemType
{
    None,
    Speed,
    Capacity,
    Profit
}

public class UI_UpgradePlayerPopupItem : UI_UpgradePopupItemBase<EUpgradePlayerPopupItemType>
{
    protected override void OnClickPurchaseButton()
    {
        if (GameManager.Instance.Money < _money)
            return;

        GameManager.Instance.Money -= _money;

        switch (_type)
        {
            case EUpgradePlayerPopupItemType.Speed:
                GameManager.Instance.BroadcastEvent(EEventType.UpgradePlayerSpeed);
                if (_slider != null && _slider.value <= 1.0f)
                    _slider.value += 0.2f;
                ShowUpgradeEffect();
                break;
            case EUpgradePlayerPopupItemType.Capacity:
                GameManager.Instance.BroadcastEvent(EEventType.UpgradePlayerCapacity);
                if (_slider != null && _slider.value <= 1.0f)
                    _slider.value += 0.2f;
                ShowUpgradeEffect();
                break;
            case EUpgradePlayerPopupItemType.Profit:
                GameManager.Instance.BroadcastEvent(EEventType.UpgradePlayerProfit);
                if (_slider != null && _slider.value <= 1.0f)
                    _slider.value += 0.2f;
                ShowUpgradeEffect();
                break;
        }
    }
}