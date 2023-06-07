using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallingState : PlayerOnAirState
{
	private readonly int r_FallingIdleAnimatorState = Animator.StringToHash("FallingIdle");

	private const float k_AnimationTransitionTime = 0.15f;

	public PlayerFallingState(PlayerStateMachine stateMachine) : base(stateMachine)
	{
	}

	public override void Enter()
	{
		_ctx.MainAnimator.CrossFadeInFixedTime(r_FallingIdleAnimatorState, k_AnimationTransitionTime);
	}

	public override void FixedTick(float fixedDeltaTime)
	{
		base.FixedTick(fixedDeltaTime);

		if (_ctx.IsGrounded())
		{
			_ctx.PlayerCollider.height = _ctx.OriginalHeight;
			_ctx.SwitchCurrentState(new PlayerIdleState(_ctx));
		}

		MovePlayer(_ctx.MovementSpeed);
		ClampsHorizontalVelocity(_ctx.MaxHorizontalSpeed);
		RotatePlayerToForward();
	}

	public override void Tick(float deltaTime)
	{

	}

	public override void Exit()
	{
		
	}
}
