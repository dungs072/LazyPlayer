using DG.Tweening;
using UnityEngine;

public struct MoveSelectBuildingEvent
{
    public int instanceId;
}

public class MoveLogic : System.IDisposable
{
    private readonly BuildingSystem system;
    private readonly EntityManager entityManager;
    private readonly GridSystem gridSystem;
    private int selectedInstanceId = -1;

    private Tween blockTween = null;

    public MoveLogic(
        BuildingSystem buildingSystem,
        EntityManager entityManager,
        GridSystem gridSystem
    )
    {
        system = buildingSystem;
        this.entityManager = entityManager;
        this.gridSystem = gridSystem;

        EventBus.Subscribe<MoveSelectBuildingEvent>(StartMoveBuilding);
    }

    public void Dispose()
    {
        EventBus.Unsubscribe<MoveSelectBuildingEvent>(StartMoveBuilding);
    }

    private void StartMoveBuilding(MoveSelectBuildingEvent e)
    {
        if (system.GB == null)
        {
            // Debug.LogError("GhostBuilding (GB) is null or destroyed. Cannot start moving.");
            return;
        }
        selectedInstanceId = e.instanceId;
        var entityInstance = entityManager.GetInstantiatedEntity(selectedInstanceId);
        if (entityInstance == null)
        {
            Debug.LogError($"Cannot find entity instance with ID: {selectedInstanceId}");
            return;
        }
        if (entityInstance is BuildableEntity buildableEntity)
        {
            system.Data = buildableEntity.Data;
            system.GB.SetSkin(system.Data.Skin);
            system.GB.transform.position = buildableEntity.transform.position;
            system.GB.transform.rotation = buildableEntity.transform.rotation;
            system.GB.SetSize(buildableEntity.DisplaySize);
            system.GB.gameObject.SetActive(true);
            system.GB.SetSkinColor(Color.green);
            buildableEntity.HideVisualForBuilding();
            system.MoveGhostBuilding(selectedInstanceId);
            InputHandler.OnMouseLeftClick += EndMoveBuilding;
            InputHandler.OnMouseRightClick += CancelMoving;
        }
    }

    private void EndMoveBuilding()
    {
        if (GamePlugin.IsPointerOverUI())
        {
            return;
        }
        var entityInstance = entityManager.GetInstantiatedEntity(selectedInstanceId);
        if (entityInstance == null)
        {
            Debug.LogError($"Cannot find entity instance with ID: {selectedInstanceId}");
            return;
        }
        if (entityInstance is BuildableEntity buildableEntity)
        {
            if (system.IsOverlapping)
            {
                blockTween?.Kill();
                system.GB.ResetOriginalValues();
                blockTween = system.CreateBlockTween();
                return;
            }
            var originalScale = system.GB.transform.localScale;
            var punchTween = system.CreatePunchTween();
            punchTween.OnComplete(() =>
            {
                var position = system.GB.transform.position;
                buildableEntity.transform.position = position;
                gridSystem.ResetCellsOccupied(selectedInstanceId);
                gridSystem.SetCellsOccupied(
                    position,
                    buildableEntity.Size,
                    buildableEntity.InstanceId
                );
                buildableEntity.ShowVisualForBuilding();

                system.HideGhostBuilding();
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
                EventBus.Publish(new EditBuildingEvent());
            });
            system.KillMoveTween();
            InputHandler.OnMouseLeftClick -= EndMoveBuilding;
            InputHandler.OnMouseRightClick -= CancelMoving;
        }
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
            system.KillMoveTween();
            system.HideGhostBuilding();
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
            InputHandler.OnMouseLeftClick -= EndMoveBuilding;
            InputHandler.OnMouseRightClick -= CancelMoving;
        }
    }
}
