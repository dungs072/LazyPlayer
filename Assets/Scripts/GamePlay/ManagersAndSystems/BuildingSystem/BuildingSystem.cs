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

public struct MoveSelectBuildingEvent
{
    public int instanceId;
}

public struct RotateSelectBuildingEvent
{
    public int instanceId;
}

public struct CancelSelectEvent { }

public enum BuildingSystemState
{
    EDIT,
    BUILD,
    MOVE,
    NONE,
}

public class BuildingSystem : MonoBehaviour
{
    [SerializeField]
    private GhostBuilding ghostBuilding;

    private EntityManager entityManager = null;
    private GridSystem gridSystem = null;

    // for build mode only
    private EntityData buildingData = null;

    //for edit, move mode only
    private int selectedInstanceId = -1;

    // general
    private bool isOverlapping = false;
    private Tween moveTween = null;
    private Tween snapTween = null;
    private Vector3 lastSnappedPosition = Vector3.zero;
    private BuildingSystemState currentState = BuildingSystemState.NONE;

    public void Initialize1(EntityManager entityManager, GridSystem gridSystem)
    {
        this.entityManager = entityManager;
        this.gridSystem = gridSystem;

        EventBus.Subscribe<BuildBuildingEvent>(StartBuildBuilding);
        EventBus.Subscribe<EditBuildingEvent>(StartEditBuilding);
        EventBus.Subscribe<MoveSelectBuildingEvent>(StartMoveBuilding);
        EventBus.Subscribe<RotateSelectBuildingEvent>(StartRotateBuilding);
        EventBus.Subscribe<CancelSelectEvent>(CancelSelectBuilding);
        InputHandler.OnMouseLeftClick += HandleLeftMouseClick;
        InputHandler.OnMouseRightClick += HandleRightMouseClick;

        ghostBuilding.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        EventBus.Unsubscribe<BuildBuildingEvent>(StartBuildBuilding);
        EventBus.Unsubscribe<EditBuildingEvent>(StartEditBuilding);
        EventBus.Unsubscribe<MoveSelectBuildingEvent>(StartMoveBuilding);
        EventBus.Unsubscribe<RotateSelectBuildingEvent>(StartRotateBuilding);
        EventBus.Unsubscribe<CancelSelectEvent>(CancelSelectBuilding);
        InputHandler.OnMouseLeftClick -= HandleLeftMouseClick;
        InputHandler.OnMouseRightClick -= HandleRightMouseClick;
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

    private void StartMoveBuilding(MoveSelectBuildingEvent e)
    {
        selectedInstanceId = e.instanceId;
        var entityInstance = entityManager.GetInstantiatedEntity(selectedInstanceId);
        if (entityInstance == null)
        {
            Debug.LogError($"Cannot find entity instance with ID: {selectedInstanceId}");
            return;
        }
        if (entityInstance is BuildableEntity buildableEntity)
        {
            currentState = BuildingSystemState.MOVE;
            buildingData = buildableEntity.Data;
            ghostBuilding.SetSkin(buildingData.Skin);
            ghostBuilding.transform.position = buildableEntity.transform.position;
            ghostBuilding.transform.rotation = buildableEntity.transform.rotation;
            ghostBuilding.SetSize(buildableEntity.DisplaySize);
            ghostBuilding.gameObject.SetActive(true);
            ghostBuilding.SetSkinColor(Color.green);
            buildableEntity.gameObject.SetActive(false);
            MoveGhostBuilding(selectedInstanceId);
        }
    }

    private void StartRotateBuilding(RotateSelectBuildingEvent e)
    {
        var entityInstance = entityManager.GetInstantiatedEntity(e.instanceId);
        if (entityInstance == null)
        {
            Debug.LogError($"Cannot find entity instance with ID: {e.instanceId}");
            return;
        }

        if (entityInstance is BuildableEntity buildableEntity)
        {
            var currentRotation = buildableEntity.transform.rotation.eulerAngles.z;
            var newRotation = currentRotation + 90f;
            buildableEntity.transform.DOKill();
            gridSystem.ResetCellsOccupied(e.instanceId);
            buildableEntity
                .transform.DORotate(new Vector3(0, 0, newRotation), 0.1f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    gridSystem.SetCellsOccupied(
                        buildableEntity.transform.position,
                        buildableEntity.Size,
                        buildableEntity.InstanceId
                    );
                });
        }
    }

