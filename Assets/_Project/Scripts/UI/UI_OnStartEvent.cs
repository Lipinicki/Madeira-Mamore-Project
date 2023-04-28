using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Triggers behaviour when a object is enabled on the canvas
public class UI_OnStartEvent : MonoBehaviour
{
	[SerializeField] private UnityEvent OnUIStart;

	private void OnEnable()
	{
		OnUIStart?.Invoke();
	}
}
