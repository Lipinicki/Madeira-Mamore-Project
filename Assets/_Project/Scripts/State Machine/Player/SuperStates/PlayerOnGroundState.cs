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
		ClampsVerticalVelocity(_ctx.MaxVerticalSpeed);
	}

	public override void Tick(float deltaTime)
	{

	}

	public override void Exit()
	{
		_ctx.PlayerInput.jumpEvent -= OnJump;
		_ctx.PlayerInput.interactEvent -= OnInteract;
	}

	protected void OnJump()
	{
		if (_ctx.MainAnimator == null)
		{
#if UNITY_EDITOR
			Debug.Log($"<color=red>Player rigidbody: {_ctx.MainRigidbody} is a NullReference. Skiping Jump this frame.</color>", _ctx);
#endif
			return;
		}

		_ctx.MainRigidbody.velocity += new Vector3(0, _ctx.InitialJumpForce, 0);
		_ctx.JumpBeginTime = Time.time; //Resets jump begin time
		_ctx.SwitchCurrentState(new PlayerJumpState(_ctx));
	}

	private void OnInteract()
	{
		if (_ctx.InteractableArea.CanInteract)
		{
			_ctx.InteractableArea.Interact();
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

			if (_ctx.ActiveBlock == null) return;
			_ctx.SwitchCurrentState(new PlayerPushingState(_ctx));
		}
	}
}
