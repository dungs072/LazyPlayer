using TMPro;
using Unity.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    private static int nextId = 1;

    [SerializeField, ReadOnly]
    private int instanceId;

    [SerializeField]
    protected SpriteRenderer spriteRenderer;

    [SerializeField]
    private TMP_Text displayNameText;
    private EntityData entityData;
    public EntityData Data => entityData;
    public int InstanceId => instanceId;

    // must call Init() after Instantiate
    public void Init()
    {
        instanceId = nextId++;
    }

    public virtual void Initialize(EntityData data)
    {
        entityData = data;
        spriteRenderer.sprite = entityData.Skin;
        displayNameText.text = entityData.DisplayName;
    }
}
