using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MyBox;
using DG.Tweening;

public class InteractableDoorLockBehaviour : MonoBehaviour, IInteractable
{
    [SerializeField] private bool useLever = false; // if true the door will open using a lever instead of a key
    [ConditionalField(nameof(useLever), inverse: true)][SerializeField] private Inventory playerInventory;
    [ConditionalField(nameof(useLever), inverse: true)][SerializeField] private string keyItemName;
    [SerializeField] private Animator animator;
    [SerializeField] private UnityEvent OnDoorOpenned;

    private readonly int r_DoorAnimationState = Animator.StringToHash("DoorOpening");

	void IInteractable.Interact()
	{
        if (playerInventory?.GetItem(keyItemName) == null && !useLever)
        {
            Debug.LogWarning("Item: " + keyItemName + "not found");
            return;
        }

        animator.Play(r_DoorAnimationState);
        OnDoorOpenned?.Invoke();
        this.enabled = false;
	}
}
