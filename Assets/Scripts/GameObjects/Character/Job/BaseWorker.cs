using System.Collections;
using UnityEngine;
using static EntityConstant;

public class BaseWorker
{
    protected Movement movement;
    protected ISwitchableJob switchableJob;
    protected IDoable doable;
    protected Transform transform;
    protected ChatPanel chatPanel;
    private JobData jobData;
    protected int currentStepIndex = 0;
    public void SetMovement(Movement movement)
    {
        this.movement = movement;
    }
    public void SetISwitchableJob(ISwitchableJob switchableJob)
    {
        this.switchableJob = switchableJob;
    }
    public void SetTransform(Transform transform)
    {
        this.transform = transform;
    }
    public void SetChatPanel(ChatPanel chatPanel)
    {
        this.chatPanel = chatPanel;
    }
    public void SetDoable(IDoable doable)
    {
        this.doable = doable;
    }

    public StepData GetStepData()
    {
        if (currentStepIndex == jobData.Steps.Length)
        {
            return null;
        }
        return jobData.Steps[currentStepIndex];
    }

    public virtual void FinishCurrentStep()
    {
        currentStepIndex++;
        if (currentStepIndex >= jobData.Steps.Length)
        {
            currentStepIndex = 0;
        }
    }

    public virtual IEnumerator DoJobAsync()
    {
        yield return null;
    }
}
