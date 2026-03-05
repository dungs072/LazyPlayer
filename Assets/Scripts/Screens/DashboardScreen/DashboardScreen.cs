using Cysharp.Threading.Tasks;
using UnityEngine;

public class DashboardScreen : BaseScreen
{
    [SerializeField]
    private DashboardScreenView view;

    void Awake()
    {
        view.startGameButton.AddListener(HandleStartGameButtonClicked);
        view.optionsButton.AddListener(HandleOptionsButtonClicked);
        view.Init();
    }

    private async UniTask HandleStartGameButtonClicked()
    {
        await FadeOutAsync();
        await ScreenPlugin.CloseScreenAsync<DashboardScreen>();
        await ScreenPlugin.OpenScreenAsync<GameScreen>();
    }

    private async UniTask HandleOptionsButtonClicked()
    {
        await UniTask.Yield();
    }

    public override void PrepareData()
    {
        // nothing
    }

    public override void PrepareFadeIn()
    {
        view.PrepareFadeIn();
    }

    public override async UniTask FadeInAsync()
    {
        await view.FadeInAsync();
    }

    public override async UniTask FadeOutAsync()
    {
        await view.FadeOutAsync();
    }
}
