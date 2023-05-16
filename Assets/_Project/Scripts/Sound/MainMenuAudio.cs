using UnityEngine;

// MainMenu options for playing audio in the screen
public class MainMenuAudio : MonoBehaviour
{
	// This Audio Source
	[SerializeField] private AudioSource audioSource;

	public void PlayMusicAudio()
	{
		audioSource.Play();
	}
}
