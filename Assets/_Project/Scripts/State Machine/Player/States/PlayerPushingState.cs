using System.Collections;
using UnityEngine;

public class PlayerPushingState : PlayerOnGroundState
{
	private BasicPullPushBlock ActiveBlock = null; // WARNING: this can leads to cache invalidation, maybe try using _ctx reference
	private float reducedMovespeed;

	private readonly int r_PushHorizontalAnimationParam = Animator.StringToHash("PushHorizontalSpeed");
	private readonly int r_PushVerticalAnimationParam = Animator.StringToHash("PushVerticalSpeed");
	private readonly int r_PushBlockBlendTree = Animator.StringToHash("LowerPushingBlockBlendTree");

	private const int k_UpperBodyAnimatorLayer = 1;
	private const float k_AnimationTransitionTime = 0.25f;
	private const float k_PushAnimationDamp = .1f;

	public PlayerPushingState(PlayerStateMachine stateMachine) : base(stateMachine)
	{
		reducedMovespeed = _ctx.MovementSpeed * 0.55f;
	}

	public override void Enter()
	{
		_ctx.PlayerInput.interactEvent += Release;
		_ctx.PlayerInput.jumpEvent += OnJump; // overrides base enter to not subscribe Interact event

		ActiveBlock = _ctx.ActiveBlock;

		Debug.Log(ActiveBlock, _ctx);
		// dinamically set the rigidBody contraints
		ActiveBlock.MainRigidBody.constraints = RigidbodyConstraints.None;
		ActiveBlock.MainRigidBody.constraints = RigidbodyConstraints.FreezeRotation;

		// sets the collision to dinamic calculations
		ActiveBlock.MainRigidBody.isKinematic = false;

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
		MovePlayer(reducedMovespeed);
		ClampsHorizontalVelocity(_ctx.MaxHorizontalSpeed);
		
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
		_ctx.PlayerInput.interactEvent -= Release;
		_ctx.PlayerInput.jumpEvent -= OnJump;

		// Turn off UpperBody animations
		_ctx.MainAnimator.SetLayerWeight(k_UpperBodyAnimatorLayer, 0f);

		_ctx.PlayerSound.DisableStepsAudio();

		DetachBlock();
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

	// Release the block and switch states
	private void Release()
	{
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
			ActiveBlock.MainRigidBody.isKinematic = true;
			_ctx.MaxInteractionDistance = 0f;
			_ctx.ActiveBlock = null;
		}
	}
}
