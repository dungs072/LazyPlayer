using UnityEngine;

public class EditBuildingPanel : MonoBehaviour
{
    [SerializeField]
    private MagicButtonWithIcon moveButton;

    [SerializeField]
    private MagicButtonWithIcon rotateButton;

    [SerializeField]
    private MagicButtonWithIcon cancelButton;

    private int selectedEntityInstanceId = -1;

    void Awake()
    {
        moveButton.AddListener(HandleMoveButtonClick);
        rotateButton.AddListener(HandleRotateButtonClick);
        cancelButton.AddListener(HandleCancelButtonClick);
    }

    void OnDestroy()
    {
        moveButton.RemoveListener(HandleMoveButtonClick);
        rotateButton.RemoveListener(HandleRotateButtonClick);
        cancelButton.RemoveListener(HandleCancelButtonClick);
    }

    public void SetCurrentInstanceId(int instanceId)
    {
        selectedEntityInstanceId = instanceId;
    }

    private void HandleMoveButtonClick()
    {
        EventBus.Publish(new MoveSelectBuildingEvent { instanceId = selectedEntityInstanceId });
        gameObject.SetActive(false);
    }

    private void HandleRotateButtonClick()
    {
        EventBus.Publish(new RotateSelectBuildingEvent { instanceId = selectedEntityInstanceId });
    }

    private void HandleCancelButtonClick()
    {
        EventBus.Publish(new CancelSelectEvent());
        gameObject.SetActive(false);
    }
}
