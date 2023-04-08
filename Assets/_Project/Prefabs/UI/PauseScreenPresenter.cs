using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using MyBox;

public class PauseScreenPresenter : MonoBehaviour
{
    [SerializeField] private bool isMainMenu = false;
    [ConditionalField(nameof(isMainMenu))][SerializeField] private string playSceneName = kMainMenuScene;
    [ConditionalField(nameof(isMainMenu))][SerializeField] private Button playButton;
    [ConditionalField(nameof(isMainMenu), true)][SerializeField] private Button gotoMainMenu;
    [SerializeField] private Button returnOptions;
    [SerializeField] private Button gotoOptions;
    [SerializeField] private Button gotoDesktop;
    [SerializeField] private GameObject optionsPresenter;
    private bool optionsIsOpen = false;

    public const string kMainMenuScene = "MainMenuScene";

    private void OnEnable()
    {
        playButton?.onClick.AddListener(StartGame);
        returnOptions?.onClick.AddListener(ToggleOptions);
        gotoOptions?.onClick.AddListener(ToggleOptions);
        gotoMainMenu?.onClick.AddListener(GoToMainMenu);
        gotoDesktop?.onClick.AddListener(GotoDesktop);
    }

    private void OnDisable()
    {
        gotoOptions?.onClick.RemoveAllListeners();
        gotoMainMenu?.onClick.RemoveAllListeners();
        gotoDesktop?.onClick.RemoveAllListeners();
    }

    private void ToggleOptions()
    {
        Debug.Log(optionsIsOpen);
        optionsIsOpen = optionsIsOpen ? false : true;
        optionsPresenter.SetActive(optionsIsOpen);
    }

    private void GoToMainMenu() => SceneManager.LoadScene(kMainMenuScene);
    private void StartGame() => SceneManager.LoadScene(playSceneName);
    private void GotoDesktop() => Application.Quit();
}
