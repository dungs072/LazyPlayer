using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
[Serializable]
public class JobHandler
{
    [SerializeField] private JobData jobData;
    [SerializeField] private bool isLooping = true;

    private BaseWorker worker;
    private Movement movement;

    public BaseWorker Worker => worker;
    public void Init(Movement movement)
    {
        this.movement = movement;
    }

    public void SetWorker(BaseWorker worker, ISwitchableJob switchableJob, ChatPanel chatPanel, IDoable doable)
    {
        worker.SetMovement(movement);
        worker.SetISwitchableJob(switchableJob);
        worker.SetChatPanel(chatPanel);
        worker.SetDoable(doable);
        this.worker = worker;

    }
    public void SetIsLoopingDoJob(bool isLoopingDoJob)
    {
        isLooping = isLoopingDoJob;
    }

    public IEnumerator DoJobAsync()
    {
        do
        {
            yield return worker.DoJobAsync();
        }
        while (isLooping);
    }
}
