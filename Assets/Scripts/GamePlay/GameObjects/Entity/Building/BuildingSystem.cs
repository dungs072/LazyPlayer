using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public struct SpawnEntityEvent
{
    public EntityId entityId;
}

public class BuildingSystem : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer ghostBuildingSkin;

    private EntityData buildingData;
    private EntityManager entityManager = null;
    private bool isOverlapping = false;
    private Tween _moveTween;

    public void Initialize1(EntityManager entityManager)
    {
        this.entityManager = entityManager;
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
        buildingData = entityManager.GetEntityData(e.entityId);
        var buildingPrefab = buildingData.EntityPrefab;
        if (buildingPrefab == null)
        {
            Debug.LogError($"Cannot find prefab for entity ID: {e.entityId}");
            return;
        }
        var buildableEntityPrefab = buildingPrefab as BuildableEntity;
        ghostBuildingSkin.sprite = buildingData.Skin;
        ghostBuildingSkin.transform.position = centerWorldPos;
        ghostBuildingSkin.transform.localScale = new Vector2(
            buildableEntityPrefab.DisplaySize.x,
            buildableEntityPrefab.DisplaySize.y
        );
        ghostBuildingSkin.gameObject.SetActive(true);
        ghostBuildingSkin.color = Color.green;

        MoveGhostBuilding();
    }

    private void MoveGhostBuilding()
    {
        _moveTween?.Kill();
        var lastMousePosition = Mouse.current.position.ReadValue();
        var buildingPrefab = buildingData.EntityPrefab as BuildableEntity;
        var skin = buildingData.Skin;
        var buildingSize = new Vector2(
            skin.bounds.size.x * buildingPrefab.DisplaySize.x,
            skin.bounds.size.y * buildingPrefab.DisplaySize.y
        );
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
                    new IsOverlappingGridQuery { position = worldPos, size = buildingSize }
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
        var entityId = EntityId.ParseId(buildingData.EntityId);
        var buildingEntity = entityManager.Acquire<BuildableEntity>(
            entityId,
            new EntityConfig { position = position }
        );
        var currentBuildableEntity = buildingEntity;
        currentBuildableEntity.SetBuildingState(BuildingState.READY);
        EventBus.Publish(
            new SetOccupiedGridEvent { position = position, size = buildingEntity.Size }
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
