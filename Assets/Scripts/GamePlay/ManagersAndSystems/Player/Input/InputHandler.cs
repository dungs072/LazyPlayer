using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

//? Only handle input from player can have raw events

public class InputHandler
{
    public static event Action OnMouseLeftClick;
    public static event Action OnMouseRightClick;
    private PlayerInputActions input;

    public void Awake()
    {
        input = new PlayerInputActions();
        input.Player.LeftClick.performed += ctx => HandleLeftClick();
        input.Player.RightClick.performed += ctx => HandleRightClick();
    }

    public void OnDestroy()
    {
        input.Player.LeftClick.performed -= ctx => HandleLeftClick();
        input.Player.RightClick.performed -= ctx => HandleRightClick();
    }

    public void OnEnable()
    {
        input.Enable();
    }

    public void OnDisable()
    {
        input.Disable();
    }

    private void HandleLeftClick()
    {
        OnMouseLeftClick?.Invoke();
    }

    private void HandleRightClick()
    {
        OnMouseRightClick?.Invoke();
    }
}
