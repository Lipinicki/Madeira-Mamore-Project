using UnityEngine;

public class PlayerIdleState : PlayerOnGroundState
{
	private readonly int r_IdleAnimationState = Animator.StringToHash("Idle");

	private const float k_AnimationTransitionTime = 0.15f;

	public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine)
	{
	}

	public override void Enter()
	{
		base.Enter();

		_ctx.PlayerInput.crouchEvent += OnCrouch;

		_ctx.MainAnimator.CrossFadeInFixedTime(r_IdleAnimationState, k_AnimationTransitionTime);
	}

	public override void FixedTick(float fixedDeltaTime)
	{
		base.FixedTick(fixedDeltaTime);
	}

	public override void Tick(float deltaTime)
	{
		base.Tick(deltaTime);

		if (_ctx.InputVector.sqrMagnitude > 0.02f)
		{
			_ctx.SwitchCurrentState(new PlayerWalkingState(_ctx));
		}
	}

	public override void Exit()
	{
		base.Exit();

		_ctx.PlayerInput.crouchEvent -= OnCrouch;
	}

	private void OnCrouch()
	{
		_ctx.SwitchCurrentState(new PlayerCrouchState(_ctx));
	}
}
