using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public struct ConfirmDestroyBuildingEvent
{
    public int instanceId;
}

public class DestroyBuildingPanel : MonoBehaviour
{
    [SerializeField]
    private Image circleImage;
    private Tween fillTween = null;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        gameObject.SetActive(false);
    }

    public void FadeIn(float duration, Action onComplete)
    {
        gameObject.SetActive(true);
        circleImage.fillAmount = 0f;
        circleImage.DOFade(0.5f, 0.3f);
        fillTween = DOTween
            .To(() => circleImage.fillAmount, x => circleImage.fillAmount = x, 1f, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                onComplete?.Invoke();
                FadeOut();
            });
    }

    public void FadeOut()
    {
        fillTween?.Kill();
        circleImage
            .DOFade(0f, 0.3f)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
    }

    public void SetRectPosition(Vector3 worldPosition)
    {
        var uiPosition = Camera.main.WorldToScreenPoint(worldPosition);
        rectTransform.position = uiPosition;
    }
}
