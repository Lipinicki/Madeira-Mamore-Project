using UnityEngine;

public class PlayerJumpState : PlayerOnAirState
{
	public PlayerJumpState(PlayerStateMachine stateMachine) : base(stateMachine)
	{
	}

	public override void Enter()
	{
		_stateMachine.PlayerInput.jumpCanceledEvent += OnJumpcannceled;
		Debug.Log("Jumping State", _stateMachine);
	}

	public override void FixedTick(float fixedDeltaTime)
	{
		base.FixedTick(fixedDeltaTime);
		RaiseGravitycontribution(fixedDeltaTime);
		HandleJump();
		MovePlayer();
	}

	public override void Tick(float deltaTime)
	{

	}

	public override void Exit()
	{
		_stateMachine.PlayerInput.jumpCanceledEvent -= OnJumpcannceled;
	}

	private void RaiseGravitycontribution(float deltaTime)
	{
		//Raises gravity contribution starting from 0f at the beginning of the jump
		//and raise it to a maximun of 1f
		_stateMachine.GravityContribution += deltaTime * _stateMachine.GravityComeback;
	}

	private void HandleJump()
	{
		//The player can only hold the Jump button for so long as
		//JumpInputDuration, _jumpBeginTime is setted when OnJump is
		//called
		if (Time.time >= _stateMachine.JumpBeginTime + _stateMachine.JumpInputDuration)
		{
			_stateMachine.GravityContribution = 1f; //Gravity influence is reset to full effect
			_stateMachine.SwitchCurrentState(new PlayerFallingState(_stateMachine));
		}
		else
		{
			_stateMachine.GravityContribution *= _stateMachine.GravityDivider; //Reduce the gravity effect
		}
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

	private void OnJumpcannceled()
	{
		Debug.Log("Enter this block");
		_stateMachine.GravityContribution = 1f;
		_stateMachine.SwitchCurrentState(new PlayerFallingState(_stateMachine));
	}
}
