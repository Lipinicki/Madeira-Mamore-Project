using System;
using System.Collections.Generic;
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

	/// <summary>
	/// Clear Lists of loading/unloading scenes
	/// </summary>
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
		if ((int)index >= SceneManager.sceneCountInBuildSettings - 1)
		{
			indexesToUnload.Add(--CurrentSceneIndex);
			index = SceneIndexes.TitleScreen;
			CurrentSceneIndex = index;
		}

		// Load level index
		indexesToLoad.Add(index);
		LoadScenes();

		// Add previous scene to scenes to unload
		if ((int)index > 0)
		{
			indexesToUnload.Add(index - 1);
		}

		// Unloads previous levels or the title screen if it was in the list
		if (indexesToUnload.Count > 0)
		{
			UnloadScenes();
		}

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

	/// <summary>
	/// Load next scene on the scene list
	/// </summary>
	public void LoadNextLevel()
	{
		// goes to the next level in the list
		CurrentSceneIndex++;
		LoadLevelWithIndex(CurrentSceneIndex);
	}

	/// <summary>
	/// Will restart the current playing level
	/// </summary>
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
		//indexesToUnload.Add(SceneIndexes.TitleScreen);
	}


	/// <summary>
	/// Will try to set the current playable scene to be the active one
	/// </summary>
	/// <returns>Wheter the function succed or not</returns>
	public bool TrySetCurrentActiveScene()
	{
		Scene sceneToActivate = SceneManager.GetSceneByName(CurrentSceneIndex.ToString());

		if (sceneToActivate.isLoaded)
		{
			//Debug.Log(CurrentSceneIndex.ToString(), this);
			SceneManager.SetActiveScene(SceneManager.GetSceneByName(CurrentSceneIndex.ToString()));
			return true;
		}
		else
		{
			return false;
		}
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
			Scene unloadedScene = SceneManager.GetSceneByName(indexesToUnload[i].ToString());

			//Checks if the scene is loaded, if is not, just cleans the list position
			if (unloadedScene.isLoaded)
			{
				scenesToLoad.Add(SceneManager.UnloadSceneAsync(indexesToUnload[i].ToString()));
			}

			indexesToUnload.Remove(indexesToUnload[i]);
		}
	}

	// Just in case there is need for the conversion
	private SceneIndexes SceneIndexFromName(string name)
	{
		return (SceneIndexes)Enum.Parse(typeof(SceneIndexes), name);
	}
}
