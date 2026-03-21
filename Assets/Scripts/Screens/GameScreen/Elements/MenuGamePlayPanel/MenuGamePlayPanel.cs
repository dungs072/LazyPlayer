using System;
using System.Collections.Generic;
using System.Linq;
using BaseEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

public enum ButtonTab1Type
{
    MANAGEMENT,
    SHOP,
    GALLERY,
    SETTING,
}

[Serializable]
public class ButtonTab2Panel
{
    public ButtonTab1Type parentTab1Type;
    public GameObject panel;
}

public class MenuGamePlayPanel : MonoBehaviour
{
    [SerializeField]
    private RectTransform tab1;

    [SerializeField]
    private List<ButtonTab2Panel> tab2ButtonPanels;

    [Header("Tab 1")]
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

    [Header("Tab 2")]
    [SerializeField]
    private MagicButtonWithIcon staffButton;

    [SerializeField]
    private MagicButtonWithIcon inventoryButton;

    [SerializeField]
    private MagicButtonWithIcon buildButton;

    [Header("Tab 3")]
    [SerializeField]
    public MenuGridScroller scroller;

    //Runtime state
    private float originalWidth = 0;
    private Vector2 openPosition = Vector2.zero;
    private float slideDistance = 0;

    //UI data
    private List<MagicButtonWithIcon> tab1Buttons = null;
    private ReadOnlyArray<MenuGridData> previousMenuGridData = null;
    private MagicButtonWithIcon previousTab1Button = null;
    private MagicButtonWithIcon previousTab2Button = null;

    //Component references
    private CanvasGroup canvasGroup = null;
    private RectTransform rectTransform = null;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
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
        canvasGroup = GetComponent<CanvasGroup>();

