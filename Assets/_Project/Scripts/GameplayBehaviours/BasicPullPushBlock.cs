using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPullPushBlock : MonoBehaviour
{
    private Rigidbody rb;
    private BoxCollider boxCollider;
    private AudioSource audioSrc;

    public Rigidbody MainRigidBody => rb;
    public BoxCollider MainCollider => boxCollider;
    private Coroutine fadeSoundRoutine = null;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        audioSrc = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }

    public void PlayAudio()
    {
        if (audioSrc.isPlaying || fadeSoundRoutine != null) return;
        audioSrc.Play();
    }
    
    // this method prevent the fadeOutRoutine to be stacked
    public void StopAudio()
    {
        if (!audioSrc.isPlaying || fadeSoundRoutine != null) return;
        
        fadeSoundRoutine = StartCoroutine(audioSrc.FadeOutSound());
        StartCoroutine(ClearFadeSoundRoutine());
    }

    private IEnumerator ClearFadeSoundRoutine()
    {
        yield return fadeSoundRoutine;
        fadeSoundRoutine = null;
    }

    private void OnDestroy()
    {
        if (fadeSoundRoutine != null) StopCoroutine(fadeSoundRoutine);
        fadeSoundRoutine = null;
    }
}
