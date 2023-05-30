using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightsOutPuzzle : MonoBehaviour
{
    [SerializeField] private List<PuzzleLever> levers = new List<PuzzleLever>();
	[SerializeField] private List<bool> initialCombination = new List<bool>();

    [SerializeField] private Material disabledMaterial;
    [SerializeField] private Material enabledMaterial;

	private void OnEnable()
	{
		for (int i = 0; i < levers.Count; i++)
		{
			Material initialMaterial = initialCombination[i] ? enabledMaterial : disabledMaterial;

			levers[i].Initialize(initialCombination[i], initialMaterial, i);
			levers[i].OnLeverInteraction += OnActivation;
		}
	}

	private void OnDisable()
	{
		for (int i = 0; i < levers.Count; i++)
		{
			levers[i].OnLeverInteraction -= OnActivation;
		}
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
	}

	private void ChangeLeverState(int id)
	{
		levers[id].Activate();

		Material visualMat = levers[id].Active ? enabledMaterial : disabledMaterial;

		levers[id].ChangeVisualState(visualMat);
	}
}
