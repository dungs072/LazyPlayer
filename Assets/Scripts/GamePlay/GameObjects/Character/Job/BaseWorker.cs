using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BaseWorker
{
    protected Character character;
    protected Movement movementComponent;
    protected Transform transform;
    protected ChatPanel chatPanelComponent;
    public virtual string JobName()
    {
        return "BaseWorker";
    }

    public void SetCharacter(Character character)
    {
        this.character = character;
        movementComponent = character.MovementComponent;
        transform = character.transform;
        chatPanelComponent = character.ChatPanelComponent;
    }
    public virtual async UniTask DoJobAsync(CancellationToken cancellationToken)
    {
        await UniTask.Yield();
    }
}
