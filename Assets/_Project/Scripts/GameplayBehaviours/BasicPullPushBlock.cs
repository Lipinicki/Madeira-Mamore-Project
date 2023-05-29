using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPullPushBlock : MonoBehaviour
{
    [SerializeField] private AudioSource blockAudioSource;

    private Rigidbody rb;
    private BoxCollider boxCollider;

    public Rigidbody MainRigidBody => rb;
    public BoxCollider MainCollider => boxCollider;
    private Coroutine fadeSoundRoutine = null;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
    }

    public void PlayAudio()
    {
        if (blockAudioSource.isPlaying || fadeSoundRoutine != null) return;
        blockAudioSource.Play();
    }
    
    // this method prevent the fadeOutRoutine to be stacked
    public void StopAudio()
    {
        if (!blockAudioSource.isPlaying || fadeSoundRoutine != null) return;
        
        fadeSoundRoutine = StartCoroutine(blockAudioSource.FadeOutSound());
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
