using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableChainBehaviour : MonoBehaviour, IInteractable
{
    [SerializeField] private List<GameObject> interactionsToTrigger = new List<GameObject>();

	public void Interact()
	{
		foreach (var interactionObject in interactionsToTrigger)
		{
			var interaction = interactionObject.GetComponent<IInteractable>();
			if (interaction == null) continue;
			interaction.Interact();
		}
	}

	public void PrincConsole(string message)
	{
		Debug.Log(message);
	}
}
