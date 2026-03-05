using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [field: SerializeField]
    public GamePlay GamePlay { get; private set; }

    [field: SerializeField]
    public ScreensManager ScreensManager { get; private set; }

    private static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        new GamePlugin().Init();
        new ScreenPlugin(ScreensManager);

        GamePlay.Initialize1();
        ScreensManager.Initialize1();
    }

    private void Start()
    {
        GamePlay.Initialize2();
        ScreenPlugin.OpenScreenAsync<DashboardScreen>().Forget();
    }
}
