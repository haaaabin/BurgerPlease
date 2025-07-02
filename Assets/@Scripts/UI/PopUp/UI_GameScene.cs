using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_GameScene : MonoBehaviour
{
	[SerializeField]
	TextMeshProUGUI _moneyCountText;

	[SerializeField]
	TextMeshProUGUI _toastMessageText;

	[SerializeField]
	Slider _expSlider;

	[SerializeField]
	TextMeshProUGUI _levelText;

	private void OnEnable()
	{
		RefreshUI();
		RefreshExpUI();
		GameManager.Instance.AddEventListener(EEventType.MoneyChanged, RefreshUI);
		GameManager.Instance.AddEventListener(EEventType.ExpChanged, RefreshExpUI);

	}

	private void OnDisable()
	{
		GameManager.Instance.RemoveEventListener(EEventType.MoneyChanged, RefreshUI);
		GameManager.Instance.RemoveEventListener(EEventType.ExpChanged, RefreshExpUI);
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

	public void RefreshExpUI()
	{
		_expSlider.value = GameManager.Instance.CurrentExp / GameManager.Instance.GetMaxExp();
		_levelText.text = GameManager.Instance.Level.ToString();
	}
}
