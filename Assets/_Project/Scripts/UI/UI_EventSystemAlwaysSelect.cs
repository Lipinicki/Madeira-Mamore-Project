using UnityEngine;
using UnityEngine.EventSystems;

// Used to always select a button inside canvas screen
public class UI_EventSystemAlwaysSelect : MonoBehaviour
{
	private EventSystem eventSystem;
	private GameObject lastSelectedGameObject; // tracks the last selected object

	void Start()
	{
		eventSystem = EventSystem.current;
	}

	public void Update()
	{
		GetLastGameObjectSelected();
	}

	// Always select the last gameObject
	private void GetLastGameObjectSelected()
	{
		if (eventSystem == null) return;

		if (eventSystem.currentSelectedGameObject != null)
		{
			lastSelectedGameObject = eventSystem.currentSelectedGameObject;
		}
		else
		{
			eventSystem.SetSelectedGameObject(lastSelectedGameObject);
		}
	}
}
