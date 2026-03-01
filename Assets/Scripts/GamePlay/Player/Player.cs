using UnityEngine;

public class Player : MonoBehaviour
{
    private InputHandler inputHandler = new();

    void Awake()
    {
        inputHandler.Awake();
    }

    void OnEnable()
    {
        inputHandler.OnEnable();
    }

    void OnDisable()
    {
        inputHandler.OnDisable();
    }
    void Update()
    {
        inputHandler.Update();
    }
}
