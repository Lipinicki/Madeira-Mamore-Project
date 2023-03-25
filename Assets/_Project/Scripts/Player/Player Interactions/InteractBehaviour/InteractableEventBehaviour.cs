using UnityEngine;
using UnityEngine.Events;

public class InteractableEventBehaviour : MonoBehaviour, IInteractable
{
    [SerializeField] private UnityEvent OnInteractEvent;

	public void Interact()
	{
		OnInteractEvent?.Invoke();
	}

	public void PrintConsole(string message)
	{
		print(message);
	}
}
