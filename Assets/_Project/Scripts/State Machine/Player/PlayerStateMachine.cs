using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStateMachine : StateMachine
{
	[field: SerializeField]
	public PlayerInput PlayerInput { get; private set; }

	[field: SerializeField]
	public Rigidbody MainRigidbody { get; private set; }

	[field: SerializeField]
	public Animator MainAnimator { get; private set; }

	[field: SerializeField]
	public InteractableArea InteractableArea { get; private set; }

	[field: SerializeField]
	public Collider PlayerCollider { get; private set; }

	[field: Space(30f), SerializeField, ReadOnly, Tooltip("Represents the input value of the player")]
	public Vector3 InputVector { get; private set; }

	[field: SerializeField, ReadOnly, Tooltip("Force applied to move the rigidbody")]
	public Vector3 MovementVector { get; set; }

	[field: Space(30f), SerializeField, Tooltip("Speed of players movement")]
	public float MovementSpeed { get; private set; } = 17f;

	[field: SerializeField, Tooltip("Used to clamp horizontal speed to prevent player walking fast")]
	public float MaxHorizontalSpeed { get; private set; } = 2.5f;

	[field: SerializeField, Tooltip("Used to clamp player's vertical speed to prevent high fall speeds")]
	public float MaxVerticalSpeed { get; private set; } = 50f;

	[field: SerializeField, Tooltip("Speed in wich the player turn around its own axis")]
	public float RotationSpeed { get; private set; } = 12f;

	[field: Space(30f), SerializeField, Tooltip("Maximun time which the player can hold the jump button")]
	public float JumpInputDuration { get; private set; } = .4f;

	[field: SerializeField, Tooltip("Sets the player rb.velocity.y to this value when jump is pressed")]
	public float InitialJumpForce { get; private set; } = 7f;

	[field: Space(30f), SerializeField, Tooltip("Scales the Physics.gravity to a new gravity that is used instead")]
	public float GravityScale { get; private set; } = 3f;

	[field: SerializeField, Tooltip("Represents how fast gravityContributionMultiplier will go back to 1f. The higher, the faster")]
	public float GravityContribution { get; set; } = 0f;

	[field: SerializeField, Tooltip("The factor which determines how much gravity is affecting verticalMovement")]
	public float GravityComeback { get; private set; } = 5f;

	[field: SerializeField, Tooltip("Is multiplied each frame while jumping trying to 'cancel' gravity effect")]
	public float GravityDivider { get; private set; } = 0.4f;

	public Transform MainCameraTransform { get; private set; }

	public float JumpBeginTime { get; set; } = Mathf.NegativeInfinity;

	[Space(30f), SerializeField, Tooltip("Tunes the IsGrounded sphereCheck position, the higher the value the lower the sphere will be")]
	private float _groundOffset = 0.75f;

	[SerializeField, Tooltip("Radius of IsGrounded sphere")]
	private float _groundedRadius = 0.4f;

	[SerializeField]
	private LayerMask _groundLayers;

	[SerializeField]
	private UnityEvent OnInteractEvent;

	[field: Space(30f), SerializeField] 
	public float LadderClimbingSpeed { get; private set; } = 5f;

	public Transform ActiveLadder { get; set; }

	private const string kLadderTag = "Ladders";

	private void OnEnable()
	{
		PlayerInput.moveEvent += OnMove;
	}

	private void OnDisable()
	{
		PlayerInput.moveEvent -= OnMove;
	}

	private void Start()
	{
		SwitchCurrentState(new PlayerIdleState(this));

		MainCameraTransform = Camera.main.transform;
		MainRigidbody.useGravity = false; //Disable Physics.gravity influence
		MainRigidbody.drag = 0.75f;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == kLadderTag) ActiveLadder = other.transform;
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.transform.tag == kLadderTag) ActiveLadder = null;
	}

	public void TriggerInteractiobnEvent()
	{
		OnInteractEvent?.Invoke();
	}

	public bool IsGrounded()
	{
		Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _groundOffset, transform.position.z);
		bool isGrounded = Physics.CheckSphere(spherePosition, _groundedRadius, _groundLayers, QueryTriggerInteraction.Ignore);
		return isGrounded;
	}

	public void ApplyGravity()
	{
		//Calculates a custom gravity for this rigidbody with contributions from gravity
		GravityContribution = Mathf.Clamp01(GravityContribution); //Clamps the contribution between 0 (jumping) and 1 (falling);
		Vector3 gravity = Physics.gravity * GravityScale * GravityContribution;
		MainRigidbody.AddForce(gravity, ForceMode.Acceleration);
	}

	void SetInputVector(Vector2 movement)
	{
		Vector3 foward = MainCameraTransform.forward;
		Vector3 right = MainCameraTransform.right;

		foward.y = 0f;
		right.y = 0f;

		foward.Normalize();
		right.Normalize();

		InputVector = foward * movement.y + right * movement.x;
	}

	void OnMove(Vector2 movement)
	{
		SetInputVector(movement);
	}

	void OnDrawGizmosSelected()
	{
		Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
		Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

		if (IsGrounded()) Gizmos.color = transparentGreen;
		else Gizmos.color = transparentRed;

		//When selected draw a gizmo that matches the position and the radius of the grounded collider
		Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - _groundOffset, transform.position.z), _groundedRadius);
	}
}
