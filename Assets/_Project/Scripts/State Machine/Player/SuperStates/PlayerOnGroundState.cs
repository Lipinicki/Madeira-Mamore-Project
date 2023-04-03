using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerOnGroundState : PlayerBaseState
{
	protected PlayerOnGroundState(PlayerStateMachine stateMachine) : base(stateMachine)
	{
	}

	public override void Enter()
	{
		_stateMachine.PlayerInput.jumpEvent += OnJump;
	}

	public override void FixedTick(float fixedDeltaTime)
	{
		if (!_stateMachine.IsGrounded())
		{
			_stateMachine.SwitchCurrentState(new PlayerFallingState(_stateMachine));
		}

		_stateMachine.ApplyGravity();
		ClampsVerticalVelocity();
	}

	public override void Exit()
	{
		_stateMachine.PlayerInput.jumpEvent -= OnJump;
	}

	private void ClampsVerticalVelocity()
	{
		Vector3 xzVel = new Vector3(_stateMachine.MainRigidbody.velocity.x, 0, _stateMachine.MainRigidbody.velocity.z);
		Vector3 yVel = new Vector3(0, _stateMachine.MainRigidbody.velocity.y, 0);

		yVel = Vector3.ClampMagnitude(yVel, _stateMachine.MaxVerticalSpeed);

		_stateMachine.MainRigidbody.velocity = xzVel + yVel;
	}

	protected void OnJump()
	{
		_stateMachine.MainRigidbody.velocity += new Vector3(0, _stateMachine.InitialJumpForce, 0);
		_stateMachine.JumpBeginTime = Time.time; //Resets jump begin time
		_stateMachine.SwitchCurrentState(new PlayerJumpState(_stateMachine));
	}
}
