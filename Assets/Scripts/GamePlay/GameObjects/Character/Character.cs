using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Movement))]
public class Character : MonoBehaviour
{
    [SerializeField]
    private ChatPanel chatPanelComponent;

    [SerializeField]
    private TMP_Text nameText;

    [SerializeField]
    private SpriteRenderer characterSpriteRenderer;

    private Queue<BaseWorker> workQueue = new();
    private BaseWorker worker = null;

    private Movement movementComponent = null;
    private CharacterData characterData = null;
    public CharacterData CharacterData => characterData;
    public Movement MovementComponent => movementComponent;
    public ChatPanel ChatPanelComponent => chatPanelComponent;

    private CancellationTokenSource currentCTS = null;

    public void Initialize(Sprite characterSkin)
    {
        movementComponent = GetComponent<Movement>();
        chatPanelComponent.HideChat();
        characterSpriteRenderer.sprite = characterSkin;
    }

    public void StartJob()
    {
        currentCTS = new CancellationTokenSource();
        DoJobAsync(currentCTS.Token).Forget();
    }

    public void EnqueueJob(BaseWorker worker)
    {
        workQueue.Enqueue(worker);
    }

    public void SetCharacterData(CharacterData data)
    {
        data.JobName = worker != null ? worker.JobName() : "Unemployed";
        characterData = data;
    }

    private void OnDestroy()
    {
        currentCTS?.Cancel();
        currentCTS?.Dispose();
        currentCTS = null;
    }

    public async UniTask DoJobAsync(CancellationToken cancellationToken)
    {
        do
        {
            var baseWorker = workQueue.Count > 0 ? workQueue.Dequeue() : BaseWorker.EMPTY_WORKER;
            baseWorker.SetCharacter(this);
            worker = baseWorker;
            nameText.text = baseWorker.GetType().Name;
            await baseWorker.DoJobAsync(cancellationToken);
            await UniTask.NextFrame(PlayerLoopTiming.Update, cancellationToken);
        } while (!cancellationToken.IsCancellationRequested);
    }
}
