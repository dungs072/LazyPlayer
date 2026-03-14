using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

enum BuildMode
{
    NONE,
    BUILD,
    EDIT,
}

public class BuildingSystem : MonoBehaviour
{
    [SerializeField]
    private GhostBuilding ghostBuilding;

    private BuildableEntity buildablePrefab;
    private bool isOverlapping = false;
    private Tween _moveTween;
    private BuildMode currentBuildMode = BuildMode.NONE;

    public void Initialize1()
    {
        EventBus.Subscribe<BuildBuildableEntityEvent>(StartBuildBuilding);
        InputHandler.OnMouseLeftClick += HandleSelection;
        InputHandler.OnMouseRightClick += CancelBuildBuilding;
        EventBus.Subscribe<EditBuildableEntityEvent>(StartEditBuilding);
        ghostBuilding.gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        EventBus.Unsubscribe<BuildBuildableEntityEvent>(StartBuildBuilding);
        InputHandler.OnMouseLeftClick -= HandleSelection;
        InputHandler.OnMouseRightClick -= CancelBuildBuilding;
        EventBus.Unsubscribe<EditBuildableEntityEvent>(StartEditBuilding);
    }

    private void StartBuildBuilding(BuildBuildableEntityEvent e)
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
        SwitchMode(BuildMode.BUILD);
        ghostBuilding.SetSkin(currentBuildableEntity.Skin);
        ghostBuilding.transform.position = centerWorldPos;
        ghostBuilding.SetSize(currentBuildableEntity.DisplaySize);
        ghostBuilding.gameObject.SetActive(true);
        ghostBuilding.SetSkinColor(Color.green);
        buildablePrefab = currentBuildableEntity;

        MoveGhostBuilding();
    }

    private void SwitchMode(BuildMode newMode)
    {
        currentBuildMode = newMode;
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
                if (!ghostBuilding.gameObject.activeSelf)
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
                    ghostBuilding.SetSkinColor(Color.red);
                }
                else
                {
                    ghostBuilding.SetSkinColor(Color.green);
                }
                var snapGridPos = QueryBus.Query(
                    new GetSnapGridPositionQuery { position = worldPos }
                );
                ghostBuilding.transform.position = snapGridPos;
                GamePlugin.BlockInput(true);
            });
    }

    private void HandleSelection()
    {
        GamePlugin.IsPointerOverUI();
        if (currentBuildMode == BuildMode.BUILD)
        {
            EndBuildBuilding();
        }
        else if (currentBuildMode == BuildMode.EDIT)
        {
            SelectEditBuilding();
        }
    }

    private void EndBuildBuilding()
    {
        // If no building is currently being placed, ignore the click event
        if (!ghostBuilding.gameObject.activeSelf)
        {
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
        var buildingEntity = QueryBus.Query(
            new GetEntityQuery { prefabId = buildablePrefab.EntityName, position = position }
        );
        var currentBuildableEntity = buildingEntity as BuildableEntity;
        currentBuildableEntity.SetBuildingState(BuildingState.READY);
        EventBus.Publish(
            new SetOccupiedGridEvent
            {
                position = position,
                size = buildablePrefab.Size,
                entityInstanceId = buildingEntity.InstanceId,
            }
        );
        KillMoveTween();
        HideGhostBuilding();
        SwitchMode(BuildMode.NONE);
    }

    private void CancelBuildBuilding()
    {
        KillMoveTween();
        HideGhostBuilding();
        SwitchMode(BuildMode.NONE);
    }

    private void StartEditBuilding(EditBuildableEntityEvent e)
    {
        SwitchMode(BuildMode.EDIT);
    }

    private void SelectEditBuilding()
    {
        var mousePosition = Mouse.current.position.ReadValue();
        var mouseWorldPos = GeneralUtils.GetMouseWorldPosition(mousePosition);
        var buildingId = QueryBus.Query(
            new GetEntityIdAtPositionQuery { position = mouseWorldPos }
        );
        if (buildingId == 0)
        {
            Debug.Log("No building found at the clicked position.");
            return;
        }
        var buildingEntity = QueryBus.Query(
            new GetInstantiatedEntityQuery { instanceId = buildingId }
        );
        var buildableEntity = buildingEntity as BuildableEntity;
        if (buildableEntity == null)
        {
            Debug.LogError($"The entity at the clicked position is not a BuildableEntity.");
            return;
        }
        var buildingPosition = buildingEntity.transform.position;
        var skin = buildableEntity.Skin;
        ghostBuilding.transform.localScale = new Vector2(
            buildableEntity.DisplaySize.x,
            buildableEntity.DisplaySize.y
        );
        ghostBuilding.transform.position = buildingPosition;
        ghostBuilding.SetSkin(skin);
        buildingEntity.gameObject.SetActive(false);
        ghostBuilding.gameObject.SetActive(true);
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
            // ghostBuilding.gameObject.SetActive(false);
        }
    }
}
