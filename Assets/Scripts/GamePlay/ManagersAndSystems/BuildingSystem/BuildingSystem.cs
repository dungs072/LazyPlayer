using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingSystem : MonoBehaviour
{
    [SerializeField]
    private GhostBuilding gb;
    public GhostBuilding GB => gb;
    private GridSystem gridSystem = null;
    public bool IsOverlapping { get; private set; } = false;
    public EntityData Data { get; set; } = null;
    private Tween moveTween = null;
    private Tween snapTween = null;
    private Vector3 lastSnappedPosition = Vector3.zero;

    private BuildLogic buildLogic;
    private EditLogic editLogic;
    private MoveLogic moveLogic;
    private DestroyLogic destroyLogic;

    private void OnDestroy()
    {
        buildLogic?.Dispose();
        editLogic?.Dispose();
        moveLogic?.Dispose();
        destroyLogic?.Dispose();
    }

    public void Initialize1(EntityManager entityManager, GridSystem gridSystem)
    {
        this.gridSystem = gridSystem;
        buildLogic = new BuildLogic(this, entityManager, gridSystem);
        editLogic = new EditLogic(this, entityManager, gridSystem);
        moveLogic = new MoveLogic(this, entityManager, gridSystem);
        destroyLogic = new DestroyLogic(this, entityManager, gridSystem);
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
                    gb.UpdateOriginalValues();

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

    public Tween CreatePunchTween()
    {
        var originalScale = GB.transform.localScale;
        return DOTween
            .Sequence()
            .Append(GB.transform.DOScale(originalScale * 1.08f, 0.06f).SetEase(Ease.OutQuad))
            .Append(GB.transform.DOScale(originalScale * 0.9f, 0.08f).SetEase(Ease.InQuad))
            .Append(GB.transform.DOScale(originalScale * 1.03f, 0.08f).SetEase(Ease.OutQuad))
            .Append(GB.transform.DOScale(originalScale, 0.1f).SetEase(Ease.OutQuad));
    }

    public Tween CreateBlockTween()
    {
        var t = GB.transform;
        var originalScale = t.localScale;
        var originalPos = t.position;

        t.DOKill();

        return DOTween
            .Sequence()
            .Append(
                t.DOScale(new Vector3(1.15f * originalScale.x, 0.85f * originalScale.y, 1f), 0.05f)
                    .SetEase(Ease.OutQuad)
            )
            .Append(t.DOLocalMoveX(originalPos.x - 0.12f, 0.03f).SetEase(Ease.OutQuad))
            .Append(t.DOLocalMoveX(originalPos.x + 0.12f, 0.04f).SetEase(Ease.InOutQuad))
            .Append(t.DOLocalMoveX(originalPos.x - 0.08f, 0.03f).SetEase(Ease.InOutQuad))
            .Append(t.DOLocalMoveX(originalPos.x + 0.05f, 0.03f).SetEase(Ease.InOutQuad))
            .Append(t.DOLocalMoveX(originalPos.x, 0.02f).SetEase(Ease.OutQuad))
            .Join(t.DOScale(originalScale, 0.08f).SetEase(Ease.OutQuad));
    }

    public Tween CreateDestroyTween(Entity entity)
    {
        var t = entity.transform;
        var originalScale = t.localScale;

        return DOTween
            .Sequence()
            .Append(
                t.DOScale(new Vector3(1.15f * originalScale.x, 0.85f * originalScale.y, 1f), 0.05f)
                    .SetEase(Ease.OutQuad)
            )
            .Append(
                t.DOScale(new Vector3(0.8f * originalScale.x, 1.2f * originalScale.y, 1f), 0.08f)
                    .SetEase(Ease.InQuad)
            )
            .Append(
                t.DOScale(new Vector3(1.1f * originalScale.x, 0.9f * originalScale.y, 1f), 0.08f)
                    .SetEase(Ease.OutQuad)
            )
            .Append(t.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InQuad));
    }
}
