using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UI_PauseMenu : MonoBehaviour, IPauseHandler
{
    [SerializeField] private PauseCallbackHandler callbackHandler;

	[SerializeField] private GameObject pausePresenter;
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
	}

	private void OnDisable()
	{
		callbackHandler?.UnsubscribeCallback(this);
	}

	public void OnPause()
	{
		pausePresenter?.SetActive(true);
		ShowPauseScreen();

		StartCoroutine(DelayFirstSelection());

		pauseEvent?.Invoke();
	}

	public void OnResume()
	{
		pausePresenter?.SetActive(false);

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
		StartCoroutine(DelayFirstSelection());
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
		StartCoroutine(DelayFirstSelection());
	}

	private void ShowPauseScreen()
	{
		inventoryScreen?.SetActive(false);
		optionsScreen?.SetActive(false);

		pauseScreen?.SetActive(true);
	}

	private IEnumerator DelayFirstSelection()
	{
		yield return new WaitForSecondsRealtime(.2f);

		SelectFirstButton();
	}

	private void SelectFirstButton()
	{
		EventSystem.current.SetSelectedGameObject(_firstSelectButton);
	}
}
