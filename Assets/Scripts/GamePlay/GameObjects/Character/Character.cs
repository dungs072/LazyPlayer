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
    private Animator animator;

    [SerializeField]
    private Movement movement;

    private Queue<BaseWorker> workQueue = new();
    private BaseWorker worker = null;

    private CharacterData characterData = null;
    public CharacterData CharacterData => characterData;
    public Movement MovementComponent => movement;
    public ChatPanel ChatPanelComponent => chatPanelComponent;

    public CharacterAnimator characterAnimator = null;
    public EntityManager entityManager = null;
    private CancellationTokenSource currentCTS = null;

    public void Initialize(
        EntityManager entityManager,
        RuntimeAnimatorController characterAnimation
    )
    {
        this.entityManager = entityManager;
        chatPanelComponent.HideChat();
        characterAnimator = new CharacterAnimator(movement, animator);
        animator.runtimeAnimatorController = characterAnimation;
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
