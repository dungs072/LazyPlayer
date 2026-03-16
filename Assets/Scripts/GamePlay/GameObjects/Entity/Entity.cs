using TMPro;
using Unity.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private int instanceId;

    [SerializeField]
    protected SpriteRenderer spriteRenderer;

    [SerializeField]
    private TMP_Text displayNameText;
    private EntityData entityData;
    public EntityData Data => entityData;
    public int InstanceId => instanceId;

    protected virtual void Awake()
    {
        instanceId = GenerateInstanceId();
    }

    private int GenerateInstanceId()
    {
        return GetHashCode();
    }

    public virtual void Initialize(EntityData data)
    {
        entityData = data;
        spriteRenderer.sprite = entityData.Skin;
        displayNameText.text = entityData.DisplayName;
    }
}
