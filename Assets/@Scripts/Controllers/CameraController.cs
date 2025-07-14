using Unity.Cinemachine;
using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public CinemachineCamera _cam;
    public float _zoomSpeed = 2f; // 줌 속도
    public float _targetSize = 6f; // 목표 줌 크기

    private float _originalSize;

    void Start()
    {
        _originalSize = _cam.Lens.OrthographicSize;
    }

    public void ZoomIn()
    {
        StopAllCoroutines();
        StartCoroutine(ZoomInCoroutine());
    }

    private IEnumerator ZoomInCoroutine()
    {
        while (Mathf.Abs(_cam.Lens.OrthographicSize - _targetSize) > 0.01f)
        {
            _cam.Lens.OrthographicSize = Mathf.Lerp(
                _cam.Lens.OrthographicSize,
                _targetSize,
                Time.deltaTime * _zoomSpeed
            );
            yield return null;
        }
        _cam.Lens.OrthographicSize = _targetSize;
    }

    public void ZoomOut()
    {
        StopAllCoroutines();
        StartCoroutine(ZoomOutCoroutine());
    }

    private IEnumerator ZoomOutCoroutine()
    {
        while (Mathf.Abs(_cam.Lens.OrthographicSize - _originalSize) > 0.01f)
        {
            _cam.Lens.OrthographicSize = Mathf.Lerp(
                _cam.Lens.OrthographicSize,
                _originalSize,
                Time.deltaTime * _zoomSpeed
            );
            yield return null;
        }
        _cam.Lens.OrthographicSize = _originalSize;
    }
}
