using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using MyBox;

public class PauseScreenPresenter : MonoBehaviour
{
    [SerializeField] private Button returnOptions;
    [SerializeField] private Button gotoOptions;
    [SerializeField] private Button gotoDesktop;
    [SerializeField] private GameObject optionsPresenter;
    private bool optionsIsOpen = false;

    private void OnEnable()
    {
        returnOptions?.onClick.AddListener(ToggleOptions);
        gotoOptions?.onClick.AddListener(ToggleOptions);
        gotoDesktop?.onClick.AddListener(GotoDesktop);
    }

    private void OnDisable()
    {
        gotoOptions?.onClick.RemoveAllListeners();
        gotoDesktop?.onClick.RemoveAllListeners();
    }

    private void ToggleOptions()
    {
        Debug.Log(optionsIsOpen);
        optionsIsOpen = optionsIsOpen ? false : true;
        optionsPresenter.SetActive(optionsIsOpen);
    }

    private void GotoDesktop() => Application.Quit();
}
