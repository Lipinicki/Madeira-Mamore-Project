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
		Debug.Log("Falling State", _ctx);

		_ctx.MainAnimator.CrossFadeInFixedTime(r_FallingIdleAnimatorState, k_AnimationTransitionTime);
	}

	public override void FixedTick(float fixedDeltaTime)
	{
		base.FixedTick(fixedDeltaTime);

		if (_ctx.IsGrounded())
		{
			_ctx.SwitchCurrentState(new PlayerIdleState(_ctx));
		}

		MovePlayer();
		RotatePlayer();
	}

	public override void Tick(float deltaTime)
	{

	}

	public override void Exit()
	{
		
	}

	private void MovePlayer()
	{
		_ctx.MovementVector = _ctx.InputVector * _ctx.MovementSpeed;

		//Moves the player
		_ctx.MainRigidbody.AddForce(_ctx.MovementVector * _ctx.MainRigidbody.mass, ForceMode.Force);
		ClampsHorizontalVelocity();
	}

	private void ClampsHorizontalVelocity()
	{
		Vector3 xzVel = new Vector3(_ctx.MainRigidbody.velocity.x, 0, _ctx.MainRigidbody.velocity.z);
		Vector3 yVel = new Vector3(0, _ctx.MainRigidbody.velocity.y, 0);

		xzVel = Vector3.ClampMagnitude(xzVel, _ctx.MaxHorizontalSpeed);

		_ctx.MainRigidbody.velocity = xzVel + yVel;
	}

	private void RotatePlayer()
	{
		//Rotate to the movement direction
		UpdateFowardOrientation(_ctx.MovementVector.normalized);
	}

	void UpdateFowardOrientation(Vector3 directionVector)
	{
		if (directionVector == Vector3.zero) return;

		Quaternion targetRotation = Quaternion.LookRotation(directionVector, Vector3.up);
		_ctx.transform.rotation = Quaternion.Slerp(_ctx.transform.rotation, targetRotation, Time.fixedDeltaTime * _ctx.RotationSpeed);
	}
}
