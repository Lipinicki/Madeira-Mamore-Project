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
	[SerializeField] private GameObject fadeInSreen;
	[SerializeField] private bool startWithTitleScreen = true;

	[Space(10)]
	[Header("Debug")]
	[SerializeField] private bool fadeOnStart = false;

	[SerializeField] private PlayerInput playerInput;
	[SerializeField] private UnityEvent activatePlayerInputEvent;
	[SerializeField] private UnityEvent activateMenusInputEvent;
	[SerializeField] private UnityEvent nextLevelEvent;

	private void Awake()
	{
		playerInput.debugActivePlayerInputEvent += DebugOnActivatePlayerInput;
		playerInput.debugActiveMenusInputEvent += DebugOnActivateMenusInput;
		playerInput.debugNextLevelEvent += DebugOnNextLevelEvent;

		Instance = this;
	}

	private void OnDestroy()
	{
		playerInput.debugActivePlayerInputEvent -= DebugOnActivatePlayerInput;
		playerInput.debugActiveMenusInputEvent -= DebugOnActivateMenusInput;
		playerInput.debugNextLevelEvent -= DebugOnNextLevelEvent;
	}

	private void Start()
	{
		sceneData.ClearSceneLists();

		if (fadeOnStart)
		{
			fadeInSreen.GetComponent<Animator>().SetTrigger("FadeIn");
		}

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
		activatePlayerInputEvent?.Invoke();
	}

	private void DebugOnActivateMenusInput()
	{
		activateMenusInputEvent?.Invoke();
	}

	private void DebugOnNextLevelEvent()
	{
		nextLevelEvent?.Invoke();
	}
}
