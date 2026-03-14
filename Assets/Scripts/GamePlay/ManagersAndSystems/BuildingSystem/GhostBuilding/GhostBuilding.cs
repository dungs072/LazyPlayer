using UnityEngine;

public class GhostBuilding : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer skin;

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
}
