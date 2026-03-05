using System;
using BaseEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class DashboardScreenView
{
    [SerializeField]
    private RectTransform panel;

    [SerializeField]
    private RectTransform title;

    [SerializeField]
    public MagicButton startGameButton;

    [SerializeField]
    public MagicButton optionsButton;

    private CanvasGroup panelCanvasGroup;
    private CanvasGroup titleCanvasGroup;
    private CanvasGroup startButtonCanvasGroup;
    private CanvasGroup optionsButtonCanvasGroup;

    private Vector3 originalPanelAnchorPosition;
    private Vector3 originalPanelScale;

    private Vector3 originalTitlePos;

    public void Init()
    {
        originalPanelAnchorPosition = panel.anchoredPosition;
        originalPanelScale = panel.localScale;

        originalTitlePos = title.anchoredPosition;

        panelCanvasGroup = panel.GetComponent<CanvasGroup>();
        titleCanvasGroup = title.GetComponent<CanvasGroup>();

        startButtonCanvasGroup = startGameButton.GetComponent<CanvasGroup>();
        optionsButtonCanvasGroup = optionsButton.GetComponent<CanvasGroup>();
    }

    public void PrepareFadeIn()
    {
        // Panel
        panel.anchoredPosition = originalPanelAnchorPosition + new Vector3(0, -Screen.height, 0);
        panelCanvasGroup.alpha = 0;
        panel.localScale = Vector3.one * 0.8f;

        // Title
        title.anchoredPosition = originalTitlePos + new Vector3(0, 80, 0);
        titleCanvasGroup.alpha = 0;

        // Buttons
        startGameButton.transform.localScale = Vector3.zero;
        optionsButton.transform.localScale = Vector3.zero;

        startButtonCanvasGroup.alpha = 0;
        optionsButtonCanvasGroup.alpha = 0;
    }

    public async UniTask FadeInAsync()
    {
        Sequence seq = DOTween.Sequence();

        // PANEL
        seq.Append(panel.DOAnchorPos(originalPanelAnchorPosition, 0.45f).SetEase(Ease.OutCubic));

        seq.Join(panelCanvasGroup.DOFade(1f, 0.35f));

        seq.Join(panel.DOScale(originalPanelScale, 0.45f).SetEase(Ease.OutBack));

        // TITLE
        seq.Join(title.DOAnchorPos(originalTitlePos, 0.35f).SetEase(Ease.OutCubic));

        seq.Join(titleCanvasGroup.DOFade(1f, 0.3f));

        // BUTTONS (stagger)
        seq.AppendInterval(0.05f);

        seq.Append(startGameButton.transform.DOScale(1f, 0.35f).SetEase(Ease.OutBack));

        seq.Join(startButtonCanvasGroup.DOFade(1f, 0.2f));

        seq.Append(optionsButton.transform.DOScale(1f, 0.35f).SetEase(Ease.OutBack));

        seq.Join(optionsButtonCanvasGroup.DOFade(1f, 0.2f));

        await seq.AsyncWaitForCompletion();
    }
}
