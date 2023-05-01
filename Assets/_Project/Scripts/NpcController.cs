using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NpcController : MonoBehaviour, IInteractable
{
    private UIController uiController;
    [SerializeField] private Dialogue npcDialogue;
    
    private void OnEnable()
    {
        //uiController = GameObject.FindGameObjectWithTag(UIController.kDialogueControllerTag).GetComponent<UIController>();   
    }

    public void Interact()
    {
        uiController.OnNpcInteraction.Invoke(npcDialogue);
    }
}
