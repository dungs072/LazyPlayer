using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BuildingEditorScreen : BaseScreen
{
    [SerializeField] private BuildingEditorScreenView view;

    private void Awake()
    {
        view.closeButton.AddListener(HandleCloseButtonClicked);
        view.rotateButton.AddListener(HandleRotateButtonClicked);
        view.buildButton.AddListener(HandleBuildButtonClicked);
    }

    void OnDestroy()
    {
        view.closeButton.RemoveListener(HandleCloseButtonClicked);
        view.rotateButton.RemoveListener(HandleRotateButtonClicked);
        view.buildButton.RemoveListener(HandleBuildButtonClicked);
    }
    private async UniTask HandleCloseButtonClicked()
    {
        GamePlugin.BlockInput(true);
        EventBus.Publish(new DestroyCurrentBuildingEvent());
        var buildingDataList = QueryBus.Query<GetBuildingDataListQuery, IReadOnlyList<BuildableEntity>>(new GetBuildingDataListQuery());

        await UniTask.WhenAll(new[] {
            ScreenPlugin.OpenScreenAsync<BuilderScreen>(new BuilderScreenData { dataList = buildingDataList }),
            ScreenPlugin.CloseScreenAsync<BuildingEditorScreen>()
        });
        GamePlugin.BlockInput(false);
    }
    private async UniTask HandleRotateButtonClicked()
    {
        GamePlugin.BlockInput(true);
        await UniTask.NextFrame();
        GamePlugin.BlockInput(false);
    }
    private async UniTask HandleBuildButtonClicked()
    {
        GamePlugin.BlockInput(true);
        EventBus.Publish(new BuildCurrentBuildingEvent());
        await UniTask.WhenAll(new[]
        {
            ScreenPlugin.OpenScreenAsync<BuilderScreen>(),
            ScreenPlugin.CloseScreenAsync<BuildingEditorScreen>()
        });
        GamePlugin.BlockInput(false);
    }
}
