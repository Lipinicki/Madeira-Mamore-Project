using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableArea : MonoBehaviour, IInteractable
{
	public bool CanInteract 
	{ 
		get 
		{
			return Interaction == null ? false : true;
		} 
	}

	[SerializeField] private UnityEvent OnPotentialInteraction;
	[SerializeField] private UnityEvent OnPotentialInteractionCancelled;

	private IInteractable Interaction;

	public const string kInteractable = "Interactable";

	private void OnEnable()
	{
		CollectableItemBehaviour.onCollectedEvent += OnCollection;
	}

	private void OnDisable()
	{
		CollectableItemBehaviour.onCollectedEvent -= OnCollection;
	}

	public void OnTriggerStay(Collider other)
	{
		if (other.gameObject.CompareTag(kInteractable))
		{
			AddInteraction(other.gameObject);
			OnPotentialInteraction?.Invoke();
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag(kInteractable))
		{
			ResetInteraction();
			OnPotentialInteractionCancelled?.Invoke();
		}
	}

	// Interacts with interactable objects
	public void Interact()
	{
		Interaction.Interact();

		ResetInteraction();
		OnPotentialInteractionCancelled?.Invoke(); // Cancels Interaction related actions
	}

	// Both will be used when other script needs to notify a prompt
	public void NotifyInteraction()
	{
		OnPotentialInteraction?.Invoke();
	}

	public void NotifyInteractionCancel()
	{
		OnPotentialInteractionCancelled?.Invoke();
	}

	private void AddInteraction(GameObject interactableObject)
	{
		Interaction = interactableObject.GetComponent<IInteractable>();
	}

	private void ResetInteraction()
	{
		Interaction = null;
	}

	private void OnCollection(Item item = null)
	{
		if (Interaction == null) return;

		Interaction = null;
	}
}
