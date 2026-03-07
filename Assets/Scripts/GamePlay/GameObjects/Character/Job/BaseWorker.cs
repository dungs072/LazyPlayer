using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BaseWorker
{
    protected Movement movement;
    protected ISwitchableJob switchableJob;
    protected IDoable doable;
    protected Transform transform;
    protected ChatPanel chatPanel;
    public virtual string JobName()
    {
        return "BaseWorker";
    }
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
    
    public virtual async UniTask DoJobAsync(CancellationToken cancellationToken)
    {
        await UniTask.Yield();
    }
}
