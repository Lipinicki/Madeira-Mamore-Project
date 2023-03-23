using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPushing : MonoBehaviour
{
    
	private Rigidbody _activeBlock = null;
	private const string kPushBlocksLayer = "PushPullBlocks";

    private void OnEnable() {
        
		PlayerInput.interactEvent += OnDragableInteraction;
    }

    private void OnDisable() {
		PlayerInput.interactEvent -= OnDragableInteraction;
    }
    private void FixedUpdate() {      
		UpdateBlockPosition();
    }
    
	private void UpdateBlockPosition()
	{
		if (_activeBlock == null) return;

		bool isPushing = CheckFacingVectors(_activeBlock.transform.forward.normalized, transform.forward.normalized);
		Vector3 forceVector = isPushing ? transform.forward.normalized : _activeBlock.transform.forward.normalized;
		_activeBlock.AddForce(forceVector * _activeBlock.mass * 10f, ForceMode.Force);
	}

	private void OnDragableInteraction()
	{
		if (_activeBlock != null)
		{
			_activeBlock = null;
		}
		else if (Physics.Raycast(transform.position, transform.forward, out var grabHit, 1f, LayerMask.GetMask(kPushBlocksLayer)) && _activeBlock == null)
		{
			_activeBlock = grabHit.transform.GetComponent<Rigidbody>();
		}
	}
    
	private bool CheckFacingVectors(Vector3 vectorA, Vector3 vectorB)
	{
		float facingDotProduct = Vector3.Dot(vectorA.normalized, vectorB.normalized);
		return (facingDotProduct <= _facingDotThreshold);
	}
}
