using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsOutPuzzle : MonoBehaviour
{
    [SerializeField] private List<PuzzleLever> levers = new List<PuzzleLever>();
	[SerializeField] private List<bool> initialCombination = new List<bool>();

	[Space(15)]
	[SerializeField] private InteractableEventBehaviour CompletionLock;
	[SerializeField] private MeshRenderer CompletionVisual;

	[Space(15)]
	[SerializeField] private Material disabledMaterial;
    [SerializeField] private Material enabledMaterial;
	[SerializeField] private Material completedMaterial;

	private void OnEnable()
	{
		for (int i = 0; i < levers.Count; i++)
		{
			Material initialMaterial = initialCombination[i] ? enabledMaterial : disabledMaterial;

			levers[i].Initialize(initialCombination[i], initialMaterial, i);
			levers[i].OnLeverInteraction += OnActivation;
		}

		HandleCompletion();
	}

	private void OnDisable()
	{
		for (int i = 0; i < levers.Count; i++)
		{
			levers[i].OnLeverInteraction -= OnActivation;
		}
	}

	public bool IsComplete()
	{
		for (int i = 0; i < levers.Count; i++)
		{
			if (!levers[i].Active)
			{
				return false;
			}
		}
		return true;
	}

	private void OnActivation(int id)
	{
		ChangeLeverState(id);

		if (!(id - 1 < 0)) // If is not less than zero
		{
			ChangeLeverState(id - 1);
		}

		if (!(id + 1 == levers.Count)) // If is not greater than levers Count
		{
			ChangeLeverState(id + 1);
		}

		HandleCompletion();
	}

	private void ChangeLeverState(int id)
	{
		levers[id].Activate();

		Material visualMat = levers[id].Active ? enabledMaterial : disabledMaterial;

		levers[id].ChangeVisualState(visualMat);
	}

	private void HandleCompletion()
	{
		Material visualMat = IsComplete() ? completedMaterial : disabledMaterial;

		CompletionVisual.material = visualMat;

		CompletionLock.enabled = IsComplete();
	}
}
