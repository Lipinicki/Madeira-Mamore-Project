using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private List<AudioClip> footstepsSFX;
	[SerializeField] private float footStepsDelay = 1.0f;
	[SerializeField, Range(0.0f, 1.0f)] private float footStepsVolume = 0.5f;

	private Coroutine stepsRoutine = null;

	// Plays the steps sound if it is not playing yet
	public void SetupStepsAudio()
	{
		if (stepsRoutine != null) return;
		stepsRoutine = StartCoroutine(PlayFootstepsAudio());
	}

	// Stops steps sound
	public void DisableStepsAudio()
	{
		audioSource.Stop();
		if (stepsRoutine == null) return;
		StopCoroutine(stepsRoutine);
		stepsRoutine = null;
	}

	IEnumerator PlayFootstepsAudio()
	{
		while (true)
		{
			int index = Random.Range(0, footstepsSFX.Count);
			audioSource.PlayOneShot(footstepsSFX[index], footStepsVolume);

			yield return new WaitForSeconds(footStepsDelay);
		}
	}

	private void OnDisable()
	{
		DisableStepsAudio();
	}
}
