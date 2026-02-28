using Cysharp.Threading.Tasks;
public class ScreenPlugin
{
    private static ScreensManager screensManager;
    public ScreenPlugin(ScreensManager screensManager)
    {
        ScreenPlugin.screensManager = screensManager;
    }
    public static async UniTask OpenScreenAsync<T>(ScreenData data = null) where T : BaseScreen
    {
        await screensManager.OpenScreenAsync<T>(data);
    }
    public static async UniTask CloseScreenAsync<T>() where T : BaseScreen
    {
        await screensManager.CloseScreenAsync<T>();
    }
}