using DG.Tweening;
using UnityEngine;

public struct BuildBuildingEvent
{
    public EntityId entityId;
}

public class BuildLogic : System.IDisposable
{
    private readonly BuildingSystem system;
    private readonly EntityManager entityManager;
    private readonly GridSystem gridSystem;
    private Tween blockTween = null;

    public BuildLogic(
        BuildingSystem buildingSystem,
        EntityManager entityManager,
        GridSystem gridSystem
    )
    {
        system = buildingSystem;
        this.entityManager = entityManager;
        this.gridSystem = gridSystem;
        EventBus.Subscribe<BuildBuildingEvent>(StartBuildBuilding);
    }

    public void Dispose()
    {
        EventBus.Unsubscribe<BuildBuildingEvent>(StartBuildBuilding);
    }

    private void StartBuildBuilding(BuildBuildingEvent e)
    {
        if (system.GB == null)
        {
            //Debug.LogError("GhostBuilding (GB) is null or destroyed. Cannot start building.");
            return;
        }
        var centerWorldPos = QueryBus.Query(new GetCenterCameraPositionQuery());
        system.Data = entityManager.GetEntityData(e.entityId);
        var buildingPrefab = system.Data.EntityPrefab;
        if (buildingPrefab == null)
        {
            Debug.LogError($"Cannot find prefab for entity ID: {e.entityId}");
            return;
        }
        var buildableEntityPrefab = buildingPrefab as BuildableEntity;
        system.GB.SetSkin(system.Data.Skin);
        system.GB.transform.position = centerWorldPos;
        system.GB.transform.localScale = new Vector2(
            buildableEntityPrefab.DisplaySize.x,
            buildableEntityPrefab.DisplaySize.y
        );
        system.GB.gameObject.SetActive(true);
        system.GB.SetSkinColor(Color.green);
        InputHandler.OnMouseLeftClick += EndBuildBuilding;
        InputHandler.OnMouseRightClick += CancelBuilding;
        system.MoveGhostBuilding();
    }

    private void EndBuildBuilding()
    {
        if (GamePlugin.IsPointerOverUI())
        {
            return;
        }
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
            var entityId = EntityId.ParseId(system.Data.EntityId);
            var buildingEntity = entityManager.Acquire<BuildableEntity>(
                entityId,
                new EntityConfig { position = position }
            );
            var currentBuildableEntity = buildingEntity;
            currentBuildableEntity.SetBuildingState(BuildingState.READY);
            currentBuildableEntity.transform.localScale = Vector3.one;
            gridSystem.SetCellsOccupied(position, buildingEntity.Size, buildingEntity.InstanceId);
            system.HideGhostBuilding();
        });
        system.KillMoveTween();
        InputHandler.OnMouseLeftClick -= EndBuildBuilding;
        InputHandler.OnMouseRightClick -= CancelBuilding;
    }

    private void CancelBuilding()
    {
        system.KillMoveTween();
        system.HideGhostBuilding();
        InputHandler.OnMouseLeftClick -= EndBuildBuilding;
        InputHandler.OnMouseRightClick -= CancelBuilding;
    }
}
