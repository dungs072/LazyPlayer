using UnityEngine;
using System;
using UnityEngine.EventSystems;

// only pure class c# is allowed
public class GamePlugin
{
    private static EventSystem eventSystem;
    public void Init()
    {
        eventSystem = EventSystem.current;
    }

    public static void BlockInput(bool block = true)
    {
        if (eventSystem != null)
        {
            eventSystem.enabled = !block;
        }
    }
}
