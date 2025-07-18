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

public class UI_UpgradeEmployeePopupItem : UI_UpgradePopupItemBase<EUpgradeEmployeePopupItemType>
{
	protected override void OnClickPurchaseButton()
	{
		if (GameManager.Instance.Money < _money)
			return;

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
}
