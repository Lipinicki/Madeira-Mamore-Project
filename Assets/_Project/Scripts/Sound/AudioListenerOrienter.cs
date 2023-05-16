using UnityEngine;

// Orients audio listener to match MainCamera rotation
public class AudioListenerOrienter : MonoBehaviour
{
	private void LateUpdate()
	{
		if (Camera.main != null)
		{
			transform.forward = Camera.main.transform.forward;
		}
	}
}
