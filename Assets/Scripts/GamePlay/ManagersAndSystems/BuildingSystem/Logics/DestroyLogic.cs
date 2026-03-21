using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public struct DestroyBuildingEvent { }

public class DestroyLogic
{
    private readonly BuildingSystem system;
    private readonly EntityManager entityManager;
    private readonly GridSystem gridSystem;
    private Tween destroyTween = null;

    public DestroyLogic(
        BuildingSystem buildingSystem,
        EntityManager entityManager,
        GridSystem gridSystem
    )
    {
        system = buildingSystem;
        this.entityManager = entityManager;
        this.gridSystem = gridSystem;

        EventBus.Subscribe<DestroyBuildingEvent>(HandleDestroyBuilding);
        EventBus.Subscribe<ConfirmDestroyBuildingEvent>(HandleConfirmDestroyBuilding);
    }

    ~DestroyLogic()
    {
        Debug.Log($"<color=#11e2c2>DestroyLogic: </color>");
        EventBus.Unsubscribe<DestroyBuildingEvent>(HandleDestroyBuilding);
        EventBus.Unsubscribe<ConfirmDestroyBuildingEvent>(HandleConfirmDestroyBuilding);
    }

    private void HandleDestroyBuilding(DestroyBuildingEvent e)
    {
        InputHandler.OnMouseLeftClick += HandleSelectDestroyBuilding;
    }

    private void HandleSelectDestroyBuilding()
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
        if (entityInstance == null)
        {
            Debug.LogError($"Cannot find entity instance with ID: {entityInstanceId}");
            return;
        }
        EventBus.Publish(
            new SelectDestroyingBuildingEvent
            {
                instanceId = entityInstanceId,
                worldPosition = entityInstance.transform.position,
                duration = 2f,
            }
        );
        InputHandler.OnMouseCancelled += HandleMouseLeftCanceled;
    }

    private void HandleMouseLeftCanceled()
    {
        EventBus.Publish(new ExitDestroyBuildingUIEvent());
        InputHandler.OnMouseLeftClick -= HandleSelectDestroyBuilding;
        InputHandler.OnMouseCancelled -= HandleMouseLeftCanceled;
    }

    private void HandleConfirmDestroyBuilding(ConfirmDestroyBuildingEvent e)
    {
        var entityInstance = entityManager.GetInstantiatedEntity(e.instanceId);
        if (entityInstance == null)
        {
            Debug.LogError($"Cannot find entity instance with ID: {e.instanceId}");
            return;
        }
        destroyTween?.Kill();
        destroyTween = system.CreateDestroyTween(entityInstance);
        destroyTween.OnComplete(() =>
        {
            gridSystem.ResetCellsOccupied(e.instanceId);
            entityManager.Release(entityInstance);
        });
    }
}
