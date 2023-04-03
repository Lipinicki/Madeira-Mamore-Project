using UnityEngine;

public abstract class PlayerOnAirState : PlayerBaseState
{
	protected PlayerOnAirState(PlayerStateMachine stateMachine) : base(stateMachine)
	{
	}

	public override void FixedTick(float fixedDeltaTime)
	{
		_stateMachine.ApplyGravity();
		ClampsVerticalVelocity();
	}

	private void ClampsVerticalVelocity()
	{
		Vector3 xzVel = new Vector3(_stateMachine.MainRigidbody.velocity.x, 0, _stateMachine.MainRigidbody.velocity.z);
		Vector3 yVel = new Vector3(0, _stateMachine.MainRigidbody.velocity.y, 0);

		yVel = Vector3.ClampMagnitude(yVel, _stateMachine.MaxVerticalSpeed);

		_stateMachine.MainRigidbody.velocity = xzVel + yVel;
	}
}
