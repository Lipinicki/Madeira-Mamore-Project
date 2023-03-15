using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Class used to manage cinemachine cameras in the scene and wich camera is being used at the moment
public class CameraManager : MonoBehaviour
{
	[HideInInspector] public List<CinemachineVirtualCamera> Cameras = new List<CinemachineVirtualCamera>(); //All cameras in the scene

    [SerializeField, Tooltip("The camera wich will follow the player at the start of the scene")] CinemachineVirtualCamera _startCamera;

    CinemachineVirtualCamera _activeCamera;

	void Awake()
	{
		FindAllCameras();
	}

	void Start()
	{
		ResetCamerasPriority();
	}

	void FindAllCameras()
	{
		Cameras = FindObjectsOfType<CinemachineVirtualCamera>().ToList();
	}

	void ResetCamerasPriority()
	{
		foreach (var cam in Cameras)
		{
			cam.Priority = 10;
		}

		if (_startCamera != null)
		{
			_activeCamera = _startCamera;

			_activeCamera.Priority = 20;
		}
	}

	public void SwitchActiveCamera(CinemachineVirtualCamera targetCamera)
	{
		if (targetCamera == null) return;

		targetCamera.Priority = 20;

		_activeCamera.Priority = 10;

		_activeCamera = targetCamera;
	}
}
