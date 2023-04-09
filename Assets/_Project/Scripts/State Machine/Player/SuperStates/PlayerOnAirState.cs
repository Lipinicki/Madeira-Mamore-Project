using UnityEngine;

public abstract class PlayerOnAirState : PlayerBaseState
{
	protected PlayerOnAirState(PlayerStateMachine stateMachine) : base(stateMachine)
	{
	}

	public override void FixedTick(float fixedDeltaTime)
	{
		_ctx.ApplyGravity();
		ClampsVerticalVelocity();
	}

	private void ClampsVerticalVelocity()
	{
		Vector3 xzVel = new Vector3(_ctx.MainRigidbody.velocity.x, 0, _ctx.MainRigidbody.velocity.z);
		Vector3 yVel = new Vector3(0, _ctx.MainRigidbody.velocity.y, 0);

		yVel = Vector3.ClampMagnitude(yVel, _ctx.MaxVerticalSpeed);

		_ctx.MainRigidbody.velocity = xzVel + yVel;
	}
}
