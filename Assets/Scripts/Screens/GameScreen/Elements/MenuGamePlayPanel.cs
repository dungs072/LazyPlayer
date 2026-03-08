using System.Collections.Generic;
using BaseEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MenuGamePlayPanel : MonoBehaviour
{
    [SerializeField]
    private RectTransform rectTransform;

    [SerializeField]
    private RectTransform tab1;

    [SerializeField]
    private RectTransform tab2;

    [SerializeField]
    private RectTransform tab3;

    [SerializeField]
    private MagicButton manageButton;

    [SerializeField]
    private MagicButton shopButton;

    [SerializeField]
    private MagicButton galleryButton;

    [SerializeField]
    private MagicButton settingButton;

    [SerializeField]
    private MagicButton[] tab2Buttons;

    [SerializeField]
    private MagicButton closeButton;
    private float originalWidth;
    private List<MagicButton> tab1Buttons;
    private MagicButton selectedTab1Button;
    private MagicButton selectedTab2Button;

    void Awake()
    {
        originalWidth = rectTransform.rect.width;
        tab1Buttons = new List<MagicButton>
        {
            manageButton,
            shopButton,
            galleryButton,
            settingButton,
        };
        SubcribeEvents();
        CloseTabsMenu();
    }

    private void SubcribeEvents()
    {
        manageButton.AddListener(HandleManageButtonClicked);
        shopButton.AddListener(HandleShopButtonClicked);
        galleryButton.AddListener(HandleGalleryButtonClicked);
        settingButton.AddListener(HandleSettingButtonClicked);
        closeButton.AddListener(HandleCloseButtonClicked);
    }

    void OnDestroy()
    {
        manageButton.RemoveListener(HandleManageButtonClicked);
        shopButton.RemoveListener(HandleShopButtonClicked);
        galleryButton.RemoveListener(HandleGalleryButtonClicked);
        settingButton.RemoveListener(HandleSettingButtonClicked);
        closeButton.RemoveListener(HandleCloseButtonClicked);
    }

    private void CloseTabsMenu()
    {
        var tab1Width = tab1.rect.width;
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tab1Width);
    }

    private async UniTask HandleManageButtonClicked()
    {
        GamePlugin.BlockInput(true);
        SetSelectedTab1Button(ref selectedTab1Button, manageButton);
        await OpenTabsMenuAsync();
        GamePlugin.BlockInput(false);
    }

    private async UniTask HandleShopButtonClicked()
    {
        GamePlugin.BlockInput(true);
        SetSelectedTab1Button(ref selectedTab1Button, shopButton);
        await OpenTabsMenuAsync();
        GamePlugin.BlockInput(false);
    }

    private async UniTask HandleGalleryButtonClicked()
    {
        GamePlugin.BlockInput(true);
        SetSelectedTab1Button(ref selectedTab1Button, galleryButton);
        await OpenTabsMenuAsync();
        GamePlugin.BlockInput(false);
    }

    private async UniTask HandleSettingButtonClicked()
    {
        GamePlugin.BlockInput(true);
        SetSelectedTab1Button(ref selectedTab1Button, settingButton);
        await OpenTabsMenuAsync();
        GamePlugin.BlockInput(false);
    }

    private void SetSelectedTab1Button(ref MagicButton selectedButton, MagicButton button)
    {
        if (selectedButton != null)
        {
            var prevImage = selectedButton.GetComponent<Image>();
            prevImage.color = Color.white;
        }
        selectedButton = button;
        if (button == null)
            return;
        var image = button.GetComponent<Image>();

        image.color = Color.gray;
    }

    private async UniTask OpenTabsMenuAsync()
    {
        var seq = DOTween.Sequence();

        float h = rectTransform.sizeDelta.y;

        seq.Append(
            rectTransform
                .DOSizeDelta(new Vector2(originalWidth + 35, h), 0.28f)
                .SetEase(Ease.OutCubic)
        );

        seq.Append(
            rectTransform
                .DOSizeDelta(new Vector2(originalWidth - 10, h), 0.12f)
                .SetEase(Ease.InOutQuad)
        );

        seq.Append(
            rectTransform.DOSizeDelta(new Vector2(originalWidth, h), 0.10f).SetEase(Ease.OutQuad)
        );

        await seq.AsyncWaitForCompletion();
    }

    private async UniTask HandleCloseButtonClicked()
    {
        GamePlugin.BlockInput(true);
        SetSelectedTab1Button(ref selectedTab1Button, null);
        await rectTransform
            .DOSizeDelta(new Vector2(tab1.rect.width, rectTransform.sizeDelta.y), 0.35f)
            .SetEase(Ease.InQuart)
            .AsyncWaitForCompletion();

        GamePlugin.BlockInput(false);
    }

    public void PrepareFadeIn()
    {
        foreach (var button in tab1Buttons)
        {
            button.transform.localScale = Vector3.zero;
        }
    }

    public async UniTask FadeInAsync()
    {
        var seq = DOTween.Sequence();

        foreach (var button in tab1Buttons)
        {
            seq.Join(button.transform.DOScale(Vector3.one, 0.35f).SetEase(Ease.OutBack));
        }

        await seq.AsyncWaitForCompletion();
    }
}
