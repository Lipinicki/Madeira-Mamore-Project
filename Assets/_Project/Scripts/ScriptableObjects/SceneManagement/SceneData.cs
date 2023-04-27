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
	public List<SceneIndexes> indexesToLoad = new List<SceneIndexes>();
	public List<SceneIndexes> indexesToUnload = new List<SceneIndexes>();

	// Clear Lists of loading/unloading scenes
	public void ClearSceneLists()
	{
		indexesToLoad.Clear();
		indexesToUnload.Clear();
	}

	/// <summary>
	/// Used to change to a selected level through menu
	/// </summary>
	/// <param name="index"></param>
	public void LoadLevelWithIndex(SceneIndexes index)
	{
		// If the player has reached the last level, them goes back to menu
		// The length has -1 because of the game manager scene
		if ((int)index == SceneManager.sceneCountInBuildSettings - 1)
		{
			index = SceneIndexes.TitleScreen;
			CurrentSceneIndex = index;
		}

		// Load level index
		indexesToLoad.Add(index);
		LoadScenes();

		// Add previous scene to scenes to unload
		if (index > 0)
		{
			indexesToUnload.Add(index - 1);
		}

		// Unloads previous levels or the title screen if it was in the list
		if (indexesToUnload.Count > 0)
		{
			UnloadScenes();
		}

		indexesToUnload.Add(index);

		CurrentSceneIndex = index;
	}

	/// <summary>
	/// Used in buttons to pass ah SceneIndexes as reference
	/// </summary>
	/// <param name="sceneIndex"></param>
	public void LoadLevelWithIndex(SceneIndex sceneIndex)
	{
		LoadLevelWithIndex(sceneIndex.Index);
	}

	public void LoadNextLevel()
	{
		// goes to the next level in the list
		CurrentSceneIndex++;
		LoadLevelWithIndex(CurrentSceneIndex);
	}

	public void RestartLevel()
	{
		// restarts with the current level index
		LoadLevelWithIndex(CurrentSceneIndex);
	}

	public void LoadTitleScreen()
	{
		CurrentSceneIndex = SceneIndexes.TitleScreen;
		indexesToLoad.Add(CurrentSceneIndex);
		LoadScenes();

		// Add main menu scene to unload when the level starts
		indexesToUnload.Add(SceneIndexes.TitleScreen);
	}

	public void SetCurrentSceneActive()
	{
		Debug.Log(CurrentSceneIndex.ToString());
		SceneManager.SetActiveScene(SceneManager.GetSceneByName(CurrentSceneIndex.ToString()));
	}

	private void LoadScenes()
	{
		for (int i = 0; i < indexesToLoad.Count; i++)
		{
			scenesToLoad.Add(SceneManager.LoadSceneAsync((int)indexesToLoad[i], LoadSceneMode.Additive));

			indexesToLoad.Remove(indexesToLoad[i]);
		}
	}

	private void UnloadScenes()
	{
		for (int i = 0; i < indexesToUnload.Count; i++)
		{
			Debug.Log(indexesToUnload[i].ToString());
			SceneManager.UnloadSceneAsync(indexesToUnload[i].ToString());

			indexesToUnload.Remove(indexesToUnload[i]);
		}
	}

	private SceneIndexes SceneIndexFromName(string name)
	{
		return (SceneIndexes) Enum.Parse(typeof(SceneIndexes), name);
	}
}
