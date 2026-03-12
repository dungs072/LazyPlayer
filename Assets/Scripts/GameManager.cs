using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [field: SerializeField]
    public GamePlay GamePlay { get; private set; }

    [field: SerializeField]
    public ScreensManager ScreensManager { get; private set; }

    private void Awake()
    {
        new GamePlugin().Init();
        new ScreenPlugin(ScreensManager);

        GamePlay.Initialize1();
        ScreensManager.Initialize1();
    }

    private void Start()
    {
        GamePlay.Initialize2();
        var data = new GameScreenData { dataList = new List<MenuGridData>() { } };
        ScreenPlugin.OpenScreenAsync<GameScreen>(data).Forget();
    }
}
