using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallingState : PlayerOnAirState
{
	public PlayerFallingState(PlayerStateMachine stateMachine) : base(stateMachine)
	{
	}

	public override void Enter()
	{
		Debug.Log("Falling State", _stateMachine);
	}

	public override void FixedTick(float fixedDeltaTime)
	{
		base.FixedTick(fixedDeltaTime);

		if (_stateMachine.IsGrounded())
		{
			_stateMachine.SwitchCurrentState(new PlayerIdleState(_stateMachine));
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
		_stateMachine.MovementVector = _stateMachine.InputVector * _stateMachine.MovementSpeed;

		//Moves the player
		_stateMachine.MainRigidbody.AddForce(_stateMachine.MovementVector * _stateMachine.MainRigidbody.mass, ForceMode.Force);
		ClampsHorizontalVelocity();
	}

	private void ClampsHorizontalVelocity()
	{
		Vector3 xzVel = new Vector3(_stateMachine.MainRigidbody.velocity.x, 0, _stateMachine.MainRigidbody.velocity.z);
		Vector3 yVel = new Vector3(0, _stateMachine.MainRigidbody.velocity.y, 0);

		xzVel = Vector3.ClampMagnitude(xzVel, _stateMachine.MaxHorizontalSpeed);

		_stateMachine.MainRigidbody.velocity = xzVel + yVel;
	}

	private void RotatePlayer()
	{
		//Rotate to the movement direction
		UpdateFowardOrientation(_stateMachine.MovementVector.normalized);
	}

	void UpdateFowardOrientation(Vector3 directionVector)
	{
		Quaternion targetRotation = Quaternion.LookRotation(directionVector, Vector3.up);
		_stateMachine.transform.rotation = Quaternion.Slerp(_stateMachine.transform.rotation, targetRotation, Time.fixedDeltaTime * _stateMachine.RotationSpeed);
	}
}
