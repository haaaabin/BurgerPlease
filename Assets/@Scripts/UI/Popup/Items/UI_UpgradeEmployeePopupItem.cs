using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public enum EUpgradeEmployeePopupItemType
{
	None,
	Speed,
	Capacity,
	Hire
}

public class UI_UpgradeEmployeePopupItem : MonoBehaviour
{
	// TODO : 나머지 UI 연동

	[SerializeField]
	private Button _purchaseButton;

	[SerializeField]
	private TextMeshProUGUI _costText;

	[SerializeField]
	private Slider _slider;

	[SerializeField]
	private GameObject _upgradeEffectArrow;

	EUpgradeEmployeePopupItemType _type = EUpgradeEmployeePopupItemType.None;

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

	public void SetInfo(EUpgradeEmployeePopupItemType type, long money)
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

		// 돈 소모.
		GameManager.Instance.Money -= _money;

		switch (_type)
		{
			case EUpgradeEmployeePopupItemType.Speed:
				{
					GameManager.Instance.BroadcastEvent(EEventType.UpgradeEmployeeSpeed);
					if (_slider != null && _slider.value <= 1.0f)
						_slider.value += 0.2f;
					ShowUpgradeEffect();
				}
				break;
			case EUpgradeEmployeePopupItemType.Capacity:
				{
					GameManager.Instance.BroadcastEvent(EEventType.UpgradeEmployeeCapacity);
					if (_slider != null && _slider.value <= 1.0f)
						_slider.value += 0.2f;
					ShowUpgradeEffect();
				}
				break;
			case EUpgradeEmployeePopupItemType.Hire:
				{
					GameManager.Instance.BroadcastEvent(EEventType.HireWorker);
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
