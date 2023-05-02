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
		_ctx.PlayerInput.jumpEvent += OnJump;
		_ctx.PlayerInput.interactEvent += OnInteract;
	}

	public override void FixedTick(float fixedDeltaTime)
	{
		// Player is not on ground
		if (!_ctx.IsGrounded())
		{
			_ctx.SwitchCurrentState(new PlayerFallingState(_ctx));
			return;
		}

		_ctx.ApplyGravity();
		ClampsVerticalVelocity();
	}

	public override void Tick(float deltaTime)
	{

	}

	public override void Exit()
	{
		_ctx.PlayerInput.jumpEvent -= OnJump;
		_ctx.PlayerInput.interactEvent -= OnInteract;
	}

	private bool CheckForLadder()
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
			return true;
		}
		else
		{
			_ctx.ActiveLadder = null;
			return false;
		}
	}

	private void ClampsVerticalVelocity()
	{
		Vector3 xzVel = new Vector3(_ctx.MainRigidbody.velocity.x, 0, _ctx.MainRigidbody.velocity.z);
		Vector3 yVel = new Vector3(0, _ctx.MainRigidbody.velocity.y, 0);

		yVel = Vector3.ClampMagnitude(yVel, _ctx.MaxVerticalSpeed);

		_ctx.MainRigidbody.velocity = xzVel + yVel;
	}

	private void OnJump()
	{
		_ctx.MainRigidbody.velocity += new Vector3(0, _ctx.InitialJumpForce, 0);
		_ctx.JumpBeginTime = Time.time; //Resets jump begin time
		_ctx.SwitchCurrentState(new PlayerJumpState(_ctx));
	}

	private void OnInteract()
	{
		if (_ctx.InteractableArea.CanInteract)
		{
			_ctx.InteractableArea.Interaction.Interact();
			_ctx.TriggerInteractionEvent();
		}
		else if (CheckForLadder())
		{
			_ctx.SwitchCurrentState(new PlayerLadderClimbState(_ctx));
		}
		else if (Physics.Raycast(_ctx.transform.position, _ctx.transform.forward, out var grabHit, _ctx.RayCastMaxDistance, _ctx.PushBlocksLayer))
		{
			Debug.Log(grabHit.transform.name, grabHit.transform);
			SetupActiveBlock(grabHit.transform);
			_ctx.SwitchCurrentState(new PlayerPushingState(_ctx));
		}
	}

	public void SetupActiveBlock(Transform t)
	{
		BasicPullPushBlock block = t.GetComponent<BasicPullPushBlock>();
		float blockDepth = block.MainCollider.bounds.size.z;
		
		_ctx.MaxInteractionDistance = blockDepth;
		_ctx.ActiveBlock = block;
	}
}
