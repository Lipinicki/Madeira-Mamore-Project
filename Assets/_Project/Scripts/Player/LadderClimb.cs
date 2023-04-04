using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderClimb : MonoBehaviour
{
	[SerializeField] float climbingSpeed = 5f;
	[SerializeField, Range(-1, 0)] float facingDotThreshold = -0.9f;

	Transform _activeLadder = null;
	PlayerMovement _playerMovement;

	private const string kLadderTag = "Ladders";

	private void OnEnable()
	{
		_playerMovement.PlayerInput.interactEvent += GrabLadder;
	}

	private void OnDisable()
	{
		_playerMovement.PlayerInput.interactEvent -= GrabLadder;
	}

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
		if (other.transform.tag == kLadderTag) _activeLadder = other.transform;
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.transform.tag == kLadderTag) ReleaseLadder();
	}

	private void HandleLadderClimb()
	{
		if (_playerMovement.CurrentPlayerState != ActionStates.Climbing || _activeLadder == null) return;
		Vector3 climbDirection = new Vector3(0f, _playerMovement.InputVector.z, 0f);
		Vector3 playerForward = transform.forward;
		Vector3 ladderForward = _activeLadder.forward;

		if (CheckFacingVectors(playerForward.normalized, ladderForward.normalized))
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

	private void GrabLadder()
	{
		if (_activeLadder == null) return;

		_playerMovement.ChangePlayerState(ActionStates.Climbing);
	}

	public bool CheckFacingVectors(Vector3 vectorA, Vector3 vectorB)
	{
		float facingDotProduct = Vector3.Dot(vectorA.normalized, vectorB.normalized);
		return (facingDotProduct <= facingDotThreshold);
	}
}
