using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


/// <summary>
/// Class used to listen to hover and exit events for this Button GameObject
/// </summary>
public class UI_ButtonCallbacksListener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
	[SerializeField] private UnityEvent OnHover;
	[SerializeField] private UnityEvent OnExit;
	[SerializeField] private UnityEvent OnButtonSelect;
	[SerializeField] private UnityEvent OnButtonDeselect;

	public void OnPointerEnter(PointerEventData eventData)
	{
		OnHover?.Invoke();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		OnExit?.Invoke();
	}

	public void OnSelect(BaseEventData eventData)
	{
		if (EventSystem.current.currentSelectedGameObject == null)
		{
			EventSystem.current.SetSelectedGameObject(gameObject);
		}

		OnButtonSelect?.Invoke();
	}

	public void OnDeselect(BaseEventData eventData)
	{
		OnButtonDeselect?.Invoke();
	}
}
