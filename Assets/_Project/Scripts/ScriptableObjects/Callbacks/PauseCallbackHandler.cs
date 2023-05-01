using UnityEngine;

// Used to store objects to receive notifications at runtime
// IPauseHandler Objects will know when the game is paused or unpaused
[CreateAssetMenu(fileName = "PauseHandler", menuName = "Pause Handler")]
public class PauseCallbackHandler : CallbackHandlerSO<IPauseHandler>
{
    public void Pause()
    {
        for (int i = 0; i < callbacks.Count; i++)
        {
            callbacks[i].OnPause();
        }
    }

	public void Resume()
	{
        for (int i = 0; i < callbacks.Count; i++)
        {
            callbacks[i].OnResume();
        }
	}
}
