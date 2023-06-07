using UnityEngine;

public class PlayerRunningState : PlayerOnGroundState
{
	private readonly int r_RunningAnimationState = Animator.StringToHash("Running");

	private const float k_AnimationTransitionTime = 0.15f;

	public PlayerRunningState(PlayerStateMachine stateMachine) : base(stateMachine)
	{
	}

	public override void Enter()
	{
		base.Enter();

		// Test
		Debug.Log("Running State!");

		_ctx.PlayerInput.runCanceledEvent += OnRunCancel;

		_ctx.MainAnimator.CrossFadeInFixedTime(r_RunningAnimationState, k_AnimationTransitionTime);
	}

	public override void Tick(float deltaTime)
	{
		base.Tick(deltaTime);
	}

	public override void FixedTick(float fixedDeltaTime)
	{
		MovePlayer(_ctx.MovementSpeed);
		ClampsHorizontalVelocity(_ctx.MaxHorizontalSpeed);
		RotatePlayerToForward();

		base.FixedTick(fixedDeltaTime);
	}

	public override void Exit()
	{
		base.Exit();

		_ctx.PlayerInput.runCanceledEvent -= OnRunCancel;
	}

	private void OnRunCancel()
	{
		_ctx.SwitchCurrentState(new PlayerWalkingState(_ctx));
	}
}
