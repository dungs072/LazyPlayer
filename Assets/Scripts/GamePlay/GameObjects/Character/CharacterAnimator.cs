using UnityEngine;

public class CharacterAnimator
{
    private Movement movement;
    private Animator animatorComponent;
    
    private readonly int IdleState = Animator.StringToHash("Idle");
    private readonly int WalkUpState = Animator.StringToHash("WalkUp");
    private readonly int WalkDownState = Animator.StringToHash("WalkDown");
    private readonly int WalkLeftState = Animator.StringToHash("WalkLeft");
    private readonly int WalkRightState = Animator.StringToHash("WalkRight");

    private const float FadeTime = 0.1f;
    public CharacterAnimator(Movement movement, Animator animatorComponent)
    {
        movement.OnStateChanged += OnMoveStateChanged;
        
        this.movement = movement;
        this.animatorComponent = animatorComponent;
    }

    ~CharacterAnimator()
    {
        movement.OnStateChanged -= OnMoveStateChanged;
    }
    
    private void OnMoveStateChanged()
    {
        if (!movement.IsMoving)
        {
            animatorComponent.CrossFadeInFixedTime(IdleState, FadeTime);
            return;
        }
        
        switch (movement.CurrentMoveDirection)
        {
            case Movement.MoveDirection.Down:
                animatorComponent.CrossFadeInFixedTime(WalkDownState, FadeTime);
                break;
            case Movement.MoveDirection.Up:
                animatorComponent.CrossFadeInFixedTime(WalkUpState, FadeTime);
                break;
            case Movement.MoveDirection.Left:
                animatorComponent.CrossFadeInFixedTime(WalkLeftState, FadeTime);
                break;
            case Movement.MoveDirection.Right:
                animatorComponent.CrossFadeInFixedTime(WalkRightState, FadeTime);
                break;
        }
    }

}