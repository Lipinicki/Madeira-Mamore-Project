using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NpcController : MonoBehaviour, IInteractable
{
    private DialogueController dialogueController;
    [SerializeField] private Dialogue npcDialogue;
    
    private void OnEnable()
    {
        dialogueController = GameObject.FindGameObjectWithTag(DialogueController.kDialogueControllerTag).GetComponent<DialogueController>();   
    }

    public void Interact()
    {
        dialogueController.OnNpcInteraction.Invoke(npcDialogue);
    }
}
