using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingSystem : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer ghostBuildingSkin;

    private BuildableEntity buildablePrefab;
    private bool isOverlapping = false;
    private Tween _moveTween;

    public void Initialize1()
    {
        EventBus.Subscribe<SpawnEntityEvent>(StartGhostBuilding);
        InputHandler.OnMouseLeftClick += EndGhostBuilding;
        InputHandler.OnMouseRightClick += DestroyGhostBuilding;
        ghostBuildingSkin.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        EventBus.Unsubscribe<SpawnEntityEvent>(StartGhostBuilding);
        InputHandler.OnMouseLeftClick -= EndGhostBuilding;
        InputHandler.OnMouseRightClick -= DestroyGhostBuilding;
    }

    public void StartGhostBuilding(SpawnEntityEvent e)
    {
        var centerWorldPos = QueryBus.Query(new GetCenterCameraPositionQuery());
        var buildingPrefab = QueryBus.Query(new GetEntityPrefabQuery { prefabId = e.entityName });
        if (buildingPrefab == null)
        {
            Debug.LogError($"Cannot find prefab for entity name: {e.entityName}");
            return;
        }
        var currentBuildableEntity = buildingPrefab as BuildableEntity;
        if (currentBuildableEntity == null)
        {
            Debug.LogError($"The entity {e.entityName} is not a BuildableEntity.");
            return;
        }
        ghostBuildingSkin.sprite = currentBuildableEntity.Skin;
        ghostBuildingSkin.transform.position = centerWorldPos;
        ghostBuildingSkin.transform.localScale = new Vector2(
            currentBuildableEntity.DisplaySize.x,
            currentBuildableEntity.DisplaySize.y
        );
        ghostBuildingSkin.gameObject.SetActive(true);
        ghostBuildingSkin.color = Color.green;
        buildablePrefab = currentBuildableEntity;

        MoveGhostBuilding();
    }

    private void MoveGhostBuilding()
    {
        _moveTween?.Kill();
        var lastMousePosition = Mouse.current.position.ReadValue();
        _moveTween = DOTween
            .To(() => 0f, _ => { }, 0f, 1f)
            .SetLoops(-1)
            .OnUpdate(() =>
            {
                if (!ghostBuildingSkin.sprite)
                    return;
                var mousePosition = Mouse.current.position.ReadValue();
                if (mousePosition == lastMousePosition)
                    return;
                lastMousePosition = mousePosition;
                var worldPos = GeneralUtils.GetMouseWorldPosition(mousePosition);
                isOverlapping = QueryBus.Query(
                    new IsOverlappingGridQuery { position = worldPos, size = buildablePrefab.Size }
                );
                if (isOverlapping)
                {
                    ghostBuildingSkin.color = Color.red;
                }
                else
                {
                    ghostBuildingSkin.color = Color.green;
                }
                var snapGridPos = QueryBus.Query(
                    new GetSnapGridPositionQuery { position = worldPos }
                );
                ghostBuildingSkin.transform.position = snapGridPos;
                GamePlugin.BlockInput(true);
            });
    }

    private void EndGhostBuilding()
    {
        // If no building is currently being placed, ignore the click event
        if (!ghostBuildingSkin.gameObject.activeSelf)
        {
            //Debug.LogWarning("No building is currently being placed. Ignoring build command.");
            return;
        }
        if (isOverlapping)
        {
            return;
        }
        if (GamePlugin.IsPointerOverUI())
        {
            return;
        }
        var position = ghostBuildingSkin.transform.position;
        var buildingEntity = QueryBus.Query(
            new GetEntityQuery { prefabId = buildablePrefab.EntityName, position = position }
        );
        var currentBuildableEntity = buildingEntity as BuildableEntity;
        currentBuildableEntity.SetBuildingState(BuildingState.READY);
        EventBus.Publish(
            new SetOccupiedGridEvent { position = position, size = buildablePrefab.Size }
        );
        KillMoveTween();
        HideGhostBuilding();
    }

    private void DestroyGhostBuilding()
    {
        KillMoveTween();
        HideGhostBuilding();
    }

    private void KillMoveTween()
    {
        _moveTween?.Kill();
        _moveTween = null;
        GamePlugin.BlockInput(false);
    }

    private void HideGhostBuilding()
    {
        if (ghostBuildingSkin.gameObject.activeSelf)
        {
            ghostBuildingSkin.sprite = null;
            ghostBuildingSkin.gameObject.SetActive(false);
        }
    }
}
