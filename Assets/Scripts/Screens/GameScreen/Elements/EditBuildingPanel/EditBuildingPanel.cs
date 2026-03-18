using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class EditBuildingPanel : MonoBehaviour
{
    [SerializeField]
    private MagicButtonWithIcon moveButton;

    [SerializeField]
    private MagicButtonWithIcon rotateButton;

    [SerializeField]
    private MagicButtonWithIcon cancelButton;

    [SerializeField]
    private float duration = 0.3f;

    [SerializeField]
    private Ease ease = Ease.OutBack;

    private MagicButtonWithIcon[] buttons = null;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Tween currentTween;
    private int selectedEntityInstanceId = -1;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        moveButton.AddListener(HandleMoveButtonClick);
        rotateButton.AddListener(HandleRotateButtonClick);
        cancelButton.AddListener(HandleCancelButtonClick);
        buttons = new MagicButtonWithIcon[] { moveButton, rotateButton, cancelButton };
    }

    void OnDestroy()
    {
        moveButton.RemoveListener(HandleMoveButtonClick);
        rotateButton.RemoveListener(HandleRotateButtonClick);
        cancelButton.RemoveListener(HandleCancelButtonClick);
    }

    public void SetCurrentInstanceId(int instanceId)
    {
        selectedEntityInstanceId = instanceId;
    }

    public void SetRectPosition(Vector3 worldPosition, Vector3 offset)
    {
        // up
        var upPosition = Camera.main.WorldToScreenPoint(
            worldPosition + new Vector3(0, offset.y, 0)
        );
        //down
        var downPosition = Camera.main.WorldToScreenPoint(
            worldPosition + new Vector3(0, -offset.y, 0)
        );

        var size = rectTransform.rect.size;
        var outOfScreenDirection = UIUtils.IsOutOfScreen(upPosition, size);
        rectTransform.position = outOfScreenDirection ? downPosition : upPosition;
    }

    private async UniTask HandleMoveButtonClick()
    {
        GamePlugin.BlockInput(true);
        await FadeOutAsync();
        EventBus.Publish(new MoveSelectBuildingEvent { instanceId = selectedEntityInstanceId });
        gameObject.SetActive(false);
        GamePlugin.BlockInput(false);
    }

    private void HandleRotateButtonClick()
    {
        EventBus.Publish(new RotateSelectBuildingEvent { instanceId = selectedEntityInstanceId });
    }

    private async UniTask HandleCancelButtonClick()
    {
        GamePlugin.BlockInput(true);
        await FadeOutAsync();
        EventBus.Publish(new CancelSelectEvent());
        gameObject.SetActive(false);
        GamePlugin.BlockInput(false);
    }

    public async UniTask FadeInAsync()
    {
        currentTween?.Kill();
        Sequence seq = DOTween.Sequence();
        seq.Join(canvasGroup.DOFade(1f, 0.25f));
        seq.Join(rectTransform.DOScale(1f, duration).SetEase(ease));
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].transform.localScale = Vector3.zero;

            seq.Join(
                buttons[i]
                    .transform.DOScale(1f, duration * 0.6f)
                    .SetDelay(i * 0.05f)
                    .SetEase(Ease.OutBack)
            );
        }
        currentTween = seq;
        await seq.AsyncWaitForCompletion();
    }

    public async UniTask FadeOutAsync()
    {
        currentTween?.Kill();
        Sequence seq = DOTween.Sequence();
        seq.Join(canvasGroup.DOFade(0f, 0.25f));
        seq.Join(rectTransform.DOScale(0f, duration).SetEase(ease));
        for (int i = 0; i < buttons.Length; i++)
        {
            seq.Join(
                buttons[i]
                    .transform.DOScale(0f, duration * 0.6f)
                    .SetDelay(i * 0.05f)
                    .SetEase(Ease.InBack)
            );
        }
        currentTween = seq;
        await seq.AsyncWaitForCompletion();
    }
}
