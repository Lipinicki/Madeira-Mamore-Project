using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LeverController : MonoBehaviour, IInteractable
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Animator animator;
    [SerializeField] private MonoBehaviour chainBehaviour;

    public const string kLeverActivate = "Activate";
    public const string kLeverIdle = "Idle";

    private bool isInteracting = false;

    public void Interact()
    {
        if (isInteracting) return;
        
        isInteracting = true;
        animator?.SetTrigger(kLeverActivate);
        audioSource?.Play();

        IInteractable currentInteraction = chainBehaviour as IInteractable;
        currentInteraction.Interact();
    }

    public void SetIdle()
    {
        animator.SetTrigger(kLeverIdle);
        isInteracting = false;
    }
}
