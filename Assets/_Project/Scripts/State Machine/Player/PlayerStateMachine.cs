using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStateMachine : StateMachine
{
	
	[field: SerializeField, Header("Essential Components")]
	public PlayerInput PlayerInput { get; private set; }

	[field: SerializeField]
	public PlayerSound PlayerSound { get; private set; }

	[field: SerializeField]
	public Rigidbody MainRigidbody { get; private set; }

	[field: SerializeField]
	public Animator MainAnimator { get; private set; }

	[field: SerializeField]
	public InteractableArea InteractableArea { get; private set; }

	[field: SerializeField]
	public CapsuleCollider PlayerCollider { get; private set; }
	
	[field: SerializeField]
	public Transform PlayerVisualTransform { get; private set; }

	[field: SerializeField]
	public Inventory PlayerInventory { get; private set; }

	[field: Space(30f), SerializeField, ReadOnly, Tooltip("Represents the input value of the player")]
	public Vector3 InputVector { get; private set; }

	[field: SerializeField, ReadOnly, Tooltip("Force applied to move the rigidbody")]
	public Vector3 MovementVector { get; set; }

	[field: Header("Movement Parameters"), Space(30f), SerializeField, Tooltip("Speed of players movement")]
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
	private float _groundOffset = 0.67f;

	[SerializeField, Tooltip("Radius of IsGrounded sphere")]
	private float _groundedRadius = 0.38f;

	[SerializeField]
	private LayerMask _groundLayers;

	[Space(30f), SerializeField]
	private UnityEvent OnInteractEvent;
	
	[field: Space(30f), SerializeField, Header("Ladder State")]
	public float LadderClimbingSpeed { get; private set; } = 5f;

	[field: SerializeField]
	public LayerMask LadderLayers { get; private set; }

	[field: SerializeField]
	public float RayCastOffset { get; private set; } = -0.75f;

	[field: SerializeField]
	public float RayCastMaxDistance { get; private set; } = 0.9f;

	[field: SerializeField]
	public float LadderStartOffsetHeight { get; private set; } = 0.5f;

	[field: SerializeField]
	public float LadderStartOffsetFoward { get; private set; } = 0.5f;

	[field: SerializeField]
	public float ForceToLeftLadder { get; private set; } = 9f;
	public Transform ActiveLadder { get; set; }
	public float StandingHeight { get; private set; }


	[field: Space(30f), SerializeField, Header("Pushing State")]
	public float MinInteractionDistance { get; private set; } = 1f;
	
	[field: SerializeField]
    public float BlockMovementSpeed { get; private set; } = 2f; // Force applied to the block when pushed	
	public float BlockOffset { get; private set; } = 1f;
	public float MaxInteractionDistance { get; private set; } 
    public LayerMask PushBlocksLayer;
    public Rigidbody ActiveBlock { get; private set; } = null;


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
		MainCameraTransform = Camera.main.transform;
		MainRigidbody.useGravity = false; //Disable Physics.gravity influence
		MainRigidbody.drag = 0.75f;
		StandingHeight = PlayerCollider.height;
		PlayerInventory.ClearInventory();

		SwitchCurrentState(new PlayerIdleState(this));
	}

	public void TriggerInteractionEvent()
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

		Ray ray = new Ray(
			new Vector3(
			transform.position.x,
			transform.position.y + RayCastOffset,
			transform.position.z
			),
			transform.forward
			);

		Debug.DrawRay(ray.origin, ray.direction * RayCastMaxDistance);
	}

	public void SetupActiveBlock(Transform t)
	{
		Rigidbody rb = t.GetComponent<Rigidbody>();
		var blockDepth = t.GetComponent<Collider>().bounds.size.z;
		
		BlockOffset = blockDepth;
		ActiveBlock = rb;
	}

	public void DetachBlock()
	{
		if (ActiveBlock != null)
		{
			ActiveBlock.velocity = Vector3.zero;
			ActiveBlock.angularVelocity = Vector3.zero;
			MaxInteractionDistance = 0f;
			ActiveBlock = null;
		}
	}
}
