using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPushing : MonoBehaviour
{
    public float maxDistance = 2.0f; // Maximum distance the player can push or pull a block
    public float moveSpeed = 5.0f; // Speed at which the block moves
	
	private bool isPushing;
	private Vector3 blockOffset; // Offset between the block's position and the player's position when the block was picked up
    private PlayerMovement _playerMovement;
	private Rigidbody activeBlock = null;
	private const string kPushBlocksLayer = "PushPullBlocks";

    private void OnEnable()
	{
		_playerMovement = GetComponent<PlayerMovement>();
		_playerMovement.PlayerInput.interactEvent += OnDragableInteraction;
    }

    private void OnDisable()
	{
		_playerMovement.PlayerInput.interactEvent -= OnDragableInteraction;
    }
    private void Update() {      
		UpdateBlockPosition();
    }
    
	private void UpdateBlockPosition()
	{
		if (activeBlock == null) return;

		// Determine if the player is pushing or pulling based on the dot product between the player's forward vector and the block's offset vector
		blockOffset = activeBlock.transform.position - transform.position;
		float inputDot = Vector3.Dot(_playerMovement.InputVector.normalized, blockOffset.normalized);
		isPushing = (inputDot > 0);

		// Determine the direction of the player's movement relative to the block
		Vector3 movementDirection = Vector3.Cross(blockOffset.normalized, transform.up);
		float movementDot = Vector3.Dot(movementDirection, _playerMovement.InputVector.normalized);

		// Calculate the perpendicular vector to both the blockOffset and the player's forward vector
		Vector3 perpendicularVector = Vector3.Cross(blockOffset, transform.TransformDirection(Vector3.forward)).normalized;

		// Determine which axis to disable based on the perpendicular vector
		bool disableHorizontalAxis = (Mathf.Abs(Vector3.Dot(movementDirection, perpendicularVector)) > 0.5f);
		bool disableVerticalAxis = (Mathf.Abs(Vector3.Dot(movementDirection, perpendicularVector)) < 0.5f);

		// Disable the correct axis while pushing or pulling the block
		_playerMovement.BlockMovementAxis(disableHorizontalAxis, disableVerticalAxis);

		// Calculate the target position for the block
		Vector3 targetPosition = transform.position + transform.forward * blockOffset.magnitude;

		// Calculate the movement amount based on the player's input
		float moveAmount = _playerMovement.InputVector.magnitude;

		// Only allow movement along the block's axis of motion
		Vector3 blockDirection = (isPushing ? transform.forward : -transform.forward);
		float moveAlongBlock = Vector3.Dot(_playerMovement.InputVector, blockDirection);
		Vector3 movement = blockDirection * moveAlongBlock;

		// Apply the movement to the block
		activeBlock.transform.position = Vector3.MoveTowards(activeBlock.transform.position, targetPosition + movement, moveSpeed * Time.deltaTime);
	}

	private void OnDragableInteraction()
	{
		if (activeBlock != null)
		{
            activeBlock.isKinematic = true;
			activeBlock = null;
			_playerMovement.ResetPlayerState();
			_playerMovement.UnlockMovementAxis();
		}
		else if (Physics.Raycast(transform.position, transform.forward, out var grabHit, maxDistance, LayerMask.GetMask(kPushBlocksLayer)) && activeBlock == null)
		{
			activeBlock = grabHit.transform.GetComponent<Rigidbody>();
            activeBlock.isKinematic = false;			
			_playerMovement.ChangePlayerSubstate(SubStates.Interacting);
		}
	}

}
