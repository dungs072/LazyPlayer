using System;
using System.Collections.Generic;
using BaseEngine;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class GameScreenView
{
    [SerializeField]
    private MenuGamePlayPanel menuGamePlayPanel;

    [SerializeField]
    public TMP_Text wheatAmount;

    [SerializeField]
    public TMP_Text breadAmount;

    [SerializeField]
    public TMP_Text moneyAmount;

    [SerializeField]
    public BaseEngine.MagicButtonWithIcon preButton;

    [SerializeField]
    public BaseEngine.MagicButtonWithIcon nextButton;

    [SerializeField]
    public BaseEngine.MagicButtonWithIcon staffButton;

    [SerializeField]
    private MenuGamePlayPanel menuPanel;

    [SerializeField]
    public EditBuildingPanel editBuildingPanel;

    [SerializeField]
    public DestroyBuildingPanel destroyBuildingPanel;

    [HideInInspector]
    public int currentMapIndex = 0;

    public void SetResourcesAmount(ResourceAmountChangedEvent e)
    {
        var resourceId = e.id;
        var amount = e.amount;
        if (resourceId == InventoryItemId.WHEAT)
        {
            wheatAmount.text = amount.ToString();
        }
        if (resourceId == InventoryItemId.BREAD)
        {
            breadAmount.text = amount.ToString();
        }
        if (resourceId == InventoryItemId.MONEY)
        {
            moneyAmount.text = amount.ToString();
        }
    }

    public void PrepareFadeIn()
    {
        menuGamePlayPanel.PrepareFadeIn();
    }

    public async UniTask FadeInAsync()
    {
        await menuGamePlayPanel.FadeInAsync();
    }

    public void SetMenuGridData(IReadOnlyList<MenuGridData> data)
    {
        menuGamePlayPanel.scroller.SetData(data);
    }

    public async UniTask ShowEditBuildingPanel(SelectEditingBuildingEvent e)
    {
        editBuildingPanel.gameObject.SetActive(true);
        editBuildingPanel.SetRectPosition(e.worldPosition, e.size);
        editBuildingPanel.SetCurrentInstanceId(e.instanceId);
        await editBuildingPanel.FadeInAsync();
    }

    public void ShowDestroyBuildingPanel(SelectDestroyingBuildingEvent e)
    {
        destroyBuildingPanel.gameObject.SetActive(true);
        destroyBuildingPanel.SetRectPosition(e.worldPosition);
        destroyBuildingPanel.FadeIn(
            e.duration,
            () =>
            {
                EventBus.Publish(new ConfirmDestroyBuildingEvent { instanceId = e.instanceId });
            }
        );
    }

    public void HideDestroyBuildingPanel(ExitDestroyBuildingUIEvent e)
    {
        destroyBuildingPanel.FadeOut();
        menuGamePlayPanel.UnblockInput();
    }
}
