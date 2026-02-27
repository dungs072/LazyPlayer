using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Resources;
public class GameScreen : BaseScreen
{
    [SerializeField] private GameScreenView view = new();


    private void Awake()
    {
        EventBus.Subscribe<ResourceAmountChangedEvent>(view.SetResourcesAmount);
        view.preButton.AddListener(OnPreButtonClicked);
        view.nextButton.AddListener(OnNextButtonClicked);
    }
    private void OnDestroy()
    {
        EventBus.Unsubscribe<ResourceAmountChangedEvent>(view.SetResourcesAmount);
        view.preButton.RemoveListener(OnPreButtonClicked);
        view.nextButton.RemoveListener(OnNextButtonClicked);
    }
    public void OnPreButtonClicked()
    {
        if (view.currentMapIndex == 0) return;
        GamePlugin.BlockInput(true);
        EventBus.Publish(new PreButtonClickedEvent());
        view.currentMapIndex--;
        GamePlugin.BlockInput(false);
    }
    public void OnNextButtonClicked()
    {
        if (view.currentMapIndex == MapConstant.TOTAL_MAPS - 1) return;
        GamePlugin.BlockInput(true);
        EventBus.Publish(new NextButtonClickedEvent());
        view.currentMapIndex++;
        GamePlugin.BlockInput(false);
    }
}
