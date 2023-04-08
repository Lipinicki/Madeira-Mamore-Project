using System.Collections;
using UnityEngine;

public class PlayerPushingState : PlayerOnGroundState
{
	private readonly int kWalkingAnimationParam = Animator.StringToHash("isWalking");
	private Rigidbody activeBlock => _stateMachine.ActiveBlock;
	private Transform transform => _stateMachine.transform;
	private Vector3 blockTargetPosition = Vector3.zero;

	public PlayerPushingState(PlayerStateMachine stateMachine) : base(stateMachine)
	{
	}

	public override void Enter()
	{
		base.Enter();

		_stateMachine.PlayerInput.interactEvent += Release;
		
		Debug.Log("Pusing State", _stateMachine);
		_stateMachine.PlayerSound.SetupStepsAudio();
		_stateMachine.MainAnimator.SetBool(kWalkingAnimationParam, true);
	}

	public override void FixedTick(float fixedDeltaTime)
	{
		MovePlayer();
		UpdateBlockPosition();
		base.FixedTick(fixedDeltaTime);
	}

	public override void Tick(float deltaTime)
	{
		base.Tick(deltaTime);
	}

	public override void Exit()
	{
		base.Exit();
		
		_stateMachine.PlayerSound.DisableStepsAudio();
		_stateMachine.MainAnimator.SetBool(kWalkingAnimationParam, false);
	}

	private void UpdateBlockPosition()
	{
		if (activeBlock == null || _stateMachine.InputVector == Vector3.zero) return;


		// Calculate the target position for the block based on the player's forward direction
		blockTargetPosition = transform.position + (transform.forward.normalized * _stateMachine.BlockOffset);
		
		float movingFactor = _stateMachine.BlockMovementSpeed * Time.fixedDeltaTime;
		Vector3 interpolatedPosition = Vector3.Lerp(activeBlock.transform.position, blockTargetPosition, movingFactor);

		_stateMachine.ActiveBlock.MovePosition(interpolatedPosition);
	}


	private void MovePlayer()
	{
		float reducedMovespeed = _stateMachine.MovementSpeed * 0.4f;
		_stateMachine.MovementVector = _stateMachine.InputVector.normalized * reducedMovespeed;

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

	private void Release()
	{
		_stateMachine.DetachBlock();
		_stateMachine.SwitchCurrentState(new PlayerIdleState(_stateMachine));
	}

}
