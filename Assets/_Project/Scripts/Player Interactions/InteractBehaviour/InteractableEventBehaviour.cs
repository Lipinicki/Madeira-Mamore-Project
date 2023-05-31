using UnityEngine;
using UnityEngine.Events;

public class InteractableEventBehaviour : MonoBehaviour, IInteractable
{
    [SerializeField] private UnityEvent OnInteractEvent;

	private void OnDisable()
	{
		//OnInteractEvent.RemoveAllListeners();
	}

	void IInteractable.Interact()
	{
		if (!enabled) return;

		OnInteractEvent?.Invoke();
	}

	public void PrintConsole(string message)
	{
		print(message);
	}
}
