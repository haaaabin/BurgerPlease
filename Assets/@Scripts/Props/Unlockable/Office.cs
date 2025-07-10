using UnityEngine;
using static Define;

public enum EOfficeType
{
	HROffice,
	UpgradeOffice
}

[RequireComponent(typeof(WorkerInteraction))]
public class Office : UnlockableBase
{
	[SerializeField]
	private EOfficeType officeType = EOfficeType.UpgradeOffice;

	private void Start()
	{
		GetComponent<WorkerInteraction>().OnTriggerStart = OnEnterOffice;
		GetComponent<WorkerInteraction>().OnTriggerEnd = OnLeaveOffice;
	}

	public void OnEnterOffice(WorkerController wc)
	{
		if (!wc.Tray.IsPlayer)
			return;

		switch (officeType)
		{
			case EOfficeType.UpgradeOffice:
				GameManager.Instance.UpgradePlayerPopup.gameObject.SetActive(true);
				break;
			case EOfficeType.HROffice:
				GameManager.Instance.UpgradeEmployeePopup.gameObject.SetActive(true);
				break;
		}
	}

	public void OnLeaveOffice(WorkerController wc)
	{
		if (!wc.Tray.IsPlayer)
			return;

		switch (officeType)
		{
			case EOfficeType.UpgradeOffice:
				GameManager.Instance.UpgradePlayerPopup.gameObject.SetActive(false);
				break;
			case EOfficeType.HROffice:
				GameManager.Instance.UpgradeEmployeePopup.gameObject.SetActive(false);
				break;
		}
	}
}
