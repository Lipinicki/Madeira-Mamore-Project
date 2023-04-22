using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLadderClimbState : PlayerBaseState
{
	public PlayerLadderClimbState(PlayerStateMachine stateMachine) : base(stateMachine)
	{
	}

	public override void Enter()
	{
		_ctx.PlayerInput.interactEvent += OnRelease;

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
		ClampsHorizontalVelocity();
	}

	public override void Tick(float deltaTime)
	{
		CheckForLadder();
	}

	private void CheckForLadder()
	{
		Ray ray = new Ray(
					new Vector3(
					_ctx.transform.position.x,
					_ctx.transform.position.y + _ctx.RayCastOffset,
					_ctx.transform.position.z
					),
					_ctx.transform.forward
					);

		if (Physics.Raycast(ray, out RaycastHit hit, _ctx.RayCastMaxDistance, _ctx.LadderLayers, QueryTriggerInteraction.Ignore))
		{
			_ctx.ActiveLadder = hit.transform;
		}
		else
		{
			_ctx.ActiveLadder = null;
		}
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

	private void ClampsHorizontalVelocity()
	{
		Vector3 xzVel = new Vector3(_ctx.MainRigidbody.velocity.x, 0, _ctx.MainRigidbody.velocity.z);
		Vector3 yVel = new Vector3(0, _ctx.MainRigidbody.velocity.y, 0);

		xzVel = Vector3.ClampMagnitude(xzVel, 0);

		_ctx.MainRigidbody.velocity = xzVel + yVel;
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
