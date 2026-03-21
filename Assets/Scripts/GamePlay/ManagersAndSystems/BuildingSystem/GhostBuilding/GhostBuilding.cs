using UnityEngine;

public class GhostBuilding : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer skin;

    private Vector3 originalScale = Vector3.one;
    private Vector3 originalPosition = Vector3.zero;

    public void SetSkin(Sprite sprite)
    {
        skin.sprite = sprite;
    }

    public void SetSkinColor(Color color)
    {
        skin.color = color;
    }

    public void SetSize(Vector2 scale)
    {
        transform.localScale = new Vector3(scale.x, scale.y, 1);
    }

    public void UpdateOriginalValues()
    {
        originalScale = transform.localScale;
        originalPosition = transform.position;
    }

    public void ResetOriginalValues()
    {
        transform.localScale = originalScale;
        transform.position = originalPosition;
    }
}
