using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera mainCamera;
    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void MoveNextMap()
    {
        mainCamera.transform.position += new Vector3(MapConstant.MAP_WIDTH, 0, 0);
    }

    public void MovePreviousMap()
    {
        mainCamera.transform.position += new Vector3(-MapConstant.MAP_WIDTH, 0, 0);
    }
}
