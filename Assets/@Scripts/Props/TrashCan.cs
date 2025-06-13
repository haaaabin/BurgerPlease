using DG.Tweening;
using UnityEditor.PackageManager;
using UnityEngine;
using static Define;

public class TrashCan : MonoBehaviour
{
    void Start()
    {
        WorkerInteraction interaction = Utils.FindChild<WorkerInteraction>(gameObject);
        interaction.InteractInterval = 0.2f;
        interaction.OnInteraction = OnWorkerInteraction;
    }

    private void OnWorkerInteraction(WorkerController wc)
    {
        EObjectType eTrayObject = wc.Tray.CurrentTrayObjectType;

        Transform t = wc.Tray.RemoveFromTray();
        if (t == null)
            return;

        t.DOJump(transform.position, 1.5f, 1, 0.5f)
            .OnComplete(() =>
            {
                switch (eTrayObject)
                {
                    case EObjectType.Burger:
                        GameManager.Instance.DeSpawnBurger(t.gameObject);
                        break;
                    case EObjectType.Trash:
                        GameManager.Instance.DeSpawnTrash(t.gameObject);
                        break;
                }
            });
    }
}
