using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [field: SerializeField] public GamePlay GamePlay { get; private set; }
    [field: SerializeField] public ScreensManager ScreensManager { get; private set; }


    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }
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
    }
    private void Start()
    {
        ScreenPlugin.OpenScreenAsync<GameScreen>().Forget();
    }
}
