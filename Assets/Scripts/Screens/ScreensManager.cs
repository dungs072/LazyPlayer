using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using Cysharp.Threading.Tasks;
public class ScreensManager : MonoBehaviour
{
    [SerializeField] private List<BaseScreen> screens;
    Dictionary<Type, BaseScreen> screenDictionary;
    private void Awake()
    {
        screenDictionary = new Dictionary<Type, BaseScreen>();
        foreach (var screen in screens)
        {
            screenDictionary.Add(screen.GetType(), screen);
            screen.gameObject.SetActive(false);
        }
    }
    public void AddScreen(BaseScreen screen)
    {
        if (!screenDictionary.ContainsKey(screen.GetType()))
        {
            screenDictionary.Add(screen.GetType(), screen);
            screen.gameObject.SetActive(false);
        }
    }
    public void RemoveScreen(BaseScreen screen)
    {
        if (screenDictionary.ContainsKey(screen.GetType()))
        {
            screenDictionary.Remove(screen.GetType());
        }
    }

    public async UniTask OpenScreen<T>(ScreenData screenData = null) where T : BaseScreen
    {
        if (screenDictionary.TryGetValue(typeof(T), out var screen))
        {
            await screen.OpenAsync(screenData);
        }
        else
        {
            Debug.LogError($"Screen of type {typeof(T)} not found in ScreensManager.");
        }
    }
    public async UniTask CloseScreen<T>() where T : BaseScreen
    {
        if (screenDictionary.TryGetValue(typeof(T), out var screen))
        {
            await screen.CloseAsync();
        }
        else
        {
            Debug.LogError($"Screen of type {typeof(T)} not found in ScreensManager.");
        }
    }

}
