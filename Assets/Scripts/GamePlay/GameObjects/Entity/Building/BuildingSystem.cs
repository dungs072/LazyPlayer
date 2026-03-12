using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingSystem : MonoBehaviour
{
    private BuildableEntity currentBuildableEntity;
    private Tween _moveTween;

    public void Initialize1()
    {
        EventBus.Subscribe<SpawnEntityEvent>(StartBuilding);
        InputHandler.OnMouseLeftClick += EndBuilding;
        InputHandler.OnMouseRightClick += DestroyBuilding;
    }

    void OnDestroy()
    {
        EventBus.Unsubscribe<SpawnEntityEvent>(StartBuilding);
        InputHandler.OnMouseLeftClick -= EndBuilding;
        InputHandler.OnMouseRightClick -= DestroyBuilding;
    }

    public void StartBuilding(SpawnEntityEvent e)
    {
        var centerWorldPos = QueryBus.Query(new GetCenterCameraPositionQuery());
        var building = QueryBus.Query(
            new GetEntityQuery { prefabId = e.entityName, position = centerWorldPos }
        );
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
        GamePlugin.BlockInput(true);
        MoveBuilding();
    }

    private void MoveBuilding()
    {
        _moveTween?.Kill();
        var lastMousePosition = Mouse.current.position.ReadValue();
        _moveTween = DOTween
            .To(() => 0f, _ => { }, 0f, 1f)
            .SetLoops(-1)
            .OnUpdate(() =>
            {
                if (!currentBuildableEntity)
                    return;
                var mousePosition = Mouse.current.position.ReadValue();
                if (mousePosition == lastMousePosition)
                    return;
                lastMousePosition = mousePosition;
                var worldPos = GeneralUtils.GetMouseWorldPosition(mousePosition);
                var snapGridPos = QueryBus.Query(
                    new GetSnapGridPositionQuery { position = worldPos }
                );
                currentBuildableEntity.Move(snapGridPos);
            });
    }

    private void EndBuilding()
    {
        // If no building is currently being placed, ignore the click event
        if (!currentBuildableEntity)
        {
            //Debug.LogWarning("No building is currently being placed. Ignoring build command.");
            return;
        }
        currentBuildableEntity.SetBuildingState(BuildingState.READY);
        KillMoveTween();
        currentBuildableEntity = null;
    }

    private void DestroyBuilding()
    {
        KillMoveTween();
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

    private void KillMoveTween()
    {
        _moveTween?.Kill();
        _moveTween = null;
        GamePlugin.BlockInput(false);
    }
}
