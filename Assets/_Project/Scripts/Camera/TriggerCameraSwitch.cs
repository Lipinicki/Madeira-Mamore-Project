using Cinemachine;
using UnityEngine;
using MyBox;

public class TriggerCameraSwitch : MonoBehaviour
{
	[SerializeField, Tooltip("Camera that the active camera in the scene will be transitioned to")] private CinemachineVirtualCamera _targetCamera; 
	[SerializeField, Tooltip("True if this trigger should swap between two cameras everytime you pass through it")] private bool switchBetween = false;
	[ConditionalField(nameof(switchBetween))][SerializeField] private CinemachineVirtualCamera secondTargetCamera; 

	[SerializeField] private CameraManager _cameraManager;

	private const string kPlayerTag = "Player";

	private bool goToFirst = true;

	void OnTriggerEnter(Collider other)
	{
		if (_cameraManager == null) return;

		if (other.gameObject.CompareTag(kPlayerTag))
		{
			CinemachineVirtualCamera desiredCam = GetDesiredCamera();
			_cameraManager.SwitchActiveCamera(desiredCam);
#if UNITY_EDITOR
			Debug.Log("Transitioning to: " + desiredCam.name);
#endif
		}
	}

	private CinemachineVirtualCamera GetDesiredCamera()
	{
		if (!switchBetween) return _targetCamera;

		if (goToFirst)
		{
			goToFirst = false;
			return _targetCamera;
		}
		else
		{
			goToFirst = true;
			return secondTargetCamera;
		}
	}

}
