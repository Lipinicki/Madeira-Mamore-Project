using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SceneData", menuName = "Scene Data")]
public class SceneData : ScriptableObject
{
	public SceneIndexes CurrentSceneIndex;

	// List of asynchronous operations running on background while loading scene
	public List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();

	// Used to load/unload scenes to/from the scenes
	public List<SceneIndexes> indexesToload = new List<SceneIndexes>();
	public List<SceneIndexes> indexesToUnload = new List<SceneIndexes>();

	[Tooltip("Dummy instance for being passed as argument")]
	public SceneIndex loadingIndex;

	// Used to change to a selected level through menu
	public void LoadLevelWithIndex(SceneIndex sceneIndex)
	{
		// If the player has reached the last level, them goes back to menu
		if ((int)sceneIndex.Index == SceneManager.sceneCount)
		{
			sceneIndex.Index = SceneIndexes.TitleScreen;
			CurrentSceneIndex = sceneIndex.Index;
		}

		// Load level index
		indexesToload.Add(sceneIndex.Index);
		LoadScenes();
		Debug.Log((int)sceneIndex.Index);

		// Add previous scene to scenes to unload
		if (sceneIndex.Index > 0)
		{
			indexesToUnload.Add(sceneIndex.Index - 1);
		}

		// Unloads previous levels or the title screen if it was in the list
		UnloadScenes();

		indexesToUnload.Add(sceneIndex.Index);

		CurrentSceneIndex = sceneIndex.Index;
	}

	public void LoadNextLevel()
	{
		// goes to the next level in the list
		CurrentSceneIndex++;
		loadingIndex.Index = CurrentSceneIndex;
		LoadLevelWithIndex(loadingIndex);
	}

	public void RestartLevel()
	{
		// restarts with the current level index
		LoadLevelWithIndex(loadingIndex);
	}

	public void LoadTitleScreen()
	{
		CurrentSceneIndex = SceneIndexes.TitleScreen;
		indexesToload.Add(CurrentSceneIndex);
		LoadScenes();

		// Add main menu scene to unload when the level starts
		indexesToUnload.Add(CurrentSceneIndex);
	}

	private void LoadScenes()
	{
		if (scenesToLoad.Count == 0) return;

		for (int i = 0; i < indexesToload.Count; i++)
		{
			scenesToLoad.Add(SceneManager.LoadSceneAsync((int)indexesToload[i], LoadSceneMode.Additive));
		}
	}

	private void UnloadScenes()
	{
		if (indexesToUnload.Count == 0) return;

		for (int i = 0; i < indexesToUnload.Count; i++)
		{
			scenesToLoad.Add(SceneManager.UnloadSceneAsync((int)indexesToload[i]));
		}
	}

	private SceneIndexes SceneIndexFromName(string name)
	{
		return (SceneIndexes) Enum.Parse(typeof(SceneIndexes), name);
	}
}
