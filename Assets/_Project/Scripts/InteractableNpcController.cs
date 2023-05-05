using UnityEngine;

public class InteractableNpcController : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private Dialogue npcDialogue;

    void IInteractable.Interact()
    {
        dialogueController.PlayDialogue(npcDialogue);
    }
}
