using UnityEngine;

public class InputHandler
{
    private PlayerInputActions input;

    public void Awake()
    {
        input = new PlayerInputActions();
    }

    public void OnEnable()
    {
        input.Enable();
    }

    public void OnDisable()
    {
        input.Disable();
    }

    public void Update()
    {
        if (input.Player.Click.IsPressed())
        {
            Vector2 mousePos = input.Player.Pointer.ReadValue<Vector2>();
            EventBus.Publish(new MouseDragEvent { mousePosition = mousePos });
        }
    }
}
