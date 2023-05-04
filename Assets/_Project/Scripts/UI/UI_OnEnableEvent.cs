using UnityEngine;
using UnityEngine.Events;

// Triggers behaviour when a object is enabled on the canvas
public class UI_OnEnableEvent : MonoBehaviour
{
	[SerializeField] private UnityEvent OnUIStart;

	private void OnEnable()
	{
		OnUIStart?.Invoke();
	}
}
