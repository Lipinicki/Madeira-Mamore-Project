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
		UpdateBlockPosition(fixedDeltaTime);
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

	private void UpdateBlockPosition(float deltaTime)
	{
		if (activeBlock == null || _ctx.InputVector == Vector3.zero) return;

		// Calculate the target position for the block based on the player's forward direction
		blockTargetPosition = new Vector3(transform.position.x, activeBlock.transform.position.y, transform.position.z) 
			+ (transform.forward.normalized * _ctx.BlockOffset);

		_ctx.ActiveBlock.MovePosition(blockTargetPosition);
	}


	private void MovePlayer()
	{
		float reducedMovespeed = _ctx.MovementSpeed * 0.8f;
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
