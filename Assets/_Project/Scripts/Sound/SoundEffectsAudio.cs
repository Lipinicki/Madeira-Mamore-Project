using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectsAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip itemCollectSFX;

	private void OnEnable()
	{
		CollectableItemBehaviour.onCollectedEvent += OnCollection;
	}

	private void OnDisable()
	{
		CollectableItemBehaviour.onCollectedEvent -= OnCollection;
	}

	private void OnCollection(Item obj = null)
	{
		if (itemCollectSFX == null) return;

		if (audioSource == null) return;

		audioSource.PlayOneShot(itemCollectSFX);
	}

	public void PlaySoundEffect(AudioClip audioClip)
	{
		if (audioClip == null) return;

		if (audioSource == null) return;

		audioSource.PlayOneShot(audioClip);
	}
}
