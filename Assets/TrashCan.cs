using DG.Tweening;
using UnityEditor.PackageManager;
using UnityEngine;
using static Define;

public class TrashCan : MonoBehaviour
{
    void Start()
    {
        PlayerInteraction interaction = Utils.FindChild<PlayerInteraction>(gameObject);
        interaction.InteractInterval = 0.2f;
        interaction.OnPlayerInteraction = OnPlayerTrashCanInteraction;
    }

    private void OnPlayerTrashCanInteraction(PlayerController pc)
    {
        Transform t = pc.Tray.RemoveFromTray();
        if (t == null)
            return;

        ETrayObject eTrayObject = pc.Tray.ETrayObject;

        t.DOJump(transform.position, 1.5f, 1, 0.5f)
            .OnComplete(() =>
            {
                switch (eTrayObject)
                {
                    case ETrayObject.Burger:
                        GameManager.Instance.DeSpawnBurger(t.gameObject);
                        break;
                    case ETrayObject.Trash:
                        GameManager.Instance.DeSpawnTrash(t.gameObject);
                        break;
                }
            });
    }
}