    private void CancelSelectBuilding(CancelSelectEvent e)
    {
        currentState = BuildingSystemState.NONE;
    }

    private void HandleLeftMouseClick()
    {
        if (GamePlugin.IsPointerOverUI())
        {
            return;
        }
        if (currentState == BuildingSystemState.BUILD)
        {
            EndBuildBuilding();
        }
        else if (currentState == BuildingSystemState.EDIT)
        {
            SelectBuildingToEdit();
        }
        else if (currentState == BuildingSystemState.MOVE)
        {
            EndMoveBuilding();
        }
    }

    private void SelectBuildingToEdit()
    {
        var mousePosition = Mouse.current.position.ReadValue();
        var worldPos = GeneralUtils.GetMouseWorldPosition(mousePosition);
        var entityInstanceId = gridSystem.GetEntityIdAtPosition(worldPos);
        if (entityInstanceId == 0)
            return;
        var entityInstance = entityManager.GetInstantiatedEntity(entityInstanceId);
        if (entityInstance is BuildableEntity buildableEntity)
        {
            var entityPosition = buildableEntity.transform.position;
            var size = buildableEntity.Size;
            _ = EventBus.PublishAsync(
                new SelectEditingBuildingEvent
                {
                    instanceId = entityInstanceId,
                    worldPosition = entityPosition,
                    size = size,
                }
            );
        }
    }

    private void EndMoveBuilding()
    {
        var entityInstance = entityManager.GetInstantiatedEntity(selectedInstanceId);
        if (entityInstance == null)
        {
            Debug.LogError($"Cannot find entity instance with ID: {selectedInstanceId}");
            return;
        }
        if (entityInstance is BuildableEntity buildableEntity)
        {
            if (isOverlapping)
            {
                return;
            }
            var originalScale = ghostBuilding.transform.localScale;
            var t = ghostBuilding.transform;
            var punchTween = DOTween
                .Sequence()
                .Append(t.DOScale(originalScale * 1.08f, 0.06f).SetEase(Ease.OutQuad))
                .Append(t.DOScale(originalScale * 0.9f, 0.08f).SetEase(Ease.InQuad))
                .Append(t.DOScale(originalScale * 1.03f, 0.08f).SetEase(Ease.OutQuad))
                .Append(t.DOScale(originalScale, 0.1f).SetEase(Ease.OutQuad));
            punchTween.OnComplete(() =>
            {
                var position = ghostBuilding.transform.position;
                buildableEntity.transform.position = position;
                gridSystem.ResetCellsOccupied(selectedInstanceId);
                gridSystem.SetCellsOccupied(
                    position,
                    buildableEntity.Size,
                    buildableEntity.InstanceId
                );
                buildableEntity.gameObject.SetActive(true);

                HideGhostBuilding();
                currentState = BuildingSystemState.NONE;
                selectedInstanceId = -1;
                var size = buildableEntity.Size;
                _ = EventBus.PublishAsync(
                    new SelectEditingBuildingEvent
                    {
                        instanceId = buildableEntity.InstanceId,
                        worldPosition = position,
                        size = size,
                    }
                );
            });
            KillMoveTween();
        }
    }

