using UnityEngine;
using UnityEngine.Events;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;

    [Header("Dialogue Session")]
    [SerializeField] private DialoguePresenter dialoguePresenter;

    public UnityAction<Dialogue> OnNpcInteraction;

    private void OnEnable()
    {
        OnNpcInteraction += ShowDialoguePresenter;
    }

    private void OnDisable() 
    {
        OnNpcInteraction -= ShowDialoguePresenter;
    }

	/// <summary>
	/// Triggers the dialogue from other scripts receiving the desirable 
	/// dialogue data
	/// </summary>
	/// <param name="dialogue">The dialogue that wilk play in the scene</param>
	public void PlayDialogue(Dialogue dialogue)
    {
        playerInput.EnableMenusInput();

        OnNpcInteraction?.Invoke(dialogue);
    }

    private void ShowDialoguePresenter(Dialogue dialogueToShow)
    {
        dialoguePresenter.gameObject.SetActive(true);
        dialoguePresenter.SetupDialoguePresenter(dialogueToShow);
    }
}
