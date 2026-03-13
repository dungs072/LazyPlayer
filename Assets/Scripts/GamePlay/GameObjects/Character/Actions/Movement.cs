using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right
    }
    
    private MoveDirection currentMoveDirection = MoveDirection.Down;
    public MoveDirection CurrentMoveDirection => currentMoveDirection;
    
    private bool isMoving = false;
    public bool IsMoving => isMoving;


    public event Action OnStateChanged;
    
    [SerializeField]
    private float speed = 5f;

    public async UniTask Move(CancellationToken cancellationToken, Vector3 position)
    {
        //TODO: Have to fix this
        while (this != null && transform != null && transform.position != position)
        {
            Vector3 nextPosition;
            if (Mathf.Abs(transform.position.y - position.y) > 0.1f)
            {
                nextPosition = transform.position;
                nextPosition.y = position.y;
                var nextMoveDirection = nextPosition.y > transform.position.y ? MoveDirection.Up  : MoveDirection.Down;
                UpdateDirection(nextMoveDirection);
            }
            else
            {
                nextPosition = position;
                var nextMoveDirection = nextPosition.x > transform.position.x ? MoveDirection.Right  : MoveDirection.Left;
                UpdateDirection(nextMoveDirection);
            } 
            
            transform.position = Vector3.MoveTowards(
                transform.position,
                nextPosition,
                Time.deltaTime * speed
            );
            UpdateIsMoving(true);
            await UniTask.NextFrame(PlayerLoopTiming.Update, cancellationToken);
        }
        
        UpdateIsMoving(false);
    }

    private void UpdateDirection(MoveDirection nextMoveDirection)
    {
        if (currentMoveDirection != nextMoveDirection)
        {
            currentMoveDirection = nextMoveDirection;
            OnStateChanged?.Invoke();
        }
    }

    private void UpdateIsMoving(bool nextIsMoving)
    {
        if (isMoving != nextIsMoving)
        {
            isMoving = nextIsMoving; 
            OnStateChanged?.Invoke();
        }
    }
}
