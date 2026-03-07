using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;

    public async UniTask Move(CancellationToken cancellationToken, Vector3 position)
    {
        //TODO: Have to fix this
        while (this != null && transform != null && transform.position != position)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                position,
                Time.deltaTime * speed
            );
            await UniTask.NextFrame(PlayerLoopTiming.Update, cancellationToken);
        }
    }
}
