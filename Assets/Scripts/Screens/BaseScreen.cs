using System.Collections;
using UnityEngine;

public class BaseScreen : MonoBehaviour
{
    protected ScreenData screenData;

    protected T GetScreenData<T>() where T : ScreenData
    {
        return screenData as T;
    }

    public virtual IEnumerator OpenAsync(ScreenData screenData = null)
    {
        this.screenData = screenData;
        gameObject.SetActive(true);
        PrepareData();
        PrepareFadeIn();
        yield return FadeInAsync();
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
    public virtual IEnumerator FadeInAsync()
    {
        // play animation here
        yield return null;
    }


    public IEnumerator CloseAsync()
    {
        yield return null;
        gameObject.SetActive(false);
    }
}
