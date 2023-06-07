

using UnityEngine;

public abstract class PlayerBaseState : GameState
{
	protected PlayerStateMachine _ctx;

	protected PlayerBaseState(PlayerStateMachine stateMachine)
	{
		_ctx = stateMachine;
	}

	protected void RotatePlayerToForward()
	{
		//Rotate to the movement direction
		UpdateFowardOrientation(_ctx.MovementVector.normalized);
	}

	protected void ClampsHorizontalVelocity(float maxSpeed)
	{
		Vector3 xzVel = new Vector3(_ctx.MainRigidbody.velocity.x, 0, _ctx.MainRigidbody.velocity.z);
		Vector3 yVel = new Vector3(0, _ctx.MainRigidbody.velocity.y, 0);

		xzVel = Vector3.ClampMagnitude(xzVel, maxSpeed); // Uses max walking speed instead

		_ctx.MainRigidbody.velocity = xzVel + yVel;
	}

	protected void ClampsVerticalVelocity(float maxSpeed)
	{
		Vector3 xzVel = new Vector3(_ctx.MainRigidbody.velocity.x, 0, _ctx.MainRigidbody.velocity.z);
		Vector3 yVel = new Vector3(0, _ctx.MainRigidbody.velocity.y, 0);

		yVel = Vector3.ClampMagnitude(yVel, maxSpeed);

		_ctx.MainRigidbody.velocity = xzVel + yVel;
	}

	protected bool CheckForLadder()
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

	protected void SetupActiveBlock(Transform t)
	{
		BasicPullPushBlock block = t.GetComponent<BasicPullPushBlock>();
		float blockDepth = block.MainCollider.bounds.size.z;

		_ctx.MaxInteractionDistance = blockDepth;
		_ctx.ActiveBlock = block;
	}

	protected virtual void MovePlayer(float speeed)
	{
		_ctx.MovementVector = _ctx.InputVector * speeed;

		//Moves the player
		_ctx.MainRigidbody.AddForce(_ctx.MovementVector * _ctx.MainRigidbody.mass, ForceMode.Force);
	}

	private void UpdateFowardOrientation(Vector3 directionVector)
	{
		Quaternion targetRotation = Quaternion.LookRotation(directionVector, Vector3.up);
		_ctx.transform.rotation = Quaternion.Slerp(_ctx.transform.rotation, targetRotation, Time.fixedDeltaTime * _ctx.RotationSpeed);
	}
}
