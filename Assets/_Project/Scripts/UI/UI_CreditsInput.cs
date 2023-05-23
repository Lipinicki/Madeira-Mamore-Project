using UnityEngine;
using UnityEngine.Events;

public class UI_CreditsInput : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;

	[Space(10)]
	[SerializeField] private UnityEvent MenuCancel;

	private void OnEnable()
	{
		playerInput.menusCancelEvent += OnMenuCancel;
	}

	private void OnDisable()
	{
		playerInput.menusCancelEvent -= OnMenuCancel;
	}

	private void OnMenuCancel()
	{
		MenuCancel?.Invoke();
	}
}
