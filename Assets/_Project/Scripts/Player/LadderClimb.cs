using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderClimb : MonoBehaviour
{
	[SerializeField, Range(-1, 0)] float facingDotThreshold = -0.9f;
	[SerializeField] float climbingSpeed = 5f;

	Transform _activeLadder = null;
	PlayerMovement _playerMovement;

	private const string kLadderLayer = "Ladders";

	private void Awake()
	{
		_playerMovement = GetComponent<PlayerMovement>();
	}

	void FixedUpdate()
	{
		//Handles ledge movement
		HandleLadderClimb();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == kLadderLayer) GrabLadder(other.transform);
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.transform.tag == kLadderLayer) ReleaseLadder();
	}

	private void HandleLadderClimb()
	{
		if (_playerMovement.CurrentPlayerState != ActionStates.Climbing || _activeLadder == null) return;

		Vector3 climbDirection = new Vector3(0f, _playerMovement.InputVector.z, 0f);
		Vector3 playerForward = transform.forward;
		Vector3 ladderForward = _activeLadder.forward;

		float facingDotProduct = Vector3.Dot(playerForward.normalized, ladderForward.normalized);

		if (facingDotProduct <= facingDotThreshold)
		{
			transform.Translate(climbDirection * climbingSpeed * Time.fixedDeltaTime);
		}

		if (_playerMovement.IsGrounded() && climbDirection.y < 0) ReleaseLadder();
	}

	private void ReleaseLadder()
	{
		_activeLadder = null;
		_playerMovement.ResetPlayerState();
	}

	private void GrabLadder(Transform ladderTransform)
	{
		_activeLadder = ladderTransform;
		_playerMovement.ChangePlayerState(ActionStates.Climbing);
	}
}
