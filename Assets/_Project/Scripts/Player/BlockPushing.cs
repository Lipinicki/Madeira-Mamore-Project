using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPushing : MonoBehaviour
{
    public float maxRaycastDistance = 2.0f; // Maximum distance the player can push or pull a block
    public float pushForce = 10.0f; // Force applied to the block when pushed
    public LayerMask pushLayer;

    private float maxBlockDistance;
    private bool isPushing;
    private Vector3 blockOffset; // Offset between the block's position and the player's position when the block was picked up
    private Rigidbody activeBlock = null;
    private PlayerMovement _playerMovement;


    private void OnEnable()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerMovement.PlayerInput.interactEvent += OnDragableInteraction;
    }

    private void OnDisable()
    {
        _playerMovement.PlayerInput.interactEvent -= OnDragableInteraction;
    }

    private void Update()
    {
        UpdateBlockPosition();
    }

    private void UpdateBlockPosition()
    {
		if (activeBlock == null) return;

		// Determine if the player is pushing or pulling based on the dot product between the player's forward vector and the block's offset vector
		blockOffset = activeBlock.transform.position - transform.position;
		float inputDot = Vector3.Dot(_playerMovement.InputVector.normalized, blockOffset.normalized);
		isPushing = (inputDot > 0);

		// Only allow movement along the block's axis of motion
		Vector3 blockDirection = _playerMovement.InputVector.normalized;
		activeBlock.AddForce(blockDirection * pushForce * activeBlock.mass, ForceMode.Force);
	}

	private void LateUpdate()
	{
		if (activeBlock == null) return;

		// Limit the distance between the player and the block to maxDistance
		float distanceToBlock = Vector3.Distance(activeBlock.transform.position, transform.position);
		if (distanceToBlock > maxBlockDistance)
		{
			Vector3 directionToBlock = (activeBlock.transform.position - transform.position).normalized;
			activeBlock.transform.position = transform.position + directionToBlock * maxBlockDistance;
		}
	}

	private void OnDragableInteraction()
	{
		if (activeBlock != null)
		{
			activeBlock.velocity = Vector3.zero;
			activeBlock.angularVelocity = Vector3.zero;
			activeBlock.isKinematic = true;
			_playerMovement.ResetPlayerState();
		}
		else if (Physics.Raycast(transform.position, transform.forward, out var grabHit, maxRaycastDistance, pushLayer))
		{
			activeBlock = grabHit.transform.GetComponent<Rigidbody>();
			maxBlockDistance = grabHit.transform.GetComponent<BoxCollider>().size.z + 0.5f;
			activeBlock.isKinematic = false;
			_playerMovement.ChangePlayerSubstate(SubStates.Interacting);
		}
	}
}
