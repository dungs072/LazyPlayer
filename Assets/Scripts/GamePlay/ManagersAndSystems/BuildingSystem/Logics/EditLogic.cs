using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public struct EditBuildingEvent { }

public struct CancelSelectEvent { }

public struct RotateSelectBuildingEvent
{
    public int instanceId;
}

public class EditLogic
{
    private readonly BuildingSystem system;
    private readonly EntityManager entityManager;
    private readonly GridSystem gridSystem;

    public EditLogic(
        BuildingSystem buildingSystem,
        EntityManager entityManager,
        GridSystem gridSystem
    )
    {
        system = buildingSystem;
        this.entityManager = entityManager;
        this.gridSystem = gridSystem;
        EventBus.Subscribe<EditBuildingEvent>(StartEditBuilding);
        EventBus.Subscribe<RotateSelectBuildingEvent>(StartRotateBuilding);
        EventBus.Subscribe<CancelSelectEvent>(CancelSelectBuilding);
    }

    ~EditLogic()
    {
        EventBus.Unsubscribe<EditBuildingEvent>(StartEditBuilding);
        EventBus.Unsubscribe<RotateSelectBuildingEvent>(StartRotateBuilding);
        EventBus.Unsubscribe<CancelSelectEvent>(CancelSelectBuilding);
    }

    private void StartEditBuilding(EditBuildingEvent e)
    {
        if (system.GB == null)
        {
            //Debug.LogError("GhostBuilding (GB) is null or destroyed. Cannot start editing.");
            return;
        }
        InputHandler.OnMouseLeftClick += SelectBuildingToEdit;
        EventBus.Unsubscribe<CancelSelectEvent>(CancelSelectBuilding);
    }

    private void SelectBuildingToEdit()
    {
        if (GamePlugin.IsPointerOverUI())
        {
            return;
        }
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
            InputHandler.OnMouseLeftClick -= SelectBuildingToEdit;
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
        InputHandler.OnMouseLeftClick -= SelectBuildingToEdit;
    }
}
