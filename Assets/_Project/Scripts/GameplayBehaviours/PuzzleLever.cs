using UnityEngine;
using UnityEngine.Events;

public class PuzzleLever : MonoBehaviour, IInteractable
{
	public bool Active { get { return active; } }

	public UnityAction<int> OnLeverInteraction;

	[SerializeField] private MeshRenderer visualState;
	[SerializeField] private Animator leverAnimator;

	private int ID;
	private bool active;
	private bool isInteracting;

	private readonly int rLeverActivate = Animator.StringToHash("Activate");
	private readonly int rLeverIdle = Animator.StringToHash("Idle");

	public void Interact()
	{
		if (isInteracting) return;

		isInteracting = true;

		OnLeverInteraction?.Invoke(ID);

		leverAnimator?.SetTrigger(rLeverActivate);
	}

	// Made so de state could be initialized as some inputed combination
	public void Initialize(bool state, Material visualStateMat, int id)
	{
		active = state;
		visualState.material = visualStateMat;
		ID = id;
	}

	// Provides an interface to change the levers state
	public void Activate()
	{
		active = !active;
	}

	// Provides an interface to change the levers visual state
	public void ChangeVisualState(Material visualStateMat)
	{
		visualState.material = visualStateMat;
	}

	public void SetIdle()
	{
		leverAnimator?.SetTrigger(rLeverIdle);
		isInteracting = false;
	}
}
