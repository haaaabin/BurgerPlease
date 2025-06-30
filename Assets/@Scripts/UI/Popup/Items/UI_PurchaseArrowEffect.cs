using DG.Tweening;
using UnityEngine;

public class UI_PurchaseArrowEffect : MonoBehaviour
{
    public float _moveUpAmount = 100f;
    public float _duration = 0.8f;

    private RectTransform _rect;
    private CanvasGroup _canvasGroup;

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void PlayEffect()
    {
        _rect.anchoredPosition = Vector3.zero;
        _canvasGroup.alpha = 1f;

        _rect.DOAnchorPosY(_moveUpAmount, _duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }
}