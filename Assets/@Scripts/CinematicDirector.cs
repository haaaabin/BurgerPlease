using UnityEngine;
using System.Collections;

public class CinematicDirector : MonoBehaviour
{
    [SerializeField] private CameraController _cameraController;

    public void PlayCinematic()
    {
        StartCoroutine(CoPlayCinematicSequence());
    }

    private IEnumerator CoPlayCinematicSequence()
    {
        // 이동 및 터치 막기
        GameManager.Instance.Player.enabled = false;

        // 카메라 줌인
        _cameraController.ZoomIn();

        yield return new WaitForSeconds(1f);

        GameManager.Instance.Player.transform.rotation = Quaternion.Euler(0, -130, 0);

        GameManager.Instance.Player.PlayDance();

        yield return new WaitForSeconds(4f);

        GameManager.Instance.Player.StopDance();

        _cameraController.ZoomOut();

        yield return new WaitForSeconds(1f);

        GameManager.Instance.Player.enabled = true;

    }
}
