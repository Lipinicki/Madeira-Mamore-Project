using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLadderClimbState : PlayerBaseState
{
	private float facingDotThreshold = -0.9f;
	private float _startOffsetHeight = 0.8f;
	private float _startOffsetFoward = 0.8f;
	private float _forceToLeftLadder = 5f;

	public PlayerLadderClimbState(PlayerStateMachine stateMachine) : base(stateMachine)
	{
	}

	public override void Enter()
	{
		_stateMachine.PlayerInput.interactEvent += OnRelease;

		Vector3 offSetPos = new Vector3(_stateMachine.ActiveLadder.position.x, 0, _stateMachine.ActiveLadder.position.z);
		_stateMachine.transform.position = offSetPos + _stateMachine.ActiveLadder.forward * _startOffsetFoward + _stateMachine.transform.up * _startOffsetHeight;
		_stateMachine.transform.rotation = Quaternion.LookRotation(-_stateMachine.ActiveLadder.transform.forward);
	}

	public override void FixedTick(float fixedDeltaTime)
	{
		HandleLadderClimb(fixedDeltaTime);
		ClampsHorizontalVelocity();
	}

	public override void Tick(float deltaTime)
	{
		
	}

	public override void Exit()
	{
		_stateMachine.PlayerInput.interactEvent -= OnRelease;
	}

	private void HandleLadderClimb(float deltaTime)
	{
		if (_stateMachine.ActiveLadder == null)
		{
			ApplyForceToLeft();
			return;
		}

		Vector3 climbDirection = new Vector3(0f, _stateMachine.InputVector.z, 0f);
		Vector3 playerForward = _stateMachine.transform.forward;
		Vector3 ladderForward = _stateMachine.ActiveLadder.forward;

		if (CheckFacingVectors(playerForward.normalized, ladderForward.normalized))
		{
			_stateMachine.transform.Translate(climbDirection * _stateMachine.LadderClimbingSpeed * deltaTime);
		}

		if (_stateMachine.IsGrounded())
		{
			OnRelease();
		}
	}

	private bool CheckFacingVectors(Vector3 vectorA, Vector3 vectorB)
	{
		float facingDotProduct = Vector3.Dot(vectorA.normalized, vectorB.normalized);
		return (facingDotProduct <= facingDotThreshold);
	}

	private void ClampsHorizontalVelocity()
	{
		Vector3 xzVel = new Vector3(_stateMachine.MainRigidbody.velocity.x, 0, _stateMachine.MainRigidbody.velocity.z);
		Vector3 yVel = new Vector3(0, _stateMachine.MainRigidbody.velocity.y, 0);

		xzVel = Vector3.ClampMagnitude(xzVel, 0);

		_stateMachine.MainRigidbody.velocity = xzVel + yVel;
	}

	private void ApplyForceToLeft()
	{
		_stateMachine.ActiveLadder = null;
		_stateMachine.MainRigidbody.AddForce(_stateMachine.transform.up * _forceToLeftLadder + _stateMachine.transform.forward * _forceToLeftLadder, ForceMode.Impulse);
		_stateMachine.SwitchCurrentState(new PlayerFallingState(_stateMachine));
	}

	private void OnRelease()
	{
		_stateMachine.ActiveLadder = null;
		_stateMachine.SwitchCurrentState(new PlayerFallingState(_stateMachine));
	}
}
