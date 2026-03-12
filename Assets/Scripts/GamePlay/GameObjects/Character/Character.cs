using UnityEngine;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;

[RequireComponent(typeof(Movement))]
public class Character : MonoBehaviour
{
    [SerializeField] private ChatPanel chatPanel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private SpriteRenderer characterSpriteRenderer;
    [SerializeField] private bool isLooping = true;
    
    private Movement movement = null;
    private CharacterData characterData = null;
    private BaseWorker worker = null;

    public CharacterData CharacterData => characterData;

    private CancellationTokenSource currentCTS = null;

    public void Initialize(Sprite characterSkin)
    {
        movement = GetComponent<Movement>();
        chatPanel.HideChat();
        characterSpriteRenderer.sprite = characterSkin;
    }
    public void StartJob()
    {
        currentCTS?.Cancel();
        currentCTS?.Dispose();
        currentCTS = new CancellationTokenSource();
        DoJobAsync(currentCTS.Token).Forget();
    }

    public void SetJob(BaseWorker worker)
    {
        worker.SetCharacter(this);
        worker.SetTransform(transform);
        worker.SetMovement(movement);
        worker.SetChatPanel(chatPanel);
        this.worker = worker;
        nameText.text = worker.GetType().Name;
    }
    public void SetIsLoopingDoJob(bool isLoopingDoJob)
    {
        isLooping = isLoopingDoJob;
    }

    public void DoJobAsync(Func<CancellationToken, UniTask> action)
    {
        currentCTS?.Cancel();
        currentCTS?.Dispose();
        currentCTS = new CancellationTokenSource();
        action(currentCTS.Token).Forget();
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
            await worker.DoJobAsync(cancellationToken);
        }
        while (!cancellationToken.IsCancellationRequested && isLooping);
    }
}
