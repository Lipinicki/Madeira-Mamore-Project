
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class DialoguePresenter : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;

    [Header("Visual Components")]
    [SerializeField] private Image portrait;
    [SerializeField] private Image portraitShadow;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text characterName;
    
    [Space]
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;

    [SerializeField] private GameObject leftXmark;
    [SerializeField] private GameObject rightXmark;


    [Space]

    [Header("Dialogue Actions")]
    [SerializeField] private AudioSource audioScr;
    [SerializeField] private Button returnButton;
    [SerializeField] private Button advanceButton;

    private Dialogue activeDialogue = null;
    private Speech currentSpeech = null;
    private int speechIndex = 0;

    private void OnEnable()
    {
        returnButton.onClick.AddListener(ReturnDialogue);
        advanceButton.onClick.AddListener(AdvanceDialogue);
    }    
    
    private void OnDisable()
    {
        returnButton.onClick.RemoveAllListeners();
        advanceButton.onClick.RemoveAllListeners();
    }

    public void SetupDialoguePresenter(Dialogue dialogue)
    {
        gameObject.SetActive(true);
        activeDialogue = dialogue;
        UpdateDialoguePresenter();
    }

    private void UpdateDialoguePresenter()
    {
        if (activeDialogue.SpheechCount == 0) return;

        List<Speech> speechies = activeDialogue.speechList.ToList();

        if (speechies.IndexOf(currentSpeech) == speechIndex) return;

        currentSpeech = speechies[speechIndex];

        portrait.sprite = currentSpeech.characterPortrait;
        portraitShadow.sprite = currentSpeech.characterPortrait;
        characterName.SetText(currentSpeech.characterName);
        dialogueText.SetText(currentSpeech.speechText);
        
        leftArrow.SetActive(speechIndex > 0);
        rightArrow.SetActive(speechIndex < activeDialogue.SpheechCount - 1);
        
        leftXmark.SetActive(speechIndex == 0);
        rightXmark.SetActive(speechIndex == activeDialogue.SpheechCount - 1);

        if (currentSpeech.hasAudio)
        { 
            audioScr.clip = currentSpeech.speechAudio;
            audioScr.Play();
        }
    }

    public void AdvanceDialogue()
    {
        if (activeDialogue == null) return;
        if (speechIndex == activeDialogue.SpheechCount - 1)
        {
            EndDialogue();
            return;
        }

        speechIndex++;
        speechIndex = Math.Clamp(speechIndex, 0, activeDialogue.SpheechCount - 1);
        
        UpdateDialoguePresenter();
    }

    public void ReturnDialogue()
    {
        if (activeDialogue == null) return;
        if (speechIndex == 0)
        { 
            EndDialogue();
            return;
        }
        
        speechIndex--;
        speechIndex = Math.Clamp(speechIndex, 0, activeDialogue.SpheechCount - 1);

        UpdateDialoguePresenter();
    }

    private void EndDialogue()
    {
        speechIndex = 0;
        activeDialogue = null;
        currentSpeech = null;
        playerInput.EnablePlayerInput();
        gameObject.SetActive(false);
    }
}
