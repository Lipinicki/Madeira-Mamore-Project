using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	// Just one Game Manager needs to exist
	public static GameManager Instance;

	[SerializeField] private SceneData sceneData;
	[SerializeField] private GameEvent onStartTitleScreen;
	[SerializeField] private bool startWithTitleScreen;

	[SerializeField] private PlayerInput playerInput;
	[SerializeField] private UnityEvent activateInputEvent;
	[SerializeField] private UnityEvent nextLevelEvent;

	private void Awake()
	{
		playerInput.debugActivePlayerInputEvent += DebugOnActivatePlayerInput;
		playerInput.debugNextLevelEvent += DebugOnNextLevelEvent;

		Instance = this;
	}

	private void OnDestroy()
	{
		playerInput.debugActivePlayerInputEvent -= DebugOnActivatePlayerInput;
		playerInput.debugNextLevelEvent -= DebugOnNextLevelEvent;
	}

	private void Start()
	{
		sceneData.ClearSceneLists();

		// Show the main menu on entering
		if (startWithTitleScreen)
		{
			sceneData.LoadTitleScreen();
			onStartTitleScreen?.Raise();
			startWithTitleScreen = false;
		}
	}	

	private void DebugOnActivatePlayerInput()
	{
		activateInputEvent?.Invoke();
	}

	private void DebugOnNextLevelEvent()
	{
		nextLevelEvent?.Invoke();
	}
}
