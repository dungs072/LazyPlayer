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
    private RectTransform mask;

    [SerializeField]
    public BaseEngine.MagicButtonWithIcon startGameButton;

    [SerializeField]
    public BaseEngine.MagicButtonWithIcon optionsButton;

    private CanvasGroup panelCanvasGroup;
    private CanvasGroup titleCanvasGroup;
    private CanvasGroup startButtonCanvasGroup;
    private CanvasGroup optionsButtonCanvasGroup;
    private CanvasGroup maskCanvasGroup;
    private RectTransform startButtonRect;
    private RectTransform optionsButtonRect;

    private Vector2 originalPanelAnchorPosition;
    private Vector3 originalPanelScale;

    private Vector2 originalTitlePos;
    private Vector3 originalTitleScale;

    private Vector2 originalStartButtonPos;
    private Vector2 originalOptionsButtonPos;

    public void Init()
    {
        panelCanvasGroup = panel.GetComponent<CanvasGroup>();
        titleCanvasGroup = title.GetComponent<CanvasGroup>();
        startButtonCanvasGroup = startGameButton.GetComponent<CanvasGroup>();
        optionsButtonCanvasGroup = optionsButton.GetComponent<CanvasGroup>();
        maskCanvasGroup = mask.GetComponent<CanvasGroup>();
        startButtonRect = startGameButton.GetComponent<RectTransform>();
        optionsButtonRect = optionsButton.GetComponent<RectTransform>();

        originalPanelAnchorPosition = panel.anchoredPosition;

        originalPanelScale = panel.localScale;

        originalTitlePos = title.anchoredPosition;
        originalTitleScale = title.localScale;

        originalStartButtonPos = startButtonRect.anchoredPosition;
        originalOptionsButtonPos = optionsButtonRect.anchoredPosition;
    }

    public void PrepareFadeIn()
    {
        // Mask
        maskCanvasGroup.alpha = 0f;

        // Panel
        panel.anchoredPosition = originalPanelAnchorPosition + new Vector2(0, -Screen.height);
        panelCanvasGroup.alpha = 0;
        panel.localScale = Vector3.one * 0.8f;

        // Title
        title.localScale = originalTitleScale * 0.6f;
        titleCanvasGroup.alpha = 0;
        title.anchoredPosition = originalTitlePos;

        // Buttons
        startGameButton.transform.localScale = Vector3.zero;
        optionsButton.transform.localScale = Vector3.zero;

        startButtonCanvasGroup.alpha = 0;
        optionsButtonCanvasGroup.alpha = 0;

        startButtonRect.anchoredPosition = originalStartButtonPos;
        optionsButtonRect.anchoredPosition = originalOptionsButtonPos;
    }

    public async UniTask FadeInAsync()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(maskCanvasGroup.DOFade(1f, 0.4f));
        // PANEL
        seq.Append(panel.DOAnchorPos(originalPanelAnchorPosition, 0.45f).SetEase(Ease.OutCubic));
        seq.Join(panelCanvasGroup.DOFade(1f, 0.35f));
        seq.Join(panel.DOScale(originalPanelScale, 0.45f).SetEase(Ease.OutBack));

        // TITLE
        seq.Append(title.DOScale(1.45f, 0.25f).SetEase(Ease.OutCubic));
        seq.Join(titleCanvasGroup.DOFade(1f, 0.25f));
        seq.Append(title.DOScale(0.9f, 0.08f));
        seq.Append(title.DOScale(1.05f, 0.08f));
        seq.Append(title.DOScale(0.98f, 0.06f));
        seq.Append(title.DOScale(1f, 0.05f));

        // BUTTONS (stagger)
        seq.AppendInterval(0.05f);

        seq.Append(startGameButton.transform.DOScale(1f, 0.35f).SetEase(Ease.OutBack));

        seq.Join(startButtonCanvasGroup.DOFade(1f, 0.2f));

        seq.Append(optionsButton.transform.DOScale(1f, 0.35f).SetEase(Ease.OutBack));

        seq.Join(optionsButtonCanvasGroup.DOFade(1f, 0.2f));

        await seq.AsyncWaitForCompletion();
    }

    public async UniTask FadeOutAsync()
    {
        Sequence seq = DOTween.Sequence();

        float dropDistance = Screen.height * 2.5f;

        var startRect = startGameButton.GetComponent<RectTransform>();
        var optionsRect = optionsButton.GetComponent<RectTransform>();

        // BUTTONS
        seq.Append(
            startRect
                .DOAnchorPos(originalStartButtonPos + Vector2.down * dropDistance, 0.35f)
                .SetEase(Ease.InBack)
        );

        seq.Join(startButtonCanvasGroup.DOFade(0f, 0.35f));

        seq.Join(
            optionsRect
                .DOAnchorPos(originalOptionsButtonPos + Vector2.down * dropDistance, 0.35f)
                .SetEase(Ease.InBack)
        );

        seq.Join(optionsButtonCanvasGroup.DOFade(0f, 0.35f));

        // TITLE
        seq.Append(
            title
                .DOAnchorPos(title.anchoredPosition + Vector2.down * dropDistance * 1.1f, 0.4f)
                .SetEase(Ease.InBack)
        );

        seq.Join(titleCanvasGroup.DOFade(0f, 0.35f));

        // PANEL
        seq.Append(
            panel
                .DOAnchorPos(originalPanelAnchorPosition + Vector2.down * dropDistance * 1.3f, 0.5f)
                .SetEase(Ease.InBack)
        );

        seq.Join(panelCanvasGroup.DOFade(0f, 0.5f));
        seq.Join(maskCanvasGroup.DOFade(0f, 0.5f));

        await seq.AsyncWaitForCompletion();
    }
}
