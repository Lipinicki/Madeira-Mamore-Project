using System.Collections;
using UnityEngine;

public class PlayerPushingState : PlayerOnGroundState
{
	private readonly int kWalkingAnimationParam = Animator.StringToHash("isWalking");
	private Rigidbody activeBlock => _ctx.ActiveBlock;
	private Transform transform => _ctx.transform;
	private Vector3 blockTargetPosition = Vector3.zero;

	public PlayerPushingState(PlayerStateMachine stateMachine) : base(stateMachine)
	{
	}

	public override void Enter()
	{
		base.Enter();

		_ctx.PlayerInput.interactEvent += Release;
		
		Debug.Log("Pusing State", _ctx);
		_ctx.PlayerSound.SetupStepsAudio();
		_ctx.MainAnimator.SetBool(kWalkingAnimationParam, true);
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
		
		_ctx.PlayerSound.DisableStepsAudio();
		_ctx.MainAnimator.SetBool(kWalkingAnimationParam, false);
	}

	private void UpdateBlockPosition()
	{
		if (activeBlock == null || _ctx.InputVector == Vector3.zero) return;


		// Calculate the target position for the block based on the player's forward direction
		blockTargetPosition = transform.position + (transform.forward.normalized * _ctx.BlockOffset);
		
		float movingFactor = _ctx.BlockMovementSpeed * Time.fixedDeltaTime;
		Vector3 interpolatedPosition = Vector3.Lerp(activeBlock.transform.position, blockTargetPosition, movingFactor);

		_ctx.ActiveBlock.MovePosition(interpolatedPosition);
	}


	private void MovePlayer()
	{
		float reducedMovespeed = _ctx.MovementSpeed * 0.4f;
		_ctx.MovementVector = _ctx.InputVector.normalized * reducedMovespeed;

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

	private void Release()
	{
		_ctx.DetachBlock();
		_ctx.SwitchCurrentState(new PlayerIdleState(_ctx));
	}

}
