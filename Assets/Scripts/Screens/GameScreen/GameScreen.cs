using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
public class GameScreen : BaseScreen
{
    [SerializeField] private GameScreenView view = new();


    private void Awake()
    {
        EventBus.Subscribe<ResourceAmountChangedEvent>(view.SetResourcesAmount);
        view.preButton.AddListener(OnPreButtonClicked);
        view.nextButton.AddListener(OnNextButtonClicked);
        view.staffButton.AddListener(OnStaffButtonClicked);
        view.builderButton.AddListener(OnBuilderButtonClicked);
    }
    private void OnDestroy()
    {
        EventBus.Unsubscribe<ResourceAmountChangedEvent>(view.SetResourcesAmount);
        view.preButton.RemoveListener(OnPreButtonClicked);
        view.nextButton.RemoveListener(OnNextButtonClicked);
        view.staffButton.RemoveListener(OnStaffButtonClicked);
        view.builderButton.RemoveListener(OnBuilderButtonClicked);
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
    public async UniTask OnStaffButtonClicked()
    {
        GamePlugin.BlockInput(true);
        var staffDataList = QueryBus.Query<GetStaffDataListQuery, IReadOnlyList<CharacterData>>(new GetStaffDataListQuery());
        var data = new StaffScreenData { dataList = staffDataList };
        await ScreenPlugin.OpenScreenAsync<StaffScreen>(data);
        GamePlugin.BlockInput(false);
    }
    public async UniTask OnBuilderButtonClicked()
    {
        GamePlugin.BlockInput(true);
        var buildingDataList = QueryBus.Query<GetBuildingDataListQuery, IReadOnlyList<BuildableEntity>>(new GetBuildingDataListQuery());
        var data = new BuilderScreenData { dataList = buildingDataList };
        await UniTask.WhenAll(new[] {
            ScreenPlugin.OpenScreenAsync<BuilderScreen>(data),
            ScreenPlugin.CloseScreenAsync<GameScreen>()
        });
        GamePlugin.BlockInput(false);
    }
}
