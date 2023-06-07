using System.Collections;
using UnityEngine;

public class PlayerWalkingState : PlayerOnGroundState
{
	private readonly int r_WalkingAnimationState = Animator.StringToHash("Walking");

	private const float k_AnimationTransitionTime = 0.15f;

	public PlayerWalkingState(PlayerStateMachine stateMachine) : base(stateMachine)
	{
	}

	public override void Enter()
	{
		base.Enter();

		_ctx.PlayerInput.crouchEvent += OnCrouch;
		_ctx.PlayerInput.runEvent += OnRun;

		_ctx.PlayerSound.SetupStepsAudio();
		_ctx.MainAnimator.CrossFadeInFixedTime(r_WalkingAnimationState, k_AnimationTransitionTime);
	}

	public override void FixedTick(float fixedDeltaTime)
	{
		MovePlayer(_ctx.MovementSpeed);
		ClampsHorizontalVelocity(_ctx.MaxWalkSpeed);
		RotatePlayerToForward();

		base.FixedTick(fixedDeltaTime);
	}

	public override void Tick(float deltaTime)
	{
		base.Tick(deltaTime);

		if (_ctx.InputVector == Vector3.zero)
		{
			_ctx.SwitchCurrentState(new PlayerIdleState(_ctx));
		}
	}

	public override void Exit()
	{
		base.Exit();

		_ctx.PlayerInput.crouchEvent -= OnCrouch;
		_ctx.PlayerInput.runEvent -= OnRun;

		_ctx.PlayerSound.DisableStepsAudio();
	}

	private void OnCrouch()
	{
		_ctx.SwitchCurrentState(new PlayerCrouchState(_ctx));
	}

	private void OnRun()
	{
		_ctx.SwitchCurrentState(new PlayerRunningState(_ctx));
	}
}
