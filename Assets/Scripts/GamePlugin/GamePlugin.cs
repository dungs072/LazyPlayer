using UnityEngine;
using System;
using UnityEngine.EventSystems;

// only pure class c# is allowed
public class GamePlugin
{
    public static void BlockInput(bool block = true)
    {
        EventSystem.current.enabled = !block;
    }
}

