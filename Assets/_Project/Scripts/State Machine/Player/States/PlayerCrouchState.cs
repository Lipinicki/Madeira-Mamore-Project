using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchState : PlayerOnGroundState
{
	private float crouchingHeight = 0.8f;
	private float transitionSpeed = 10f;

	private float currentHeight;
	private Coroutine crouchRoutine;

	public PlayerCrouchState(PlayerStateMachine stateMachine) : base(stateMachine)
	{
	}

	public override void Enter()
	{
		base.Enter();

		_stateMachine.PlayerInput.crouchEvent += ReleaseCrouch;

		currentHeight = _stateMachine.StandingHeight;

		StartCrouch();
	}

	public override void FixedTick(float fixedDeltaTime)
	{
		base.FixedTick(fixedDeltaTime);

		MovePlayer();
		RotatePlayer();
	}

	public override void Tick(float deltaTime)
	{
		base.Tick(deltaTime);
	}

	public override void Exit()
	{
		base.Exit();
		_stateMachine.StopCoroutine(crouchRoutine);
		_stateMachine.PlayerInput.crouchEvent -= ReleaseCrouch;
	}

	private void StartCrouch()
	{
		if (crouchRoutine != null) _stateMachine.StopCoroutine(crouchRoutine);
		crouchRoutine = _stateMachine.StartCoroutine(CrouchCoroutine(crouchingHeight));
	}

	private void ReleaseCrouch()
	{
		if (CanStand())
		{
			if (crouchRoutine != null) _stateMachine.StopCoroutine(crouchRoutine);
			crouchRoutine = _stateMachine.StartCoroutine(CrouchCoroutine(_stateMachine.StandingHeight));
			_stateMachine.SwitchCurrentState(new PlayerIdleState(_stateMachine));
		}
	}

	private bool CanStand()
	{
		Vector3 raycastOrigin = _stateMachine.transform.position + Vector3.up * (_stateMachine.PlayerCollider.height / 2);
		return !Physics.Raycast(raycastOrigin, Vector3.up, out var hit, _stateMachine.StandingHeight - crouchingHeight);
	}

	private void MovePlayer()
	{
		_stateMachine.MovementVector = _stateMachine.InputVector * _stateMachine.MovementSpeed;

		//Moves the player
		_stateMachine.MainRigidbody.AddForce(_stateMachine.MovementVector * _stateMachine.MainRigidbody.mass, ForceMode.Force);
		ClampsHorizontalVelocity();
	}

	private void RotatePlayer()
	{
		//Rotate to the movement direction
		UpdateFowardOrientation(_stateMachine.MovementVector.normalized);
	}

	private void ClampsHorizontalVelocity()
	{
		Vector3 xzVel = new Vector3(_stateMachine.MainRigidbody.velocity.x, 0, _stateMachine.MainRigidbody.velocity.z);
		Vector3 yVel = new Vector3(0, _stateMachine.MainRigidbody.velocity.y, 0);

		xzVel = Vector3.ClampMagnitude(xzVel, _stateMachine.MaxHorizontalSpeed);

		_stateMachine.MainRigidbody.velocity = xzVel + yVel;
	}

	void UpdateFowardOrientation(Vector3 directionVector)
	{
		Quaternion targetRotation = Quaternion.LookRotation(directionVector, Vector3.up);
		_stateMachine.transform.rotation = Quaternion.Slerp(_stateMachine.transform.rotation, targetRotation, Time.fixedDeltaTime * _stateMachine.RotationSpeed);
	}

	private IEnumerator CrouchCoroutine(float targetHeight)
	{
		var initalTarget = targetHeight;
		while (Mathf.Abs(currentHeight - targetHeight) > 0.01f)
		{
			var crouchDelta = Time.deltaTime * transitionSpeed;
			currentHeight = Mathf.Lerp(currentHeight, targetHeight, crouchDelta);
			_stateMachine.PlayerCollider.height = currentHeight;
			yield return null;
		}
		currentHeight = initalTarget;
	}
}
