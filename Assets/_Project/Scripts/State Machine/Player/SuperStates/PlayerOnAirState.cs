using UnityEngine;

public abstract class PlayerOnAirState : PlayerBaseState
{
	protected PlayerOnAirState(PlayerStateMachine stateMachine) : base(stateMachine)
	{
	}

	public override void FixedTick(float fixedDeltaTime)
	{
		_ctx.ApplyGravity();
		ClampsVerticalVelocity(_ctx.MaxVerticalSpeed);
	}
}
