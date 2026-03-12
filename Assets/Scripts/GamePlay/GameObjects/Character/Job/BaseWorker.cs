using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BaseWorker
{
    protected Character character;
    protected Movement movement;
    protected Transform transform;
    protected ChatPanel chatPanel;
    public virtual string JobName()
    {
        return "BaseWorker";
    }

    public void SetCharacter(Character character)
    {
        this.character = character;
    }
    public void SetMovement(Movement movement)
    {
        this.movement = movement;
    }
    public void SetTransform(Transform transform)
    {
        this.transform = transform;
    }
    public void SetChatPanel(ChatPanel chatPanel)
    {
        this.chatPanel = chatPanel;
    }
    public virtual async UniTask DoJobAsync(CancellationToken cancellationToken)
    {
        await UniTask.Yield();
    }
}
