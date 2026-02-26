using UnityEngine;
using TMPro;
public class Entity : MonoBehaviour
{
    [SerializeField] private string entityName = "Default";
    [SerializeField] private TMP_Text displayNameText;

    public string EntityName { get { return entityName; } }

    protected virtual void Awake()
    {
        displayNameText.text = entityName;
    }
}
