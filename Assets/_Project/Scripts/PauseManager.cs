using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private PauseCallbackHandler _callbackHandler;
    [SerializeField] private PlayerInput _playerInput;

	private void OnEnable()
	{
		_playerInput.pauseEvent += PauseGame;
		_playerInput.unPauseEvent += ResumeGame;
	}

	private void OnDisable()
	{
		_playerInput.pauseEvent -= PauseGame;
		_playerInput.unPauseEvent -= ResumeGame;
	}

	public void PauseGame()
    {
        // Freeze timeScale
        Time.timeScale = 0f;

        // Disables Player Input
        _playerInput.EnableMenusInput();

        _callbackHandler.Pause();
    }

    public void ResumeGame()
    {
		// Unfreeze timeScale
		Time.timeScale = 1f;

		// Unables Player Input
		_playerInput.EnablePlayerInput();

		_callbackHandler.Resume();
    }
}
