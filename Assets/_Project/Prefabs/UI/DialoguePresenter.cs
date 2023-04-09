using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class DialoguePresenter : MonoBehaviour
{
    [Header("Visual Components")]
    [SerializeField] private Image portrait;
    [SerializeField] private Image portraitShadow;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text characterName;
    
    [Space]

    [Header("Dialogue Actions")]
    [SerializeField] private AudioSource audioScr;
    [SerializeField] private Button returnButton;
    [SerializeField] private Button advanceButton;

    private Dialogue activeDialogue;
    private Speech currentSpeech;
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
        activeDialogue = dialogue;
        UpdateDialoguePresenter();
    }

    private void UpdateDialoguePresenter()
    {
        List<Speech> speechies = activeDialogue.speechList.ToList();

        if (speechies.IndexOf(currentSpeech) == speechIndex) return;

        currentSpeech = speechies[speechIndex];

        portrait.sprite = currentSpeech.characterPortrait;
        portraitShadow.sprite = currentSpeech.characterPortrait;
        characterName.SetText(currentSpeech.characterName);
        dialogueText.SetText(currentSpeech.speechText);
        
        if (currentSpeech.hasAudio)
        { 
            audioScr.clip = currentSpeech.speechAudio;
            audioScr.Play();
        }
    }

    public void AdvanceDialogue()
    {
        if (activeDialogue == null) return;

        speechIndex++;
        speechIndex = Math.Clamp(speechIndex, 0, activeDialogue.SpheechCount - 1);
        
        UpdateDialoguePresenter();
    }

    public void ReturnDialogue()
    {
        if (activeDialogue == null) return;

        speechIndex--;
        speechIndex = Math.Clamp(speechIndex, 0, activeDialogue.SpheechCount - 1);

        UpdateDialoguePresenter();
    }
}
