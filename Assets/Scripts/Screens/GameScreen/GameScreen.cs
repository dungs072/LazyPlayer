using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public struct SelectEditingBuildingEvent
{
    public int instanceId;
    public Vector3 worldPosition;
    public Vector3 size;
}

public struct SelectDestroyingBuildingEvent
{
    public int instanceId;
    public Vector3 worldPosition;
    public float duration;
}

public struct ExitDestroyBuildingUIEvent { }

public class GameScreen : BaseScreen
{
    [SerializeField]
    private GameScreenView view = new();

    public override void Initialize1()
    {
        EventBus.Subscribe<ResourceAmountChangedEvent>(view.SetResourcesAmount);
        EventBus.SubscribeAsync<SelectEditingBuildingEvent>(view.ShowEditBuildingPanel);
        EventBus.Subscribe<SelectDestroyingBuildingEvent>(view.ShowDestroyBuildingPanel);
        EventBus.Subscribe<ExitDestroyBuildingUIEvent>(view.HideDestroyBuildingPanel);
        view.preButton.AddListener(OnPreButtonClicked);
        view.nextButton.AddListener(OnNextButtonClicked);
        view.staffButton.AddListener(OnStaffButtonClicked);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<ResourceAmountChangedEvent>(view.SetResourcesAmount);
        EventBus.UnsubscribeAsync<SelectEditingBuildingEvent>(view.ShowEditBuildingPanel);
        EventBus.Unsubscribe<SelectDestroyingBuildingEvent>(view.ShowDestroyBuildingPanel);
        EventBus.Unsubscribe<ExitDestroyBuildingUIEvent>(view.HideDestroyBuildingPanel);
        view.preButton.RemoveListener(OnPreButtonClicked);
        view.nextButton.RemoveListener(OnNextButtonClicked);
        view.staffButton.RemoveListener(OnStaffButtonClicked);
    }

    public void OnPreButtonClicked()
    {
        if (view.currentMapIndex == 0)
            return;
        GamePlugin.BlockInput(true);
        EventBus.Publish(new PreButtonClickedEvent());
        view.currentMapIndex--;
        GamePlugin.BlockInput(false);
    }

    public void OnNextButtonClicked()
    {
        if (view.currentMapIndex == MapConstant.TOTAL_MAPS - 1)
            return;
        GamePlugin.BlockInput(true);
        EventBus.Publish(new NextButtonClickedEvent());
        view.currentMapIndex++;
        GamePlugin.BlockInput(false);
    }

    public async UniTask OnStaffButtonClicked()
    {
        GamePlugin.BlockInput(true);
        var staffDataList = QueryBus.Query(new GetStaffDataListQuery());
        var data = new StaffScreenData { dataList = staffDataList };
        await ScreenPlugin.OpenScreenAsync<StaffScreen>(data);
        GamePlugin.BlockInput(false);
    }

    public async UniTask OnBuilderButtonClicked()
    {
        GamePlugin.BlockInput(true);
        // var buildingDataList = QueryBus.Query(new GetBuildingDataListQuery());
        // var data = new BuilderScreenData { dataList = buildingDataList };
        // await UniTask.WhenAll(
        //     new[]
        //     {
        //         ScreenPlugin.OpenScreenAsync<BuilderScreen>(data),
        //         ScreenPlugin.CloseScreenAsync<GameScreen>(),
        //     }
        // );
        await UniTask.NextFrame();
        GamePlugin.BlockInput(false);
    }

    public override void PrepareData()
    {
        var gridMenuData = GetScreenData<GameScreenData>();
        view.SetMenuGridData(gridMenuData.dataList);
    }

    public override void PrepareFadeIn()
    {
        view.PrepareFadeIn();
    }

    public override async UniTask FadeInAsync()
    {
        await view.FadeInAsync();
    }

    public override UniTask FadeOutAsync()
    {
        return UniTask.CompletedTask;
    }
}
