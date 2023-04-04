using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerOnGroundState : PlayerBaseState
{
	protected PlayerOnGroundState(PlayerStateMachine stateMachine) : base(stateMachine)
	{
	}

	public override void Enter()
	{
		_stateMachine.PlayerInput.jumpEvent += OnJump;
		_stateMachine.PlayerInput.crouchEvent += OnCrouch;
		_stateMachine.PlayerInput.interactEvent += OnInteract;
	}

	public override void FixedTick(float fixedDeltaTime)
	{
		if (!_stateMachine.IsGrounded())
		{
			_stateMachine.SwitchCurrentState(new PlayerFallingState(_stateMachine));
		}

		_stateMachine.ApplyGravity();
		ClampsVerticalVelocity();
	}

	public override void Tick(float deltaTime)
	{
		CheckForLadder();

	}

	public override void Exit()
	{
		_stateMachine.PlayerInput.jumpEvent -= OnJump;
		_stateMachine.PlayerInput.crouchEvent -= OnCrouch;
		_stateMachine.PlayerInput.interactEvent -= OnInteract;
	}

	private void CheckForLadder()
	{
		Ray ray = new Ray(
					new Vector3(
					_stateMachine.transform.position.x,
					_stateMachine.transform.position.y + _stateMachine.RayCastOffset,
					_stateMachine.transform.position.z
					),
					_stateMachine.transform.forward
					);

		if (Physics.Raycast(ray, out RaycastHit hit, _stateMachine.RayCastMaxDistance, _stateMachine.LadderLayers, QueryTriggerInteraction.Ignore))
		{
			Debug.Log("Found Ladder: " + hit.transform.name);
			_stateMachine.ActiveLadder = hit.transform;
		}
		else
		{
			_stateMachine.ActiveLadder = null;
		}
	}

	private void ClampsVerticalVelocity()
	{
		Vector3 xzVel = new Vector3(_stateMachine.MainRigidbody.velocity.x, 0, _stateMachine.MainRigidbody.velocity.z);
		Vector3 yVel = new Vector3(0, _stateMachine.MainRigidbody.velocity.y, 0);

		yVel = Vector3.ClampMagnitude(yVel, _stateMachine.MaxVerticalSpeed);

		_stateMachine.MainRigidbody.velocity = xzVel + yVel;
	}

	private void OnJump()
	{
		_stateMachine.MainRigidbody.velocity += new Vector3(0, _stateMachine.InitialJumpForce, 0);
		_stateMachine.JumpBeginTime = Time.time; //Resets jump begin time
		_stateMachine.SwitchCurrentState(new PlayerJumpState(_stateMachine));
	}

	private void OnCrouch()
	{
		_stateMachine.SwitchCurrentState(new PlayerCrouchState(_stateMachine));
	}

	private void OnInteract()
	{
		if (_stateMachine.InteractableArea.CanInteract)
		{
			_stateMachine.InteractableArea.Interaction.Interact();
			_stateMachine.TriggerInteractiobnEvent();
		}

		if (_stateMachine.ActiveLadder != null)
		{
			_stateMachine.SwitchCurrentState(new PlayerLadderClimbState(_stateMachine));
		}
	}
}
