using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BaseWorker
{
    public static BaseWorker EMPTY_WORKER = new();
    protected Character character;
    public virtual string JobName()
    {
        return "BaseWorker";
    }

    public void SetCharacter(Character character)
    {
        this.character = character;
    }
    public virtual async UniTask DoJobAsync(CancellationToken cancellationToken)
    {
        await UniTask.Yield();
    }
}
