using UnityEngine;
using UnityEngine.Events;

// Listens for input directly from player's input events
public class UI_InputListener : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;

	[Space(10)]
	[SerializeField] private UnityEvent MenuCancel;
	[SerializeField] private UnityEvent SubmitEvent;
 
	private void OnEnable()
	{
		playerInput.menusCancelEvent += OnMenuCancel;
		playerInput.menusSubmitEvent += OnSubmit;
	}

	private void OnDisable()
	{
		playerInput.menusCancelEvent -= OnMenuCancel;
		playerInput.menusSubmitEvent -= OnSubmit;
	}

	private void OnMenuCancel()
	{
		MenuCancel?.Invoke();
	}

	private void OnSubmit()
	{
		SubmitEvent?.Invoke();
	}
}
