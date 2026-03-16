using UnityEngine;

public class EditBuildingPanel : MonoBehaviour
{
    [SerializeField]
    private MagicButtonWithIcon moveButton;

    [SerializeField]
    private MagicButtonWithIcon rotateButton;

    [SerializeField]
    private MagicButtonWithIcon cancelButton;

    void Awake() { }

    void OnDestroy() { }
}
