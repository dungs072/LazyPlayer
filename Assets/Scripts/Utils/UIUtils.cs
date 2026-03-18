using UnityEngine;

public static class UIUtils
{
    //? only work if the pivot is center
    public static bool IsOutOfScreen(Vector3 screenPosition, Vector3 size)
    {
        var center = screenPosition;
        var right = screenPosition + new Vector3(size.x * 0.5f, 0, 0);
        var top = screenPosition + new Vector3(0, size.y * 0.5f, 0);

        float halfWidth = Mathf.Abs(right.x - center.x);
        float halfHeight = Mathf.Abs(top.y - center.y);

        float minX = center.x - halfWidth;
        float maxX = center.x + halfWidth;
        float minY = center.y - halfHeight;
        float maxY = center.y + halfHeight;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        if (minX < 0 || maxX > screenWidth || minY < 0 || maxY > screenHeight)
        {
            return true;
        }

        return false;
    }
}
