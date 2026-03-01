using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class BuilderScreen : BaseScreen
{
    [SerializeField] private BuilderScreenView view;

    void Awake()
    {
        view.closeButton.AddListener(HandleCloseButtonClicked);
    }
    void OnDestroy()
    {
        view.closeButton.RemoveListener(HandleCloseButtonClicked);
    }
    public override void PrepareData()
    {
        var data = GetScreenData<BuilderScreenData>();
        if (data == null)
        {
            Debug.LogError("BuilderScreen: No data found");
            return;
        }
        view.scroller.SetData(data.dataList);
    }
    private async UniTask HandleCloseButtonClicked()
    {
        GamePlugin.BlockInput(true);
        await UniTask.WhenAll(new[] {
            ScreenPlugin.OpenScreenAsync<GameScreen>(),
            ScreenPlugin.CloseScreenAsync<BuilderScreen>()
        });
        GamePlugin.BlockInput(false);
    }
}
