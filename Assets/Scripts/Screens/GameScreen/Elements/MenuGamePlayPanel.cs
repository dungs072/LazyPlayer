using System;
using System.Collections.Generic;
using System.Linq;
using BaseEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
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

    [SerializeField]
    public MenuGridScroller scroller;
    private float originalWidth;
    private Vector2 openPosition;
    private float slideDistance;
    private List<MagicButtonWithIcon> tab1Buttons;
    private MagicButtonWithIcon selectedTab1Button;
    private ReadOnlyArray<MenuGridData> previousMenuGridData;

    void Awake()
    {
        originalWidth = rectTransform.rect.width;
        openPosition = rectTransform.anchoredPosition;
        slideDistance = originalWidth - tab1.rect.width;
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

        tab2ButtonPanel.OnRequestMenuGridData += HandleTab2ButtonPanelRequestMenuGridData;
        scroller.OnGridBlockItemClicked += HandleGridBlockItemClicked;
    }

    void OnDestroy()
    {
        manageButton.RemoveListener(HandleManageButtonClicked);
        shopButton.RemoveListener(HandleShopButtonClicked);
        galleryButton.RemoveListener(HandleGalleryButtonClicked);
        settingButton.RemoveListener(HandleSettingButtonClicked);
        closeButton.RemoveListener(HandleCloseButtonClicked);
        tab2ButtonPanel.OnRequestMenuGridData -= HandleTab2ButtonPanelRequestMenuGridData;
        scroller.OnGridBlockItemClicked -= HandleGridBlockItemClicked;
    }

    private void HandleTab2ButtonPanelRequestMenuGridData(ReadOnlyArray<MenuGridData> data)
    {
        previousMenuGridData = data;
        scroller.SetData(data);
    }

    private void CloseTabsMenu()
    {
        var pos = openPosition;
        pos.x += slideDistance;
        rectTransform.anchoredPosition = pos;
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
        scroller.SetData(new ReadOnlyArray<MenuGridData>());
    }

    private async UniTask OpenTabsMenuAsync()
    {
        var seq = DOTween.Sequence();
        float targetX = openPosition.x;

        seq.Append(rectTransform.DOAnchorPosX(targetX - 35, 0.28f).SetEase(Ease.OutCubic));

        seq.Append(rectTransform.DOAnchorPosX(targetX + 10, 0.12f).SetEase(Ease.InOutQuad));

        seq.Append(rectTransform.DOAnchorPosX(targetX, 0.10f).SetEase(Ease.OutQuad));

        await seq.AsyncWaitForCompletion();
    }

    private async UniTask HandleCloseButtonClicked()
    {
        GamePlugin.BlockInput(true);
        SetSelectedTab1Button(null);
        float closedX = openPosition.x + slideDistance;
        await rectTransform
            .DOAnchorPosX(closedX, 0.35f)
            .SetEase(Ease.InQuart)
            .AsyncWaitForCompletion();

        GamePlugin.BlockInput(false);
    }

    private async UniTask HandleGridBlockItemClicked(MenuGridData data)
    {
        GamePlugin.BlockInput(true);
        var type = data.Type;
        if (type == ButtonTab3Type.BUILD)
        {
            HandleClickBuildButton();
        }
        else if (type == ButtonTab3Type.BUILD_BACK)
        {
            HandleClickBuildBackButton();
        }
        else if (type == ButtonTab3Type.BUILDING_LIST)
        {
            HandleClickBuildingListButton(data.Name);
        }
        else
        {
            Debug.Log($"Clicked grid block with type {type}");
        }
        await UniTask.NextFrame();
        GamePlugin.BlockInput(false);
    }

    private void HandleClickBuildButton()
    {
        var buildingDataList = QueryBus.Query(new GetBuildingDataListQuery());
        var gridData = new MenuGridData[buildingDataList.Count + 1];
        for (int i = 0; i < buildingDataList.Count; i++)
        {
            gridData[i] = new MenuGridData
            {
                Type = ButtonTab3Type.BUILDING_LIST,
                Name = buildingDataList[i].DisplayName,
                Icon = null,
            };
        }
        gridData[buildingDataList.Count] = new MenuGridData
        {
            Type = ButtonTab3Type.BUILD_BACK,
            Name = "Back",
            Icon = null, // need to query the back icon
        };

        scroller.SetData(new ReadOnlyArray<MenuGridData>(gridData));
    }

    private void HandleClickBuildBackButton()
    {
        scroller.SetData(previousMenuGridData);
    }

    private void HandleClickBuildingListButton(string entityName)
    {
        EventBus.Publish(new SpawnEntityEvent { entityId = EntityId.ParseId(entityName) });
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
