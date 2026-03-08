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
    private MagicButtonWithIcon manageButton;

    [SerializeField]
    private MagicButtonWithIcon shopButton;

    [SerializeField]
    private MagicButtonWithIcon galleryButton;

    [SerializeField]
    private MagicButtonWithIcon settingButton;

    [SerializeField]
    private MagicButtonWithIcon closeButton;

    [SerializeField]
    private Tab2ButtonsPanel tab2ButtonPanel;
    private float originalWidth;
    private List<MagicButtonWithIcon> tab1Buttons;
    private MagicButtonWithIcon selectedTab1Button;

    void Awake()
    {
        originalWidth = rectTransform.rect.width;
        tab1Buttons = new List<MagicButtonWithIcon>
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
        SetSelectedTab1Button(manageButton);
        tab2ButtonPanel.SetTab2Buttons(ButtonTab1Type.MANAGEMENT);
        await OpenTabsMenuAsync();
        GamePlugin.BlockInput(false);
    }

    private async UniTask HandleShopButtonClicked()
    {
        GamePlugin.BlockInput(true);
        SetSelectedTab1Button(shopButton);
        tab2ButtonPanel.SetTab2Buttons(ButtonTab1Type.SHOP);
        await OpenTabsMenuAsync();
        GamePlugin.BlockInput(false);
    }

    private async UniTask HandleGalleryButtonClicked()
    {
        GamePlugin.BlockInput(true);
        SetSelectedTab1Button(galleryButton);
        tab2ButtonPanel.SetTab2Buttons(ButtonTab1Type.GALLERY);
        await OpenTabsMenuAsync();
        GamePlugin.BlockInput(false);
    }

    private async UniTask HandleSettingButtonClicked()
    {
        GamePlugin.BlockInput(true);
        SetSelectedTab1Button(settingButton);
        tab2ButtonPanel.SetTab2Buttons(ButtonTab1Type.SETTING);
        await OpenTabsMenuAsync();
        GamePlugin.BlockInput(false);
    }

    private void SetSelectedTab1Button(MagicButtonWithIcon button)
    {
        if (selectedTab1Button != null)
        {
            var prevImage = selectedTab1Button.GetComponent<Image>();
            prevImage.color = Color.white;
        }
        selectedTab1Button = button;
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
        SetSelectedTab1Button(null);
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
