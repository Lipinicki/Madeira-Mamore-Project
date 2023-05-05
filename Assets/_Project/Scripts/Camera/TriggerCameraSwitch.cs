using Cinemachine;
using UnityEngine;
using MyBox;

public class TriggerCameraSwitch : MonoBehaviour
{
	[SerializeField, Tooltip("Camera that the active camera in the scene will be transitioned to")] private CinemachineVirtualCamera _targetCamera; 
	[SerializeField, Tooltip("True if this trigger should swap between two cameras everytime you pass through it")] private bool switchBetween = false;
	[ConditionalField(nameof(switchBetween))][SerializeField] private CinemachineVirtualCamera secondTargetCamera; 

	CameraManager _cameraManager;
	private int internalControlDigit = 0;

	void Awake()
	{
		_cameraManager = FindObjectOfType<CameraManager>();
	}

	void OnTriggerEnter(Collider other)
	{
		if (_cameraManager == null) return;

		if (other.gameObject.CompareTag("Player"))
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

		bool goToFirstTarget = (internalControlDigit % 2 == 0);

		if (goToFirstTarget)
		{
			internalControlDigit++;
			return _targetCamera;
		}
		else
		{
			internalControlDigit--;
			return secondTargetCamera;
		}
	}

}
