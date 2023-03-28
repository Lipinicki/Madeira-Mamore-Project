using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
	[SerializeField] private AudioSource audioSource;
	[SerializeField] private List<AudioClip> footstepsSFX;
	[SerializeField] private float footStepsDelay = 1.0f;
	[SerializeField, Range(0.0f, 1.0f)] private float footStepsVolume = 0.5f;

	private PlayerMovement playerMov;

	private void Start()
	{
		playerMov = GetComponent<PlayerMovement>();
		StartCoroutine(PlayFootstepsAudio());
	}

	IEnumerator PlayFootstepsAudio()
	{
		while (true)
		{
			if (playerMov.IsWalking())
			{
				int index = Random.Range(0, footstepsSFX.Count);
				audioSource.PlayOneShot(footstepsSFX[index], footStepsVolume);
			}

			yield return new WaitForSeconds(footStepsDelay);
		}
	}
}
