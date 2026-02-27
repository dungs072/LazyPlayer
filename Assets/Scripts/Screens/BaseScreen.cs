using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BaseScreen : MonoBehaviour
{
    protected ScreenData screenData;

    protected T GetScreenData<T>() where T : ScreenData
    {
        return screenData as T;
    }

    public virtual async UniTask OpenAsync(ScreenData screenData = null)
    {
        this.screenData = screenData;
        gameObject.SetActive(true);
        PrepareData();
        PrepareFadeIn();
        await FadeInAsync();
    }
    /// <summary>
    /// Prepare the screen data here, this will be called before OpenAsync, so the screen will be ready when it opens
    /// </summary>

    public virtual void PrepareData()
    {
        // prepare data here
    }
    /// <summary>
    /// Prepare the screen for fade in animation, this will be called before OpenAsync, so the screen will be ready when it opens
    /// </summary>
    public virtual void PrepareFadeIn()
    {
        // prepare animation here
    }
    /// <summary>
    /// Play the fade in animation here, this will be called after OpenAsync, so the screen will be active when it plays
    /// </summary>
    public virtual async UniTask FadeInAsync()
    {
        // play animation here
        await UniTask.NextFrame();
    }


    public async UniTask CloseAsync()
    {
        await UniTask.Yield();
        gameObject.SetActive(false);
    }
}
