using System.Collections;
using UnityEngine;

public class PlayerPushingState : PlayerOnGroundState
{
	private BasicPullPushBlock ActiveBlock = null;

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
		base.Enter();

		_ctx.PlayerInput.interactEvent += Release;
		ActiveBlock = _ctx.ActiveBlock;

		// dinamically set the rigidBody contraints
		ActiveBlock.MainRigidBody.constraints = RigidbodyConstraints.None;
		ActiveBlock.MainRigidBody.constraints = RigidbodyConstraints.FreezeRotation;

		// add a tiny impulse to reposition the block
		Vector3 offsetDirection = ActiveBlock.transform.position - _ctx.transform.position;
		offsetDirection.y = 0;
		ActiveBlock.MainRigidBody.AddForceAtPosition(offsetDirection * 0.1f, _ctx.transform.position, ForceMode.Impulse);

		// Turn on UpperBody animations for pushing the block
		_ctx.MainAnimator.SetLayerWeight(k_UpperBodyAnimatorLayer, 1f);
		_ctx.MainAnimator.CrossFadeInFixedTime(r_PushBlockBlendTree, k_AnimationTransitionTime);
	}

	public override void FixedTick(float fixedDeltaTime)
	{
		UpdateBlockPosition(fixedDeltaTime);
		MovePlayer();
		
		base.FixedTick(fixedDeltaTime);
		
		// stop block movement if the raw input is zero
		if (_ctx.PlayerInput.RawMovementInput == Vector3.zero)
		{
			ActiveBlock.MainRigidBody.velocity = Vector3.zero;
			ActiveBlock.StopAudio();
		}
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
		if (ActiveBlock == null || _ctx.InputVector == Vector3.zero) return;
		
		// release block if player strays too far from it
		float currentBlockDistance = Vector3.Distance(ActiveBlock.transform.position, _ctx.transform.position);
		if (currentBlockDistance > _ctx.MaxInteractionDistance)
		{ 
			Release();
			return;
		}

		// moves the block based on the player velocity
		ActiveBlock.MainRigidBody.velocity = _ctx.MainRigidbody.velocity;
		ActiveBlock.PlayAudio();
	}

	// Function used to move player inside this state
	private void MovePlayer()
	{
		float reducedMovespeed = _ctx.MovementSpeed * 0.55f;
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
		DetachBlock();
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

			_ctx.PlayerSound.SetupStepsAudio();
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

			_ctx.PlayerSound.SetupStepsAudio();
		}

		if (input.z == 0 && input.x == 0)
		{
			_ctx.PlayerSound.DisableStepsAudio();
		}
	}
	
	// release the current block a reset it's variables
	public void DetachBlock()
	{
		if (_ctx.ActiveBlock != null)
		{
			ActiveBlock.StopAudio();
			ActiveBlock.MainRigidBody.velocity = Vector3.zero;
			ActiveBlock.MainRigidBody.angularVelocity = Vector3.zero;
			ActiveBlock.MainRigidBody.constraints = RigidbodyConstraints.FreezeAll;	
			_ctx.MaxInteractionDistance = 0f;
			_ctx.ActiveBlock = null;
		}
	}
}
