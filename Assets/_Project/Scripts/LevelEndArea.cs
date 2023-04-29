using UnityEngine;

public class LevelEndArea : MonoBehaviour
{
	// Trigger level end corroutines or other events like changing to next scene
	[SerializeField] private GameEvent OnLevelEnd;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			OnLevelEnd?.Raise();

			// Deactivates to stop double trigger
			this.enabled = false;
		}
	}
}
