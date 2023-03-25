using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInteraction : MonoBehaviour
{
	public PlayerInput playerInput;

    [SerializeField] private InteractableArea areaOfInteraction;
	[SerializeField] private UnityEvent OnInteraction;

	private void OnEnable()
	{
		playerInput.interactEvent += OnInteract;
	}

	private void OnDisable()
	{
		playerInput.interactEvent -= OnInteract;
	}

	public void OnInteract()
	{
		if (areaOfInteraction.CanInteract)
		{
			areaOfInteraction.Interaction.Interact();
			OnInteraction?.Invoke();
		}
	}
}