        SubcribeEvents();
        CloseTabsMenu();
    }

    private void CloseTabsMenu()
    {
        var pos = openPosition;
        pos.x += slideDistance;
        rectTransform.anchoredPosition = pos;
    }

    private void SubcribeEvents()
    {
        EventBus.Subscribe<CancelSelectEvent>(HandleCancelEdit);
        //tab1 buttons
        manageButton.AddListener(HandleManageButtonClicked);
        shopButton.AddListener(HandleShopButtonClicked);
        galleryButton.AddListener(HandleGalleryButtonClicked);
        settingButton.AddListener(HandleSettingButtonClicked);
        //tab2 buttons
        closeButton.AddListener(HandleCloseButtonClicked);
        buildButton.AddListener(HandleBuildButtonClicked);
        //tab3
        scroller.OnGridBlockItemClicked += HandleGridBlockItemClicked;
    }

    void OnDestroy()
    {
        EventBus.Unsubscribe<CancelSelectEvent>(HandleCancelEdit);
        //tab1 buttons
        manageButton.RemoveListener(HandleManageButtonClicked);
        shopButton.RemoveListener(HandleShopButtonClicked);
        galleryButton.RemoveListener(HandleGalleryButtonClicked);
        settingButton.RemoveListener(HandleSettingButtonClicked);
        //tab2 buttons
        closeButton.RemoveListener(HandleCloseButtonClicked);
        buildButton.RemoveListener(HandleBuildButtonClicked);
        //tab3
        scroller.OnGridBlockItemClicked -= HandleGridBlockItemClicked;
    }

    #region Animations

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
    #endregion

    #region Tab 1 Button Handlers

    private async UniTask HandleManageButtonClicked()
    {
        GamePlugin.BlockInput(true);
        SwitchTab2Panel(ButtonTab1Type.MANAGEMENT);
        SetSelectedTab1Button(manageButton);
        await OpenTabsMenuAsync();
        GamePlugin.BlockInput(false);
    }

    private async UniTask HandleShopButtonClicked()
    {
        GamePlugin.BlockInput(true);
        SwitchTab2Panel(ButtonTab1Type.SHOP);
        SetSelectedTab1Button(shopButton);
        await OpenTabsMenuAsync();
        GamePlugin.BlockInput(false);
    }

    private async UniTask HandleGalleryButtonClicked()
    {
        GamePlugin.BlockInput(true);
        SwitchTab2Panel(ButtonTab1Type.GALLERY);
        SetSelectedTab1Button(galleryButton);
        await OpenTabsMenuAsync();
        GamePlugin.BlockInput(false);
    }

    private async UniTask HandleSettingButtonClicked()
    {
        GamePlugin.BlockInput(true);
        SwitchTab2Panel(ButtonTab1Type.SETTING);
        SetSelectedTab1Button(settingButton);
        await OpenTabsMenuAsync();
        GamePlugin.BlockInput(false);
    }

    private void SwitchTab2Panel(ButtonTab1Type tab1Type)
    {
        foreach (var panel in tab2ButtonPanels)
        {
            panel.panel.SetActive(panel.parentTab1Type == tab1Type);
        }
        SetSelectedTab2Button(null);
    }

    private void SetSelectedTab1Button(MagicButtonWithIcon button)
    {
        if (previousTab1Button != null)
        {
            var prevImage = previousTab1Button.GetComponent<Image>();
            prevImage.color = Color.white;
        }
        previousTab1Button = button;
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

    #endregion

    #region Tab 2 Button Handlers
    private async UniTask HandleCloseButtonClicked()
    {
        GamePlugin.BlockInput(true);
        float closedX = openPosition.x + slideDistance;
        await rectTransform
            .DOAnchorPosX(closedX, 0.35f)
            .SetEase(Ease.InQuart)
            .AsyncWaitForCompletion();
        SetSelectedTab1Button(null);
        GamePlugin.BlockInput(false);
    }

    private async UniTask HandleBuildButtonClicked()
    {
        GamePlugin.BlockInput(true);
        var gridData = new MenuGridData[3]
        {
            new MenuGridData
            {
                Type = MenuGridType.BUILD,
                Name = "Build",
                Icon = null, //! need to query the build icon
            },
            new MenuGridData
            {
                Type = MenuGridType.EDIT,
                Name = "Edit",
                Icon = null, //! need to query the edit icon
            },
            new MenuGridData
            {
                Type = MenuGridType.DESTROY,
                Name = "Destroy",
                Icon = null, //! need to query the destroy icon
            },
        };
        var data = new ReadOnlyArray<MenuGridData>(gridData);
        scroller.SetData(data);
        SavePreviousScrollerData(data);
        SetSelectedTab2Button(buildButton);
        await UniTask.NextFrame();
        GamePlugin.BlockInput(false);
    }

    private void SavePreviousScrollerData(ReadOnlyArray<MenuGridData> data)
    {
        previousMenuGridData = data;
        scroller.SetData(data);
    }

    private void SetSelectedTab2Button(MagicButtonWithIcon button)
    {
        if (previousTab2Button != null)
        {
            var prevImage = previousTab2Button.GetComponent<Image>();
            prevImage.color = Color.white;
        }
        previousTab2Button = button;
        if (button == null)
            return;
        var image = button.GetComponent<Image>();
        image.color = Color.gray;
    }
    #endregion

    #region Tab 3 Button Handlers

    private async UniTask HandleGridBlockItemClicked(MenuGridData data)
    {
        GamePlugin.BlockInput(true);
        var type = data.Type;
        if (type == MenuGridType.BUILD)
        {
            HandleClickBuildButton();
        }
        else if (type == MenuGridType.EDIT)
        {
            HandleClickEditButton();
        }
        else if (type == MenuGridType.DESTROY)
        {
            HandleClickDestroyButton();
        }
        else if (type == MenuGridType.BUILD_BACK)
        {
            HandleClickBuildBackButton();
        }
        else if (type == MenuGridType.BUILDING_LIST)
        {
            HandleClickBuildingListButton(data.Name);
        }
        else
        {
            Debug.LogWarning($"No handler for clicking grid block with type {type}");
        }
        await UniTask.NextFrame();
        GamePlugin.BlockInput(false);
    }

    private void HandleClickBuildButton()
    {
        GamePlugin.BlockInput(true);
        var buildingDataList = QueryBus.Query(new GetBuildingDataListQuery());
        var gridData = new MenuGridData[buildingDataList.Count + 1];
        for (int i = 0; i < buildingDataList.Count; i++)
        {
            gridData[i] = new MenuGridData
            {
                Type = MenuGridType.BUILDING_LIST,
                Name = buildingDataList[i].DisplayName,
                Icon = null, //! need to query the building icon
            };
        }
        gridData[buildingDataList.Count] = new MenuGridData
        {
            Type = MenuGridType.BUILD_BACK,
            Name = "Back",
            Icon = null, // need to query the back icon
        };

        scroller.SetData(new ReadOnlyArray<MenuGridData>(gridData));
        GamePlugin.BlockInput(false);
    }

    private void HandleClickEditButton()
    {
        canvasGroup.blocksRaycasts = false;
        EventBus.Publish(new EditBuildingEvent());
    }

    private void HandleClickDestroyButton()
    {
        GamePlugin.BlockInput(true);
        canvasGroup.blocksRaycasts = false;
        EventBus.Publish(new DestroyBuildingEvent());
        GamePlugin.BlockInput(false);
    }

    private void HandleClickBuildBackButton()
    {
        scroller.SetData(previousMenuGridData);
    }

    private void HandleClickBuildingListButton(string entityName)
    {
        EventBus.Publish(new BuildBuildingEvent { entityId = EntityId.ParseId(entityName) });
    }

    private void HandleCancelEdit(CancelSelectEvent e)
    {
        canvasGroup.blocksRaycasts = true;
    }

    public void UnblockInput()
    {
        canvasGroup.blocksRaycasts = true;
    }
    #endregion
}
