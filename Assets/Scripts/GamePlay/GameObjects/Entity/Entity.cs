using TMPro;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField]
    protected SpriteRenderer spriteRenderer;

    [SerializeField]
    private TMP_Text displayNameText;
    private EntityData entityData;
    public EntityData Data => entityData;

    public virtual void Initialize(EntityData data)
    {
        entityData = data;
        spriteRenderer.sprite = entityData.Skin;
        displayNameText.text = entityData.DisplayName;
    }
}
