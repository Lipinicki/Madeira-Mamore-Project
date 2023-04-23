using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableArea : MonoBehaviour
{
	[HideInInspector] public bool CanInteract;
	[HideInInspector] public IInteractable Interaction;

	[SerializeField] private UnityEvent OnPotentialInteraction;
	[SerializeField] private UnityEvent OnPotentialInteractionCancelled;

	public const string kInteractable = "Interactable";

	private void OnEnable()
	{
		CollectableItemBehaviour.onCollectedEvent += OnCollection;
	}

	private void OnDisable()
	{
		CollectableItemBehaviour.onCollectedEvent -= OnCollection;
	}

	public void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag(kInteractable))
		{
			CanInteract = true;
			AddInteraction(other.gameObject);
			OnPotentialInteraction?.Invoke();
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag(kInteractable))
		{
			CanInteract = false;
			ResetInteraction();
			OnPotentialInteractionCancelled?.Invoke();
		}
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
		CanInteract = false;
	}
}
