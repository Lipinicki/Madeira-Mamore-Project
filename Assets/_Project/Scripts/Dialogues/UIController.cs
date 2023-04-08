using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;

    [Header("Dialogue Session")]
    [SerializeField] private DialoguePresenter dialoguePresenter;
    [HideInInspector] public UnityEvent<Dialogue> OnNpcInteraction = new UnityEvent<Dialogue>();
    public const string kDialogueControllerTag = "DialogueController";

    [Header("Pause Session")]
    [SerializeField] private GameObject pauseScreen;
    [HideInInspector] public UnityEvent<bool> OnGamePause = new UnityEvent<bool>();
	private bool gameIsPaused = false;

    private void OnEnable()
    {
        OnNpcInteraction.AddListener(ShowDialoguePresenter);
        playerInput.pauseEvent += PauseGame;
    }

    private void OnDisable() 
    {
        OnNpcInteraction.RemoveAllListeners();
        OnGamePause.RemoveAllListeners();
    }

    private void ShowDialoguePresenter(Dialogue dialogueToShow)
    {
        dialoguePresenter.gameObject.SetActive(true);
        dialoguePresenter.SetupDialoguePresenter(dialogueToShow);
    }

    private void PauseGame()
    {
        gameIsPaused = gameIsPaused ? false : true;
        float currentTimeScale = gameIsPaused ? 0f : 1f;
        pauseScreen.SetActive(gameIsPaused);
        Time.timeScale = currentTimeScale;
    }
}
