using TMPro;
using Unity.Collections;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private int instanceId;

    [SerializeField]
    private string entityName = "Default";

    [SerializeField]
    private TMP_Text displayNameText;
    public int InstanceId
    {
        get { return instanceId; }
    }
    public string EntityName
    {
        get { return entityName; }
    }

    protected virtual void Awake()
    {
        instanceId = GenerateInstanceId();
        if (displayNameText == null)
            return;
        displayNameText.text = entityName;
    }

    private int GenerateInstanceId()
    {
        return GetHashCode();
    }
}
