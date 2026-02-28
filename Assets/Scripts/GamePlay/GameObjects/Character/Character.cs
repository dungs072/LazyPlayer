using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using System;

using TMPro;

[RequireComponent(typeof(Movement))]
public class Character : MonoBehaviour, ISwitchableJob, IDoable
{
    [SerializeField] private JobHandler jobHandler;
    [SerializeField] private ChatPanel chatPanel;
    [SerializeField] private TMP_Text nameText;
    private Movement movement = null;
    private CharacterData characterData = null;

    public CharacterData CharacterData => characterData;

    void Awake()
    {
        movement = GetComponent<Movement>();
        jobHandler.Init(movement);
        chatPanel.HideChat();
    }
    public void StartJob()
    {
        StopAllCoroutines();
        StartCoroutine(jobHandler.DoJobAsync());
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

    public void DoJobAsync(Func<IEnumerator> action)
    {
        StopAllCoroutines();
        StartCoroutine(action());
    }

    public void SetCharacterData(CharacterData data)
    {
        var worker = jobHandler.Worker;
        data.JobName = worker != null ? worker.JobName() : "Unemployed";
        characterData = data;
    }

}
