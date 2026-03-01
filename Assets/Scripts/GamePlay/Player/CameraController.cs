using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera mainCamera;
    private void Start()
    {
        mainCamera = Camera.main;
        EventBus.Subscribe<PreButtonClickedEvent>(MovePreviousMap);
        EventBus.Subscribe<NextButtonClickedEvent>(MoveNextMap);
        QueryBus.Subscribe<GetCenterCameraPositionQuery, Vector3>(query => GetWorldCenterCameraPosition());
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe<PreButtonClickedEvent>(MovePreviousMap);
        EventBus.Unsubscribe<NextButtonClickedEvent>(MoveNextMap);
    }

    public void MoveNextMap(NextButtonClickedEvent e)
    {
        mainCamera.transform.position += new Vector3(MapConstant.MAP_WIDTH, 0, 0);
    }

    public void MovePreviousMap(PreButtonClickedEvent e)
    {
        mainCamera.transform.position += new Vector3(-MapConstant.MAP_WIDTH, 0, 0);
    }
    public Vector3 GetWorldCenterCameraPosition()
    {
        Vector3 center = new Vector3(0.5f, 0.5f, mainCamera.nearClipPlane);
        return mainCamera.ViewportToWorldPoint(center);
    }
}
