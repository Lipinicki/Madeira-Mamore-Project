using UnityEngine;

public class PlayerJumpState : PlayerOnAirState
{
	private readonly int r_JumpAnimationState = Animator.StringToHash("FallingIdle");

	private const float k_AnimationTransitionTime = 0.15f;

	public PlayerJumpState(PlayerStateMachine stateMachine) : base(stateMachine)
	{
	}

	public override void Enter()
	{

		_ctx.PlayerInput.jumpCanceledEvent += OnJumpcannceled;
		_ctx.PlayerCollider.height = 0.8f;
		_ctx.MainAnimator.CrossFadeInFixedTime(r_JumpAnimationState, k_AnimationTransitionTime);
	}

	public override void FixedTick(float fixedDeltaTime)
	{
		base.FixedTick(fixedDeltaTime);
		RaiseGravitycontribution(fixedDeltaTime);
		HandleJump();
		MovePlayer(_ctx.MovementSpeed);
		ClampsHorizontalVelocity(_ctx.MaxHorizontalSpeed);
		RotatePlayerToForward();
	}

	public override void Tick(float deltaTime)
	{

	}

	public override void Exit()
	{
		_ctx.PlayerInput.jumpCanceledEvent -= OnJumpcannceled;
	}

	private void RaiseGravitycontribution(float deltaTime)
	{
		//Raises gravity contribution starting from 0f at the beginning of the jump
		//and raise it to a maximun of 1f
		_ctx.GravityContribution += deltaTime * _ctx.GravityComeback;
	}

	private void HandleJump()
	{
		//The player can only hold the Jump button for so long as
		//JumpInputDuration, _jumpBeginTime is setted when OnJump is
		//called
		if (Time.time >= _ctx.JumpBeginTime + _ctx.JumpInputDuration)
		{
			_ctx.GravityContribution = 1f; //Gravity influence is reset to full effect
			_ctx.SwitchCurrentState(new PlayerFallingState(_ctx));
		}
		else
		{
			_ctx.GravityContribution *= _ctx.GravityDivider; //Reduce the gravity effect
		}
	}

	private void OnJumpcannceled()
	{
		_ctx.GravityContribution = 1f;
		_ctx.SwitchCurrentState(new PlayerFallingState(_ctx));
	}
}
