using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

//TODO: consider block input for UI by using block raycast component
// only pure class c# is allowed
public class GamePlugin
{
    private static EventSystem eventSystem;
    private static readonly System.Collections.Generic.List<RaycastResult> raycastResults = new();

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

    /// <summary>
    /// Do not run function every frame, only run when player click, so it won't cause performance issue. It can be used to check if player click on UI or not.
    /// </summary>
    public static bool IsPointerOverUI()
    {
        var eventData = new PointerEventData(eventSystem)
        {
            position = Mouse.current.position.ReadValue(),
        };
        raycastResults.Clear();
        int uiLayer = LayerMask.NameToLayer(LayerConstant.UI);
        eventSystem.RaycastAll(eventData, raycastResults);
        foreach (var result in raycastResults)
        {
            // Debug.Log(
            //     $"Raycast hit: {result.gameObject.transform.parent.name}, layer: {result.gameObject.layer}"
            // );
            if (result.gameObject.layer == uiLayer)
                return true;
            if (result.gameObject.GetComponentInParent<Selectable>() != null)
                return true;
            if (result.gameObject.GetComponent<Image>() != null)
                return true;
        }
        return false;
    }
}
