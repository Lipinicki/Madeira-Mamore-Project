using System.Collections;
using UnityEngine;

public class PlayerWalkingState : PlayerOnGroundState
{
	private readonly int kWalkingAnimationParam = Animator.StringToHash("isWalking");

	public PlayerWalkingState(PlayerStateMachine stateMachine) : base(stateMachine)
	{
	}

	public override void Enter()
	{
		base.Enter();

		_stateMachine.PlayerInput.crouchEvent += OnCrouch;

		Debug.Log("Walking State", _stateMachine);
		_stateMachine.PlayerSound.SetupStepsAudio();
		_stateMachine.MainAnimator.SetBool(kWalkingAnimationParam, true);
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

		if (_stateMachine.InputVector == Vector3.zero)
		{
			_stateMachine.SwitchCurrentState(new PlayerIdleState(_stateMachine));
		}
	}

	public override void Exit()
	{
		base.Exit();

		_stateMachine.PlayerInput.crouchEvent -= OnCrouch;
		
		_stateMachine.PlayerSound.DisableStepsAudio();
		_stateMachine.MainAnimator.SetBool(kWalkingAnimationParam, false);
	}

	private void MovePlayer()
	{
		_stateMachine.MovementVector = _stateMachine.InputVector.normalized * _stateMachine.MovementSpeed;

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

	private void OnCrouch()
	{
		_stateMachine.SwitchCurrentState(new PlayerCrouchState(_stateMachine));
	}
}
