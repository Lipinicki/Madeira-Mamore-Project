using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchState : PlayerOnGroundState
{
	private float crouchingHeight = 0.8f;
	private float transitionSpeed = 10f;
	private float currentHeight;
	private Coroutine crouchRoutine;

	private readonly int r_StartCrouchingAnimationState = Animator.StringToHash("StandingToCrouch");
	private readonly int r_ExitCrouchingAnimationState = Animator.StringToHash("CrouchToStanding");
	private readonly int r_CrouchMotionParam = Animator.StringToHash("CrouchSpeed");

	private const float k_AnimationTransitionTime = 0.15f;
	private const float k_AnimatorDampTime = 0.1f;

	public PlayerCrouchState(PlayerStateMachine stateMachine) : base(stateMachine)
	{
	}

	public override void Enter()
	{
		base.Enter();

		Debug.Log("Crouching State");

		_ctx.PlayerInput.crouchCanceledEvent += ReleaseCrouch;

		currentHeight = _ctx.StandingHeight;

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

		if (_ctx.InputVector ==  Vector3.zero)
		{
			_ctx.MainAnimator.SetFloat(r_CrouchMotionParam, 0f, k_AnimatorDampTime, deltaTime);
			return;
		}

		_ctx.MainAnimator.SetFloat(r_CrouchMotionParam, 1f, k_AnimatorDampTime, deltaTime);
	}

	public override void Exit()
	{
		base.Exit();
		_ctx.PlayerInput.crouchCanceledEvent -= ReleaseCrouch;
	}

	private void StartCrouch()
	{
		if (crouchRoutine != null) _ctx.StopCoroutine(crouchRoutine);
		crouchRoutine = _ctx.StartCoroutine(CrouchCoroutine(crouchingHeight));
		_ctx.MainAnimator.CrossFadeInFixedTime(r_StartCrouchingAnimationState, k_AnimationTransitionTime);
	}

	private void ReleaseCrouch()
	{
		if (CanStand())
		{
			_ctx.StartCoroutine(ExitCrouchCoroutine());
		}
	}

	private bool CanStand()
	{
		Vector3 raycastOrigin = _ctx.transform.position + Vector3.up * (_ctx.PlayerCollider.height * 0.5f);
		return !Physics.Raycast(raycastOrigin, Vector3.up, out var hit, _ctx.StandingHeight - crouchingHeight);
	}

	private void MovePlayer()
	{
		_ctx.MovementVector = _ctx.InputVector * _ctx.MovementSpeed;

		//Moves the player
		_ctx.MainRigidbody.AddForce(_ctx.MovementVector * _ctx.MainRigidbody.mass, ForceMode.Force);
		ClampsHorizontalVelocity();
	}

	private void RotatePlayer()
	{
		//Rotate to the movement direction
		UpdateFowardOrientation(_ctx.MovementVector.normalized);
	}

	private void ClampsHorizontalVelocity()
	{
		Vector3 xzVel = new Vector3(_ctx.MainRigidbody.velocity.x, 0, _ctx.MainRigidbody.velocity.z);
		Vector3 yVel = new Vector3(0, _ctx.MainRigidbody.velocity.y, 0);

		xzVel = Vector3.ClampMagnitude(xzVel, _ctx.MaxHorizontalSpeed);

		_ctx.MainRigidbody.velocity = xzVel + yVel;
	}

	void UpdateFowardOrientation(Vector3 directionVector)
	{
		if (directionVector == Vector3.zero) return;

		Quaternion targetRotation = Quaternion.LookRotation(directionVector, Vector3.up);
		_ctx.transform.rotation = Quaternion.Slerp(_ctx.transform.rotation, targetRotation, Time.fixedDeltaTime * _ctx.RotationSpeed);
	}

	private IEnumerator CrouchCoroutine(float targetHeight)
	{
		if (_ctx.PlayerCollider.height == targetHeight) yield break;

		while (Mathf.Abs(currentHeight - targetHeight) > 0.01f)
		{
			var crouchDelta = Time.deltaTime * transitionSpeed;
			currentHeight = Mathf.Lerp(currentHeight, targetHeight, crouchDelta);
			_ctx.PlayerCollider.height = currentHeight;
			yield return null;
		}
		currentHeight = targetHeight;
	}

	private IEnumerator ExitCrouchCoroutine()
	{
		if (crouchRoutine != null) _ctx.StopCoroutine(crouchRoutine);
		crouchRoutine = _ctx.StartCoroutine(CrouchCoroutine(_ctx.StandingHeight));
		_ctx.MainAnimator.CrossFadeInFixedTime(r_ExitCrouchingAnimationState, k_AnimationTransitionTime);

		while (_ctx.MainAnimator.IsInTransition(0)) yield return null;

		_ctx.SwitchCurrentState(new PlayerIdleState(_ctx));
	}
}
