using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour
{
    [SerializeField] private SceneData sceneDB;
    [SerializeField] private Image LoadingBarImage;
    [SerializeField, Tooltip("Used to tune looading bar animation")] private float LoadingLerpTime = 0.3f;

    // List of asynchronous operations running on background while loading scene
    private List<AsyncOperation> currentScenesToLoad = new List<AsyncOperation>();

    // Called when scene changing is raised, LoadingScreen needs to be seted through events
    public void StartProgressBar()
    {
        StartCoroutine(ShowLoadingScreen());
    }

    private IEnumerator ShowLoadingScreen()
    {
        for (int i = 0; i < sceneDB.scenesToLoad.Count; i++)
        {
            // Set current async operations
            currentScenesToLoad.Add(sceneDB.scenesToLoad[i]);
        }

        float elapsedTime = 0f; // time elapsed for the loading bar lerp

        for (int i = 0; i < currentScenesToLoad.Count; ++i)
        {
            while (!currentScenesToLoad[i].isDone) // checks individually for each load progress
            {
				float totalProgress = 0f; // progress of the loading going from 0 to 1f

				foreach (AsyncOperation operation in currentScenesToLoad)
                {
                    totalProgress += operation.progress;
                }

                totalProgress = totalProgress / currentScenesToLoad.Count; // normalizes the progress value

                // Lerps the bar fill for smooth animation
                LoadingBarImage.fillAmount = Mathf.Lerp(LoadingBarImage.fillAmount, totalProgress, elapsedTime/LoadingLerpTime);
                elapsedTime += Time.deltaTime;

                yield return null;
            }
        }

		gameObject.SetActive(false);
	}
}
