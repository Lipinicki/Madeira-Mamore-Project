using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCameraSwitch : MonoBehaviour
{
	[SerializeField, Tooltip("Camera that the active camera in the scene will be transitioned to")] CinemachineVirtualCamera _targetCamera; 
	
	CameraManager _cameraManager;

	void Awake()
	{
		_cameraManager = FindObjectOfType<CameraManager>();
	}

	void OnTriggerEnter(Collider other)
	{
		if (_cameraManager == null) return;

		_cameraManager.SwitchActiveCamera(_targetCamera);
	}
}
