using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private DialoguePresenter dialoguePresenter;
    [HideInInspector] public UnityEvent<Dialogue> OnNpcInteraction = new UnityEvent<Dialogue>();
    public const string kDialogueControllerTag = "DialogueController";

    private void OnEnable()
    {
        OnNpcInteraction.AddListener(ShowDialoguePresenter);
    }

    private void OnDisable() 
    {
        OnNpcInteraction.RemoveAllListeners();
    }

    private void ShowDialoguePresenter(Dialogue dialogueToShow)
    {
        dialoguePresenter.gameObject.SetActive(true);
        dialoguePresenter.SetupDialoguePresenter(dialogueToShow);
    }
}
