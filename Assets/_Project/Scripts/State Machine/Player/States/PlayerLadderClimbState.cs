using UnityEngine;

public class PlayerLadderClimbState : PlayerBaseState
{
	private readonly int r_ClimbingAnimationBlendTree = Animator.StringToHash("ClimbingLadderBlendTree");
	private readonly int r_ClimbingAnimationParam = Animator.StringToHash("ClimbingSpeed");

	private const float k_AnimationTransitionTime = 0.25f;
	private const float k_AnimatorDampTime = 0.15f;

	public PlayerLadderClimbState(PlayerStateMachine stateMachine) : base(stateMachine)
	{
	}

	public override void Enter()
	{
		_ctx.PlayerInput.interactEvent += OnRelease;

		_ctx.InteractableArea.NotifyInteractionCancel(); // Notifies prompts

		_ctx.MainAnimator.CrossFadeInFixedTime(r_ClimbingAnimationBlendTree, k_AnimationTransitionTime);

		Vector3 offSetPos = new Vector3(
			_ctx.ActiveLadder.position.x,
			_ctx.transform.position.y,
			_ctx.ActiveLadder.position.z
			);
		_ctx.transform.position = offSetPos +
			_ctx.ActiveLadder.forward *
			_ctx.LadderStartOffsetFoward +
			_ctx.transform.up *
			_ctx.LadderStartOffsetHeight;
		_ctx.transform.rotation = Quaternion.LookRotation(-_ctx.ActiveLadder.transform.forward);
	}

	public override void FixedTick(float fixedDeltaTime)
	{
		HandleLadderClimb(fixedDeltaTime);
		ClampsHorizontalVelocity(0f);
	}

	public override void Tick(float deltaTime)
	{
		CheckForLadder();
		SetupAnimation(deltaTime);
	}

	public override void Exit()
	{
		_ctx.PlayerInput.interactEvent -= OnRelease;
	}

	private void HandleLadderClimb(float deltaTime)
	{
		if (_ctx.ActiveLadder == null)
		{
			ApplyForceToLeft(_ctx.transform.up *
			_ctx.ForceToLeftLadder +
			_ctx.transform.forward *
			_ctx.ForceToLeftLadder);
			return;
		}

		Vector3 climbDirection = new Vector3(0f, _ctx.PlayerInput.RawMovementInput.z, 0f);

		_ctx.transform.Translate(climbDirection * _ctx.LadderClimbingSpeed * deltaTime);

		if (_ctx.IsGrounded())
		{
			OnRelease();
		}
	}

	// Controls animations based on input
	private void SetupAnimation(float deltaTime)
	{
		if (_ctx.PlayerInput.RawMovementInput.z == 0f)
		{
			_ctx.MainAnimator.SetFloat(r_ClimbingAnimationParam, 0f, k_AnimatorDampTime, deltaTime);
		}
		else
		{
			float zValue = _ctx.PlayerInput.RawMovementInput.z > 0 ? 1f : -1f;
			_ctx.MainAnimator.SetFloat(r_ClimbingAnimationParam, zValue, k_AnimatorDampTime, deltaTime);
		}
		
	}

	private void ApplyForceToLeft(Vector3 force)
	{
		_ctx.MainRigidbody.AddForce(force, ForceMode.Impulse);
		_ctx.SwitchCurrentState(new PlayerFallingState(_ctx));
	}

	private void OnRelease()
	{
		_ctx.ActiveLadder = null;
		ApplyForceToLeft(_ctx.transform.up * _ctx.ForceToLeftLadder);
	}
}
