using UnityEngine;

public class GeneralUtils
{
    public static Vector3 GetMouseWorldPosition(Vector2 uiPosition)
    {
        var mainCamera = Camera.main;
        Vector3 mouseScreenPosition = uiPosition;
        mouseScreenPosition.z = mainCamera.nearClipPlane;
        return mainCamera.ScreenToWorldPoint(mouseScreenPosition);
    }
}
