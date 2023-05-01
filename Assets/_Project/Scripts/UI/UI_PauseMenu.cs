using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UI_PauseMenu : MonoBehaviour, IPauseHandler
{
    [SerializeField] private PauseCallbackHandler callbackHandler;
	[SerializeField] private GameObject pauseScreen;
	[SerializeField] private GameObject optionsScreen;

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
		pauseScreen?.SetActive(true);
		pauseEvent?.Invoke();
	}

	public void OnResume()
	{
		pauseScreen?.SetActive(false);
		pauseEvent?.Invoke();
	}

	public void ShowOptions()
	{
		optionsScreen?.SetActive(true);
	}

	public void HideOptions()
	{
		optionsScreen?.SetActive(false);
	}
}
