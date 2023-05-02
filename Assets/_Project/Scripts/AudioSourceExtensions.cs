using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AudioSourceExtensions
{
    // gradually fade out the sound volume
    public static IEnumerator FadeOutSound(this AudioSource audioSrc, float fadeTime = 0.35f)
    {
        float startVolume = audioSrc.volume;

        while (audioSrc.volume > 0)
        {
            audioSrc.volume -= startVolume * Time.deltaTime / fadeTime;
            yield return null;
        }

        audioSrc.Stop();
        audioSrc.volume = startVolume;

        yield break;
    }
}