    private void MoveGhostBuilding(int instanceIdToIgnore = 0)
    {
        moveTween?.Kill();
        var lastMousePosition = Mouse.current.position.ReadValue();
        var buildingPrefab = buildingData.EntityPrefab as BuildableEntity;
        var skin = buildingData.Skin;
        var buildingSize = new Vector2(
            skin.bounds.size.x * buildingPrefab.DisplaySize.x,
            skin.bounds.size.y * buildingPrefab.DisplaySize.y
        );
        var originalScale = ghostBuilding.transform.localScale;

        moveTween = DOTween
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
                isOverlapping = gridSystem.IsCellOccupied(
                    worldPos,
                    buildingSize,
                    instanceIdToIgnore
                );
                if (isOverlapping)
                {
                    ghostBuilding.SetSkinColor(Color.red);
                }
                else
                {
                    ghostBuilding.SetSkinColor(Color.green);
                }
                var snapGridPos = gridSystem.GetSnapGridPosition(worldPos);
                if ((lastSnappedPosition - snapGridPos).sqrMagnitude > 0.001f)
                {
                    snapTween?.Kill();
                    ghostBuilding.transform.position = snapGridPos;
                    ghostBuilding.transform.localScale = originalScale;
                    lastSnappedPosition = snapGridPos;

                    snapTween = ghostBuilding
                        .transform.DOScale(originalScale * 1.08f, 0.08f)
                        .SetEase(Ease.OutQuad)
                        .OnComplete(() =>
                        {
                            ghostBuilding
                                .transform.DOScale(originalScale, 0.22f)
                                .SetEase(Ease.OutBack);
                        });
                }
                GamePlugin.BlockInput(true);
            });
    }

    private void EndBuildBuilding()
    {
        if (currentState == BuildingSystemState.NONE)
            return;
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
        var originalScale = ghostBuilding.transform.localScale;
        var t = ghostBuilding.transform;
        var punchTween = DOTween
            .Sequence()
            // 1. Slight enlarge (anticipation)
            .Append(t.DOScale(originalScale * 1.08f, 0.06f).SetEase(Ease.OutQuad))
            // 2. Strong squash (impact)
            .Append(t.DOScale(originalScale * 0.9f, 0.08f).SetEase(Ease.InQuad))
            // 3. Bounce back
            .Append(t.DOScale(originalScale * 1.03f, 0.08f).SetEase(Ease.OutQuad))
            // 4. Settle
            .Append(t.DOScale(originalScale, 0.1f).SetEase(Ease.OutQuad));
        punchTween.OnComplete(() =>
        {
            var position = ghostBuilding.transform.position;
            var entityId = EntityId.ParseId(buildingData.EntityId);
            var buildingEntity = entityManager.Acquire<BuildableEntity>(
                entityId,
                new EntityConfig { position = position }
            );
            var currentBuildableEntity = buildingEntity;
            currentBuildableEntity.SetBuildingState(BuildingState.READY);
            gridSystem.SetCellsOccupied(position, buildingEntity.Size, buildingEntity.InstanceId);
            HideGhostBuilding();
            currentState = BuildingSystemState.NONE;
        });
        KillMoveTween();
    }

    private void HandleRightMouseClick()
    {
        if (currentState == BuildingSystemState.BUILD)
        {
            DestroyBuilding();
        }
        else if (currentState == BuildingSystemState.MOVE)
        {
            CancelMoving();
        }
    }

    private void DestroyBuilding()
    {
        KillMoveTween();
        HideGhostBuilding();
        currentState = BuildingSystemState.NONE;
    }

    private void CancelMoving()
    {
        var entityInstance = entityManager.GetInstantiatedEntity(selectedInstanceId);
        if (entityInstance == null)
        {
            Debug.LogError($"Cannot find entity instance with ID: {selectedInstanceId}");
            return;
        }
        if (entityInstance is BuildableEntity buildableEntity)
        {
            buildableEntity.gameObject.SetActive(true);
            KillMoveTween();
            HideGhostBuilding();
            currentState = BuildingSystemState.NONE;
            selectedInstanceId = -1;
            var size = buildableEntity.Size;
            _ = EventBus.PublishAsync(
                new SelectEditingBuildingEvent
                {
                    instanceId = buildableEntity.InstanceId,
                    worldPosition = buildableEntity.transform.position,
                    size = size,
                }
            );
        }
    }

    private void KillMoveTween()
    {
        moveTween?.Kill();
        moveTween = null;
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
