using UnityEngine;

[RequireComponent(typeof(WorkerInteraction))]
public class Office : MonoBehaviour
{
    private void Start()
    {
        GetComponent<WorkerInteraction>().OnTriggerStart = OnEnterOffice;
        GetComponent<WorkerInteraction>().OnTriggerEnd = OnLeaveOffice;
    }

    public void OnEnterOffice(WorkerController wc)
    {
        GameManager.Instance.UpgradeEmployeePopup.gameObject.SetActive(true);
    }

    
    public void OnLeaveOffice(WorkerController wc)
    {
        GameManager.Instance.UpgradeEmployeePopup.gameObject.SetActive(false);
    }
}