using UnityEngine;

public class UI_MainMenu : MonoBehaviour
{
	[SerializeField] private GameObject OptionsScreen;
	[SerializeField] private GameObject MainMenuOptions;

	public void ShowOptionsScreen()
	{
		OptionsScreen?.SetActive(true);
		
		MainMenuOptions?.SetActive(false);
	}

	public void ShowMainMenuOptions()
	{
		MainMenuOptions?.SetActive(true);

		OptionsScreen?.SetActive(false);
	}

	public void QuitGame()
	{
		Application.Quit();

#if UNITY_EDITOR
		Debug.Log("Quit Game!");
#endif
	}
}
