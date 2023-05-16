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

		_ctx.PlayerSound.SetupStepsAudio();
		_ctx.MainAnimator.CrossFadeInFixedTime(r_WalkingAnimationState, k_AnimationTransitionTime);
	}

	public override void FixedTick(float fixedDeltaTime)
	{
		MovePlayer();

		base.FixedTick(fixedDeltaTime);
		RotatePlayer();
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
		
		_ctx.PlayerSound.DisableStepsAudio();
	}

	private void MovePlayer()
	{
		_ctx.MovementVector = _ctx.InputVector.normalized * _ctx.MovementSpeed;

		//Moves the player
		_ctx.MainRigidbody.AddForce(_ctx.MovementVector * _ctx.MainRigidbody.mass, ForceMode.Force);
		ClampsHorizontalVelocity();
	}

	private void RotatePlayer()
	{
		//Rotate to the movement direction
		UpdateFowardOrientation(_ctx.MovementVector.normalized);
	}

	private void ClampsHorizontalVelocity()
	{
		Vector3 xzVel = new Vector3(_ctx.MainRigidbody.velocity.x, 0, _ctx.MainRigidbody.velocity.z);
		Vector3 yVel = new Vector3(0, _ctx.MainRigidbody.velocity.y, 0);

		xzVel = Vector3.ClampMagnitude(xzVel, _ctx.MaxHorizontalSpeed);

		_ctx.MainRigidbody.velocity = xzVel + yVel;
	}

	void UpdateFowardOrientation(Vector3 directionVector)
	{
		Quaternion targetRotation = Quaternion.LookRotation(directionVector, Vector3.up);
		_ctx.transform.rotation = Quaternion.Slerp(_ctx.transform.rotation, targetRotation, Time.fixedDeltaTime * _ctx.RotationSpeed);
	}

	private void OnCrouch()
	{
		_ctx.SwitchCurrentState(new PlayerCrouchState(_ctx));
	}
}
