using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	// Just one Game Manager needs to exist
	public static GameManager Instance;

	[SerializeField] private SceneData sceneData;
	[SerializeField] private GameEvent onStartTitleScreen;
	[SerializeField] private bool startWithTitleScreen;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		sceneData.ClearSceneLists();

		if (startWithTitleScreen)
		{
			sceneData.LoadTitleScreen();
			onStartTitleScreen?.Raise();
			startWithTitleScreen = false;
		}
	}
}
