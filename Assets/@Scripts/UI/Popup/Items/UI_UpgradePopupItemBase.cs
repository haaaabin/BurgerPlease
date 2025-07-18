using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class UI_UpgradePopupItemBase<T> : MonoBehaviour
{
    [SerializeField]
    protected Button _purchaseButton;

    [SerializeField]
    protected TextMeshProUGUI _costText;

    [SerializeField]
    protected Slider _slider;

    [SerializeField]
    protected GameObject _upgradeEffectArrow;

    protected T _type;
    protected long _money = 0;

    protected virtual void Start()
    {
        _purchaseButton.onClick.AddListener(OnClickPurchaseButton);
    }

    protected virtual void Update()
    {
        if (GameManager.Instance.Money < _money)
            _purchaseButton.interactable = false;
        else
            _purchaseButton.interactable = true;
    }

    public virtual void SetInfo(T type, long money, int upgradeLevel)
    {
        _type = type;
        _money = money;
        RefreshUI();
        if (_slider != null)
        {
            if (upgradeLevel == 0)
                _slider.value = 0f;
            else
                _slider.value = Mathf.Clamp01(upgradeLevel * 0.2f);
        }
    }

    public virtual void RefreshUI()
    {
        _costText.text = Utils.GetMoneyText(_money);
    }

    protected abstract void OnClickPurchaseButton();

    protected void ShowUpgradeEffect()
    {
        if (_upgradeEffectArrow != null)
        {
            _upgradeEffectArrow.SetActive(true);
            _upgradeEffectArrow.GetComponent<UI_PurchaseArrowEffect>().PlayEffect();
        }
    }
}