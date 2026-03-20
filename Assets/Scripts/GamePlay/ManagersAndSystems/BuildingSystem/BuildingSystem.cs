using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingSystem : MonoBehaviour
{
    [field: SerializeField]
    public GhostBuilding GB { get; private set; }
    private GridSystem gridSystem = null;
    public bool IsOverlapping { get; private set; } = false;
    public EntityData Data { get; set; } = null;
    private Tween moveTween = null;
    private Tween snapTween = null;
    private Vector3 lastSnappedPosition = Vector3.zero;

    public void Initialize1(EntityManager entityManager, GridSystem gridSystem)
    {
        this.gridSystem = gridSystem;
        new BuildLogic(this, entityManager, gridSystem);
        new EditLogic(this, entityManager, gridSystem);
        new MoveLogic(this, entityManager, gridSystem);

        GB.gameObject.SetActive(false);
    }

    public void MoveGhostBuilding(int instanceIdToIgnore = 0)
    {
        moveTween?.Kill();
        var lastMousePosition = Mouse.current.position.ReadValue();
        var buildingPrefab = Data.EntityPrefab as BuildableEntity;
        var skin = Data.Skin;
        var buildingSize = new Vector2(
            skin.bounds.size.x * buildingPrefab.DisplaySize.x,
            skin.bounds.size.y * buildingPrefab.DisplaySize.y
        );
        var originalScale = GB.transform.localScale;

        moveTween = DOTween
            .To(() => 0f, _ => { }, 0f, 1f)
            .SetLoops(-1)
            .OnUpdate(() =>
            {
                if (!GB.gameObject.activeSelf)
                    return;
                var mousePosition = Mouse.current.position.ReadValue();
                if (mousePosition == lastMousePosition)
                    return;
                lastMousePosition = mousePosition;
                var worldPos = GeneralUtils.GetMouseWorldPosition(mousePosition);
                IsOverlapping = gridSystem.IsCellOccupied(
                    worldPos,
                    buildingSize,
                    instanceIdToIgnore
                );
                if (IsOverlapping)
                {
                    GB.SetSkinColor(Color.red);
                }
                else
                {
                    GB.SetSkinColor(Color.green);
                }
                var snapGridPos = gridSystem.GetSnapGridPosition(worldPos);
                if ((lastSnappedPosition - snapGridPos).sqrMagnitude > 0.001f)
                {
                    snapTween?.Kill();
                    GB.transform.position = snapGridPos;
                    GB.transform.localScale = originalScale;
                    lastSnappedPosition = snapGridPos;

                    snapTween = GB
                        .transform.DOScale(originalScale * 1.08f, 0.08f)
                        .SetEase(Ease.OutQuad)
                        .OnComplete(() =>
                        {
                            GB.transform.DOScale(originalScale, 0.22f).SetEase(Ease.OutBack);
                        });
                }
                GamePlugin.BlockInput(true);
            });
    }

    public void KillMoveTween()
    {
        moveTween?.Kill();
        moveTween = null;
        GamePlugin.BlockInput(false);
    }

    public void HideGhostBuilding()
    {
        if (GB.gameObject.activeSelf)
        {
            GB.SetSkin(null);
            GB.gameObject.SetActive(false);
        }
    }

    public Tween CreatePunchTween(Transform t)
    {
        var originalScale = t.localScale;
        return DOTween
            .Sequence()
            .Append(t.DOScale(originalScale * 1.08f, 0.06f).SetEase(Ease.OutQuad))
            .Append(t.DOScale(originalScale * 0.9f, 0.08f).SetEase(Ease.InQuad))
            .Append(t.DOScale(originalScale * 1.03f, 0.08f).SetEase(Ease.OutQuad))
            .Append(t.DOScale(originalScale, 0.1f).SetEase(Ease.OutQuad));
    }
}
