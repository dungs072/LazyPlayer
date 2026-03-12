using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Movement))]
public class Character : MonoBehaviour, ISwitchableJob, IDoable
{
    [SerializeField]
    private JobHandler jobHandler;

    [SerializeField]
    private ChatPanel chatPanel;

    [SerializeField]
    private TMP_Text nameText;

    [SerializeField]
    private SpriteRenderer characterSpriteRenderer;

    private Movement movement = null;
    private CharacterData characterData = null;

    public CharacterData CharacterData => characterData;

    private CancellationTokenSource currentCTS = null;

    public void Initialize(Sprite characterSkin)
    {
        movement = GetComponent<Movement>();
        jobHandler.Init(movement);
        chatPanel.HideChat();
        characterSpriteRenderer.sprite = characterSkin;
    }

    public void StartJob()
    {
        currentCTS?.Cancel();
        currentCTS?.Dispose();
        currentCTS = new CancellationTokenSource();
        jobHandler.DoJobAsync(currentCTS.Token).Forget();
    }

    public void SetJob(BaseWorker worker)
    {
        worker.SetTransform(transform);
        jobHandler.SetWorker(worker, this, chatPanel, this);
        nameText.text = worker.GetType().Name;
    }

    public void SetIsLoopingDoJob(bool isLoopingDoJob)
    {
        jobHandler.SetIsLoopingDoJob(isLoopingDoJob);
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
        var worker = jobHandler.Worker;
        data.JobName = worker != null ? worker.JobName() : "Unemployed";
        characterData = data;
    }

    private void OnDestroy()
    {
        currentCTS?.Cancel();
        currentCTS?.Dispose();
        currentCTS = null;
    }
}
