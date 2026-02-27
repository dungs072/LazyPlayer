using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BaseEngine
{
    public enum ButtonType
    {
        NONE,
        SCALE,
    }
    public enum ButtonState
    {
        NONE,
        BUBBLE,
    }
    [RequireComponent(typeof(RectTransform))]
    public class MagicButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private Action OnClicked;
        [SerializeField] private float increasePercentHitArea = 0.1f;
        [SerializeField] private bool isClickedOnHitArea = true;
        [SerializeField] private ButtonType buttonType = ButtonType.NONE;
        // for scale button only
        [Header("Scale button")]
        [SerializeField] private float scaleDownFactor = 0.9f;
        [SerializeField] private float scaleUpFactor = 1f;
        [SerializeField] private float scaleDuration = 0.3f;
        [Header("State")]
        [SerializeField] private ButtonState buttonState = ButtonState.NONE;
        // for bubble config only
        [SerializeField] private float bubbleUpFactor = 1.2f;
        [SerializeField] private float bubbleDownFactor = 0.95f;
        [SerializeField] private float bubbleUpDuration = 0.3f;
        [SerializeField] private float bubbleDownDuration = 0.5f;

        private RectTransform rectTransform;
        private bool isClicking = false;
        private CancellationTokenSource cts;

        void Awake()
        {
            InitComponents();
            InitButtonState();
            IncreaseHitArea();
        }
        void OnDestroy()
        {
            cts?.Cancel();
            cts?.Dispose();
        }
        public void AddListener(Action action)
        {
            OnClicked += action;
        }
        public void RemoveListener(Action action)
        {
            OnClicked -= action;
        }
        private void InitComponents()
        {
            rectTransform = GetComponent<RectTransform>();
        }
        private void InitButtonState()
        {
            if (buttonState == ButtonState.BUBBLE)
            {
                rectTransform.DOScale(bubbleUpFactor, bubbleUpDuration)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    rectTransform.DOScale(bubbleDownFactor, bubbleDownDuration)
                        .SetEase(Ease.InOutSine)
                        .SetLoops(-1, LoopType.Yoyo);
                });
            }
        }

        private void IncreaseHitArea()
        {
            if (!rectTransform) return;
            Vector2 sizeDelta = rectTransform.sizeDelta;
            sizeDelta.x += sizeDelta.x * increasePercentHitArea;
            sizeDelta.y += sizeDelta.y * increasePercentHitArea;
            rectTransform.sizeDelta = sizeDelta;

        }


        public void OnPointerDown(PointerEventData eventData)
        {
            if (isClicking) return;

            isClicking = true;

            KillAllAsync();

            if (buttonType == ButtonType.SCALE)
            {
                PlayPointerDownAnim(cts.Token).Forget();
            }
        }
        private async UniTask PlayPointerDownAnim(CancellationToken token)
        {
            if (buttonType == ButtonType.SCALE)
            {
                var tween = transform.DOScale(scaleDownFactor, scaleDuration);
                await tween
                    .AsyncWaitForCompletion()
                    .AsUniTask()
                    .AttachExternalCancellation(token);
            }
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            HandleButtonClicked(eventData, cts.Token).Forget();
        }
        private async UniTask HandleButtonClicked(PointerEventData eventData, CancellationToken token)
        {
            await PlayPointerUpAnim(token);

            isClicking = false;

            var canClick = true;

            if (isClickedOnHitArea && !IsPointerOverUI(eventData))
            {
                canClick = false;
            }

            if (!canClick)
            {
                InitButtonState();
                return;
            }

            OnClicked?.Invoke();
        }
        private async UniTask PlayPointerUpAnim(CancellationToken token)
        {
            if (buttonType == ButtonType.SCALE)
            {
                var tween = transform.DOScale(scaleUpFactor, scaleDuration);
                await tween
                    .AsyncWaitForCompletion()
                    .AsUniTask()
                    .AttachExternalCancellation(token);
            }
        }
        private bool IsPointerOverUI(PointerEventData eventData)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(
            rectTransform,
            eventData.position,
            eventData.enterEventCamera);
        }
        private void KillAllAsync()
        {
            if (cts != null)
            {
                cts.Cancel();
                cts.Dispose();
            }

            cts = new CancellationTokenSource();

            transform.DOKill();
        }
    }

}
