using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Class used to manage cinemachine cameras in the scene and wich camera is being used at the moment
public class CameraManager : MonoBehaviour
{
    [SerializeField, Tooltip("The camera wich will follow the player at the start of the scene")] 
	private CinemachineVirtualCamera startCamera;

	[SerializeField, Tooltip("The list of all CinemachineCameras in the Scene")] 
	private List<CinemachineVirtualCamera> cameras = new List<CinemachineVirtualCamera>();

	// Tracks the current Live CM camera in the scene, which the priority is the highest
    private CinemachineVirtualCamera _activeCamera;

	private const int kLowestPriority = 10;
	private const int kHighestPriority = 20;

	private void OnValidate()
	{
		// Switch the active camera on editor mode
		ResetCamerasPriority();
	}

	void Start()
	{
		ResetCamerasPriority();
	}

	// Sets all cameras priority to the lowest and sets active camera to the start camera
	void ResetCamerasPriority()
	{
		foreach (var cam in cameras)
		{
			cam.Priority = kLowestPriority;
		}

		if (startCamera != null)
		{
			_activeCamera = startCamera;

			_activeCamera.Priority = kHighestPriority;
		}
	}

	// Switches the active camera to the specified camera
	public void SwitchActiveCamera(CinemachineVirtualCamera targetCamera)
	{
		if (targetCamera == null) return;

		foreach (var cam in cameras)
		{
			cam.Priority = kLowestPriority;
		}

		targetCamera.Priority = kHighestPriority;

		_activeCamera = targetCamera;
	}
}
