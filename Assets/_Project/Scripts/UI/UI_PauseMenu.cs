using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UI_PauseMenu : MonoBehaviour, IPauseHandler
{
    [SerializeField] private PauseCallbackHandler callbackHandler;
	[SerializeField] private GameObject pauseScreen;
	[SerializeField] private GameObject optionsScreen;
	[SerializeField] private GameObject inventoryScreen;

	[Space(10)]
	[Header("First Selected Button")]
	[SerializeField] private GameObject _firstSelectButton;

	[SerializeField] private UnityEvent pauseEvent;
	[SerializeField] private UnityEvent resumeEvent;

	private void OnEnable()
	{
		callbackHandler?.SubscribeCallback(this);

		Invoke(nameof(SelectFirstButton), 0.05f);	
	}

	private void OnDisable()
	{
		callbackHandler?.UnsubscribeCallback(this);
	}

	public void OnPause()
	{
		pauseScreen?.SetActive(true);
		Invoke(nameof(SelectFirstButton), 0.05f);

		pauseEvent?.Invoke();
	}

	public void OnResume()
	{
		pauseScreen?.SetActive(false);

		resumeEvent?.Invoke();
	}

	public void ShowOptions()
	{
		pauseScreen?.SetActive(false);

		optionsScreen?.SetActive(true);
	}

	public void HideOptions()
	{
		optionsScreen?.SetActive(false);

		pauseScreen?.SetActive(true);
		Invoke(nameof(SelectFirstButton), 0.05f);
	}

	public void ShowInvetory()
	{
		pauseScreen?.SetActive(false);
		inventoryScreen?.SetActive(true);
	}

	public void HideInventory()
	{
		inventoryScreen?.SetActive(false);

		pauseScreen?.SetActive(true);
		Invoke(nameof(SelectFirstButton), 0.05f);
	}

	private void SelectFirstButton()
	{
		EventSystem.current.SetSelectedGameObject(_firstSelectButton);
	}
}
