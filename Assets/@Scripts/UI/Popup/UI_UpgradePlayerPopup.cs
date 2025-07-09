using UnityEngine;
using UnityEngine.UI;

public class UI_UpgradePlayerPopup : MonoBehaviour
{
    [SerializeField]
    Button _closeButton;

    [SerializeField]
    UI_UpgradePlayerPopupItem _speedItem;

    [SerializeField]
    UI_UpgradePlayerPopupItem _capacityItem;

    [SerializeField]
    UI_UpgradePlayerPopupItem _profitItem;

    void Start()
    {
        _closeButton.onClick.AddListener(OnClickCloseButton);

        _speedItem.SetInfo(EUpgradePlayerPopupItemType.Speed, 500);
        _capacityItem.SetInfo(EUpgradePlayerPopupItemType.Capacity, 500);
        _profitItem.SetInfo(EUpgradePlayerPopupItemType.Profit, 500);
    }

    void OnClickCloseButton()
    {
        gameObject.SetActive(false);
    }
}
