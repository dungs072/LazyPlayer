using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public struct BuildBuildingEvent
{
    public EntityId entityId;
}

public struct EditBuildingEvent { }

public enum BuildingSystemState
{
    EDIT,
    BUILD,
    NONE,
}

public class BuildingSystem : MonoBehaviour
{
    [SerializeField]
    private GhostBuilding ghostBuilding;

    private EntityManager entityManager = null;
    private GridSystem gridSystem = null;
    private EntityData buildingData = null;
    private bool isOverlapping = false;
    private Tween _moveTween = null;
    private BuildingSystemState currentState = BuildingSystemState.NONE;

    public void Initialize1(EntityManager entityManager, GridSystem gridSystem)
    {
        this.entityManager = entityManager;
        this.gridSystem = gridSystem;

        EventBus.Subscribe<BuildBuildingEvent>(StartBuildBuilding);
        EventBus.Subscribe<EditBuildingEvent>(StartEditBuilding);
        InputHandler.OnMouseLeftClick += HandleClick;
        InputHandler.OnMouseRightClick += DestroyBuilding;

        ghostBuilding.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        EventBus.Unsubscribe<BuildBuildingEvent>(StartBuildBuilding);
        InputHandler.OnMouseLeftClick -= HandleClick;
        InputHandler.OnMouseRightClick -= DestroyBuilding;
    }

    private void StartBuildBuilding(BuildBuildingEvent e)
    {
        var centerWorldPos = QueryBus.Query(new GetCenterCameraPositionQuery());
        buildingData = entityManager.GetEntityData(e.entityId);
        var buildingPrefab = buildingData.EntityPrefab;
        if (buildingPrefab == null)
        {
            Debug.LogError($"Cannot find prefab for entity ID: {e.entityId}");
            return;
        }
        currentState = BuildingSystemState.BUILD;
        var buildableEntityPrefab = buildingPrefab as BuildableEntity;
        ghostBuilding.SetSkin(buildingData.Skin);
        ghostBuilding.transform.position = centerWorldPos;
        ghostBuilding.transform.localScale = new Vector2(
            buildableEntityPrefab.DisplaySize.x,
            buildableEntityPrefab.DisplaySize.y
        );
        ghostBuilding.gameObject.SetActive(true);
        ghostBuilding.SetSkinColor(Color.green);

        MoveGhostBuilding();
    }

    private void StartEditBuilding(EditBuildingEvent e)
    {
        currentState = BuildingSystemState.EDIT;
    }

    private void HandleClick()
    {
        if (currentState == BuildingSystemState.BUILD)
        {
            EndBuildBuilding();
        }
        else if (currentState == BuildingSystemState.EDIT)
        {
            SelectBuildingToEdit();
        }
    }

    private void SelectBuildingToEdit()
    {
        var mousePosition = Mouse.current.position.ReadValue();
        var worldPos = GeneralUtils.GetMouseWorldPosition(mousePosition);
        var entityInstanceId = gridSystem.GetEntityIdAtPosition(worldPos);
        var entityInstance = entityManager.GetInstantiatedEntity(entityInstanceId);
        if (entityInstance == null)
        {
            Debug.LogError($"Cannot find entity instance with ID: {entityInstanceId}");
            return;
        }
        if (entityInstance is BuildableEntity buildableEntity)
        {
            buildingData = buildableEntity.Data;
            ghostBuilding.SetSkin(buildingData.Skin);
            ghostBuilding.transform.position = buildableEntity.transform.position;
            ghostBuilding.SetSize(buildableEntity.DisplaySize);
            ghostBuilding.gameObject.SetActive(true);
            ghostBuilding.SetSkinColor(Color.green);
            buildableEntity.gameObject.SetActive(false);
            MoveGhostBuilding();
        }
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
                if (!ghostBuilding.gameObject.activeSelf)
                    return;
                var mousePosition = Mouse.current.position.ReadValue();
                if (mousePosition == lastMousePosition)
                    return;
                lastMousePosition = mousePosition;
                var worldPos = GeneralUtils.GetMouseWorldPosition(mousePosition);
                isOverlapping = gridSystem.IsCellOccupied(worldPos, buildingSize);
                if (isOverlapping)
                {
                    ghostBuilding.SetSkinColor(Color.red);
                }
                else
                {
                    ghostBuilding.SetSkinColor(Color.green);
                }
                var snapGridPos = gridSystem.GetSnapGridPosition(worldPos);
                ghostBuilding.transform.position = snapGridPos;
                GamePlugin.BlockInput(true);
            });
    }

    private void EndBuildBuilding()
    {
        // If no building is currently being placed, ignore the click event
        if (!ghostBuilding.gameObject.activeSelf)
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
        var position = ghostBuilding.transform.position;
        var entityId = EntityId.ParseId(buildingData.EntityId);
        var buildingEntity = entityManager.Acquire<BuildableEntity>(
            entityId,
            new EntityConfig { position = position }
        );
        var currentBuildableEntity = buildingEntity;
        currentBuildableEntity.SetBuildingState(BuildingState.READY);
        gridSystem.SetCellsOccupied(position, buildingEntity.Size, buildingEntity.GetInstanceID());
        KillMoveTween();
        HideGhostBuilding();
        currentState = BuildingSystemState.NONE;
    }

    private void DestroyBuilding()
    {
        KillMoveTween();
        HideGhostBuilding();
        currentState = BuildingSystemState.NONE;
    }

    private void KillMoveTween()
    {
        _moveTween?.Kill();
        _moveTween = null;
        GamePlugin.BlockInput(false);
    }

    private void HideGhostBuilding()
    {
        if (ghostBuilding.gameObject.activeSelf)
        {
            ghostBuilding.SetSkin(null);
            ghostBuilding.gameObject.SetActive(false);
        }
    }
}
