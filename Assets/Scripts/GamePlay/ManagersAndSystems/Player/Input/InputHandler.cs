using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

//? Only handle input from player can have raw events

public class InputHandler
{
    public static event Action OnMouseLeftClick;
    public static event Action OnMouseCancelled;
    public static event Action OnMouseRightClick;
    private PlayerInputActions input;
    private Action<InputAction.CallbackContext> onLeftClickPerformed;
    private Action<InputAction.CallbackContext> onLeftClickCanceled;
    private Action<InputAction.CallbackContext> onRightClickPerformed;

    public void Awake()
    {
        input = new PlayerInputActions();
        onLeftClickPerformed = ctx => HandleLeftClick();
        onLeftClickCanceled = ctx => HandleLeftClickCancelled();
        onRightClickPerformed = ctx => HandleRightClick();
        input.Player.LeftClick.performed += onLeftClickPerformed;
        input.Player.LeftClick.canceled += onLeftClickCanceled;
        input.Player.RightClick.performed += onRightClickPerformed;
    }

    public void OnDestroy()
    {
        input.Player.LeftClick.performed -= onLeftClickPerformed;
        input.Player.LeftClick.canceled -= onLeftClickCanceled;
        input.Player.RightClick.performed -= onRightClickPerformed;
        input.Dispose();
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

    private void HandleLeftClickCancelled()
    {
        OnMouseCancelled?.Invoke();
    }

    private void HandleRightClick()
    {
        OnMouseRightClick?.Invoke();
    }
}
