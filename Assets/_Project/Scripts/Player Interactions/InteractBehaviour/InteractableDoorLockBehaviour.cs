using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableDoorLockBehaviour : MonoBehaviour, IInteractable
{
    [SerializeField] private Inventory playerInventory;
    [SerializeField] private string keyItemName;
    [SerializeField] private Animator animator;
    [SerializeField] private UnityEvent OnDoorOpenned;

    private readonly int r_DoorAnimationState = Animator.StringToHash("DoorOpening");

	void IInteractable.Interact()
	{
        if (playerInventory.GetItem(keyItemName) == null)
        {
            Debug.Log("Item: " + keyItemName + "not found");
            return;
        }

        animator.Play(r_DoorAnimationState);
        OnDoorOpenned?.Invoke();
        this.enabled = false;
	}
}
