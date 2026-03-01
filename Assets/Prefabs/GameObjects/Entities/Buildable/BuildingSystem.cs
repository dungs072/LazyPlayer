using System.Collections.Generic;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    private BuildableEntity currentBuildableEntity;
    void Awake()
    {
        EventBus.Subscribe<SpawnEntityEvent>(Build);
        EventBus.Subscribe<DestroyCurrentBuildingEvent>(DestroyCurrentBuilding);
        EventBus.Subscribe<MouseDragEvent>(HandleMouseDrag);
        EventBus.Subscribe<BuildCurrentBuildingEvent>(HandleBuildCurrentBuilding);
    }
    void OnDestroy()
    {
        EventBus.Unsubscribe<SpawnEntityEvent>(Build);
        EventBus.Unsubscribe<DestroyCurrentBuildingEvent>(DestroyCurrentBuilding);
        EventBus.Unsubscribe<MouseDragEvent>(HandleMouseDrag);
        EventBus.Unsubscribe<BuildCurrentBuildingEvent>(HandleBuildCurrentBuilding);
    }
    public void Build(SpawnEntityEvent e)
    {
        var centerWorldPos = QueryBus.Query<GetCenterCameraPositionQuery, Vector3>(new GetCenterCameraPositionQuery());
        var building = QueryBus.Query<GetEntityQuery, Entity>(new GetEntityQuery { entityName = e.entityName, position = centerWorldPos });
        if (building == null)
        {
            Debug.LogError($"Failed to build {e.entityName} at {centerWorldPos}");
        }
        currentBuildableEntity = building as BuildableEntity;
        if (currentBuildableEntity)
        {
            currentBuildableEntity.SetBuildingState(BuildingState.UNDER_CONSTRUCTION);
        }
        else
        {
            Debug.LogError($"The entity {e.entityName} is not a BuildableEntity.");
        }
    }

    private void DestroyCurrentBuilding(DestroyCurrentBuildingEvent e)
    {
        if (currentBuildableEntity != null)
        {
            currentBuildableEntity.gameObject.SetActive(false);
            currentBuildableEntity = null;
        }
        else
        {
            Debug.LogWarning("No building to destroy.");
        }
    }
    private void HandleMouseDrag(MouseDragEvent e)
    {
        if (!currentBuildableEntity)
        {
            // Debug.LogWarning("No building is currently being placed. Ignoring mouse drag.");
            return;
        }
        var worldPos = GeneralUtils.GetMouseWorldPosition(e.mousePosition);
        var snapGridPos = QueryBus.Query<GetSnapGridPositionQuery, Vector3>(new GetSnapGridPositionQuery { position = worldPos });
        currentBuildableEntity.Move(snapGridPos);

    }
    private void HandleBuildCurrentBuilding(BuildCurrentBuildingEvent e)
    {
        if (!currentBuildableEntity)
        {
            Debug.LogWarning("No building is currently being placed. Ignoring build command.");
            return;
        }
        currentBuildableEntity.SetBuildingState(BuildingState.READY);
        currentBuildableEntity = null;
    }
}
