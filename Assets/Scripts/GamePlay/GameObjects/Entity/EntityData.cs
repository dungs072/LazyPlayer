using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// EntityId is file name
/// </summary>
[CreateAssetMenu(fileName = "New Entity Data", menuName = "Entity Data", order = 51)]
public class EntityData : ScriptableObject
{
    [SerializeField]
    private string displayName;

    [SerializeField]
    private Sprite skin;

    [SerializeField]
    private Entity entityPrefab;
    public string EntityId => name;
    public string DisplayName => displayName;
    public Sprite Skin => skin;
    public Entity EntityPrefab => entityPrefab;
}
