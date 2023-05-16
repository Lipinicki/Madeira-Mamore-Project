using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer _mainMixer;

    [Space(10)]
    [Header("First selection when active")]
    [SerializeField] private GameObject _firstSelection;

	private void OnEnable()
	{
        StartCoroutine(DelaySelection());
	}

    // Menu Options

	public void SetMasterVolume(float volume)
    {
        _mainMixer.SetFloat("MasterVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
		_mainMixer.SetFloat("SFXVolume", volume);
	}

    public void SetUIVolume(float volume)
    {
		_mainMixer.SetFloat("UIVolume", volume);
	}

    public void SetDialoguesVolume(float volume)
    {
		_mainMixer.SetFloat("DialoguesVolume", volume);
	}

    public void SetMusicVolume(float volume)
    {
		_mainMixer.SetFloat("MusicVolume", volume);
	}

    public void SetFullScreen(bool value)
    {
        Screen.fullScreen = value;
#if UNITY_EDITOR
        Debug.Log("Is FullScreen = " + value);
#endif
    }

    // Selection when active

    private IEnumerator DelaySelection()
    {
        yield return new WaitForSecondsRealtime(.2f);

        SelectFirsObject();
    }

    private void SelectFirsObject()
    {
        EventSystem.current.SetSelectedGameObject(_firstSelection);
	}
}
