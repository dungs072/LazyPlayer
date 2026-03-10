using System.Collections.Generic;
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
        //! remove it when we have real data
        var data = new GameScreenData
        {
            dataList = new List<MenuGridData>()
            {
                new MenuGridData { Name = "Map 1", Icon = null },
                new MenuGridData { Name = "Map 2", Icon = null },
                new MenuGridData { Name = "Map 3", Icon = null },
                new MenuGridData { Name = "Map 4", Icon = null },
                new MenuGridData { Name = "Map 5", Icon = null },
            },
        };
        await ScreenPlugin.OpenScreenAsync<GameScreen>(data);
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
