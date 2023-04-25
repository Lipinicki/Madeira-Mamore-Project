using System.Collections;
using UnityEngine;

public class PlayerPushingState : PlayerOnGroundState
{
	private Rigidbody activeBlock => _ctx.ActiveBlock;
	private Transform transform => _ctx.transform;
	private Vector3 blockTargetPosition = Vector3.zero;

	private readonly int r_PushHorizontalAnimationParam = Animator.StringToHash("PushHorizontalSpeed");
	private readonly int r_PushVerticalAnimationParam = Animator.StringToHash("PushVerticalSpeed");
	private readonly int r_PushBlockBlendTree = Animator.StringToHash("LowerPushingBlockBlendTree");

	private const int k_UpperBodyAnimatorLayer = 1;
	private const float k_AnimationTransitionTime = 0.25f;
	private const float k_PushAnimationDamp = .1f;

	public PlayerPushingState(PlayerStateMachine stateMachine) : base(stateMachine)
	{
	}

	public override void Enter()
	{
		Debug.Log("Pushing State", _ctx);

		base.Enter();

		_ctx.PlayerInput.interactEvent += Release;
		
		_ctx.PlayerSound.SetupStepsAudio();

		// Turn on UpperBody animations for pushing the block
		_ctx.MainAnimator.SetLayerWeight(k_UpperBodyAnimatorLayer, 1f);
		_ctx.MainAnimator.CrossFadeInFixedTime(r_PushBlockBlendTree, k_AnimationTransitionTime);
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

		SetLowerBodyAnimations(deltaTime);
	}

	public override void Exit()
	{
		base.Exit();

		// Turn off UpperBody animations
		_ctx.MainAnimator.SetLayerWeight(k_UpperBodyAnimatorLayer, 0f);

		_ctx.PlayerSound.DisableStepsAudio();
	}

	// Translates the block position to follow the player
	private void UpdateBlockPosition(float deltaTime)
	{
		if (activeBlock == null || _ctx.InputVector == Vector3.zero) return;

		// Calculate the target position for the block based on the player's forward direction
		blockTargetPosition = new Vector3(transform.position.x, activeBlock.transform.position.y, transform.position.z) 
			+ (transform.forward.normalized * _ctx.BlockOffset);

		_ctx.ActiveBlock.MovePosition(blockTargetPosition);
	}

	// Function used to move player inside this state
	private void MovePlayer()
	{
		float reducedMovespeed = _ctx.MovementSpeed * 0.8f;
		_ctx.MovementVector = _ctx.InputVector.normalized * reducedMovespeed;

		//Moves the player
		_ctx.MainRigidbody.AddForce(_ctx.MovementVector * _ctx.MainRigidbody.mass, ForceMode.Force);
		ClampsHorizontalVelocity();
	}

	// Clamps velocity so the player don't go faster when moving on diagonals
	private void ClampsHorizontalVelocity()
	{
		Vector3 xzVel = new Vector3(_ctx.MainRigidbody.velocity.x, 0, _ctx.MainRigidbody.velocity.z);
		Vector3 yVel = new Vector3(0, _ctx.MainRigidbody.velocity.y, 0);

		xzVel = Vector3.ClampMagnitude(xzVel, _ctx.MaxHorizontalSpeed);

		_ctx.MainRigidbody.velocity = xzVel + yVel;
	}

	// Release the block and switch states
	private void Release()
	{
		_ctx.DetachBlock();
		_ctx.SwitchCurrentState(new PlayerIdleState(_ctx));
	}

	// Controls the lower body legs animations
	private void SetLowerBodyAnimations(float deltaTime)
	{
		Vector3 input = _ctx.PlayerInput.RawMovementInput;

		// Sets horizontal (x) movement values
		if (input.x == 0)
		{
			_ctx.MainAnimator.SetFloat(r_PushHorizontalAnimationParam, 0, k_PushAnimationDamp, deltaTime);
		}
		else
		{
			float xValue = input.x > 0 ? 1f : -1f;
			_ctx.MainAnimator.SetFloat(r_PushHorizontalAnimationParam, xValue, k_PushAnimationDamp, deltaTime);
		}

		// Sets vertical (z) values
		if (input.z == 0)
		{
			_ctx.MainAnimator.SetFloat(r_PushVerticalAnimationParam, 0, k_PushAnimationDamp, deltaTime);
		}
		else
		{
			float yValue = input.z > 0 ? 1f : -1f;
			_ctx.MainAnimator.SetFloat(r_PushVerticalAnimationParam, yValue, k_PushAnimationDamp, deltaTime);
		}
	}

}
