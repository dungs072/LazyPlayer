using TMPro;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField]
    private string entityName = "Default";

    [SerializeField]
    private TMP_Text displayNameText;

    public string EntityName
    {
        get { return entityName; }
    }

    protected virtual void Awake()
    {
        if (displayNameText == null)
            return;
        displayNameText.text = entityName;
    }
}
