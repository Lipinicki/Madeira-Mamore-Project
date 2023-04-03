using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using MyBox;
using System.Threading.Tasks;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [field: SerializeField] 
	public PlayerInput PlayerInput { get; private set; }

	[field: SerializeField] 
	public Rigidbody MainRigidbody { get; private set; }

	[field: SerializeField] 
	public Animator MainAnimator { get; private set; }

	[field: SerializeField, ReadOnly, Tooltip("Represents the input value of the player")]
	public Vector3 InputVector { get; private set; }

	public Transform MainCameraTransform { get; private set; }
	public int KIdleAnimationParam { get { return kIdleAnimationParam; } }
	public ActionStates CurrentPlayerState { get; private set; } = ActionStates.Idle;
	public SubStates CurrentPlayerSubState { get; private set; } = SubStates.None;

	[Header("Character Movement")]
	[SerializeField, Tooltip("Tunes the IsGrounded sphereCheck position, the higher the value the lower the sphere will be")] 
	private float _groundOffset = 0.75f;

	[SerializeField, Tooltip("Radius of IsGrounded sphere")] 
	private float _groundedRadius = 0.4f;

	[SerializeField, Tooltip("Scales the Physics.gravity to a new gravity that is used instead")] 
	private float _gravityScale = 3f;

	[SerializeField, Tooltip("Represents how fast gravityContributionMultiplier will go back to 1f. The higher, the faster")] 
	private float _gravityContribution = 0f;

	[SerializeField, Tooltip("The factor which determines how much gravity is affecting verticalMovement")] 
	private float _gravityComeback = 5f;

	[SerializeField, Tooltip("Is multiplied each frame while jumping trying to 'cancel' gravity effect")] 
	private float _gravityDivider = 0.4f;

	[SerializeField, Tooltip("Maximun time which the player can hold the jump button")] 
	private float _jumpInputDuration = 0.4f;

	[SerializeField, Tooltip("Sets the player rb.velocity.y to this value when jump is pressed")] 
	private float _initialJumpForce = 7f;

	[SerializeField, Tooltip("Speed of players movement")] 
	private float _movementSpeed = 17f;

	[SerializeField, Tooltip("Speed in wich the player turn around its own axis")] 
	private float _rotationSpeed = 12f;

	[SerializeField, Tooltip("Used to clamp horizontal speed to prevent player walking fast")] 
	private float _maxHorizontalSpeed = 2.5f;

	[SerializeField, Tooltip("Used to clamp player's vertical speed to prevent high fall speeds")] 
	private float _maxVerticalSpeed = 50f;
	
	[Space]
	[SerializeField] 
	private List<LayerMask> _groundLayers;

	[Header("Vectors")]
	[SerializeField, ReadOnly, Tooltip("Force applied to move the rigidbody")] 
	private Vector3 _movementVector;

	private bool _canApplyMovement => (int)CurrentPlayerState < 3;
	private float _jumpBeginTime = Mathf.NegativeInfinity;
	
	private readonly int kWalkingAnimationParam = Animator.StringToHash("isWalking");
	private readonly int kIdleAnimationParam = Animator.StringToHash("StartIdle");

	//Adds listeners to events triggered in PlayerInput script
	void OnEnable()
	{
		PlayerInput.jumpEvent += OnJump;
		PlayerInput.jumpCanceledEvent += OnJumpCanceled;
		PlayerInput.moveEvent += OnMove;
	}

	//Removes all listeners to events coming from PlayerInput script
	void OnDisable()
	{
		PlayerInput.jumpEvent -= OnJump;
		PlayerInput.jumpCanceledEvent -= OnJumpCanceled;
		PlayerInput.moveEvent -= OnMove;
	}

	private void Start()
	{
		MainCameraTransform = Camera.main.transform;
		MainRigidbody.useGravity = false; //Disable Physics.gravity influence
		MainRigidbody.drag = 0.75f;
	}

	private void Update() {
		// set animation parameters
		MainAnimator.SetBool(kWalkingAnimationParam, CurrentPlayerState == ActionStates.Walking);
	}

	void FixedUpdate()
	{
		RaiseGravitycontribution();
		HandleJump();
		MovePlayer();
		RotatePlayer();
	}

	private void MovePlayer()
	{
		if (_canApplyMovement)
		{
			//Applies the movement to players input direction
			float currentMoveSpeed = CurrentPlayerSubState == SubStates.Interacting ?
			_movementSpeed * 0.45f : _movementSpeed;

			_movementVector = InputVector * currentMoveSpeed;
			ApplyGravity();  //Adds gravity

			//Moves the player
			MainRigidbody.AddForce(_movementVector * MainRigidbody.mass, ForceMode.Force);
			ClampsVelocity();
		}
	}

	private void RotatePlayer()
	{
		//Rotate to the movement direction
		UpdateFowardOrientation(_movementVector.normalized);
	}

	private void HandleJump()
	{
		if (CurrentPlayerState == ActionStates.Jumping)
		{
			//The player can only hold the Jump button for so long as
			//JumpInputDuration, _jumpBeginTime is setted when OnJump is
			//called
			if (Time.time >= _jumpBeginTime + _jumpInputDuration)
			{
				ResetPlayerState();
				_gravityContribution = 1f; //Gravity influence is reset to full effect
			}
			else
			{
				_gravityContribution *= _gravityDivider; //Reduce the gravity effect
			}
		}
	}

	private void RaiseGravitycontribution()
	{
		//Raises gravity contribution starting from 0f at the beginning of the jump
		//and raise it to a maximun of 1f
		if (!IsGrounded())
		{
			_gravityContribution += Time.fixedDeltaTime * _gravityComeback;
		}
	}

	public bool IsGrounded()
	{
		Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _groundOffset, transform.position.z);
		bool isGrounded = false;
		foreach (int layer in _groundLayers)
		{
			isGrounded = Physics.CheckSphere(spherePosition, _groundedRadius, layer, QueryTriggerInteraction.Ignore);
			if (isGrounded) break;
		}
		return isGrounded;
	}

	public bool IsWalking() => InputVector.magnitude > 0.01f && IsGrounded() ? true : false; 

	public void ResetPlayerState(bool resetSubstateToo = true)
	{	
		CurrentPlayerState = ActionStates.Idle;
		if (resetSubstateToo == false) return; 
		CurrentPlayerSubState = SubStates.None;
	}

	public void ChangePlayerState(ActionStates state, SubStates substate = SubStates.None)
	{
		CurrentPlayerState = state;
		if (substate == SubStates.None) return; 
		CurrentPlayerSubState = substate;
	}

	public void ChangePlayerSubstate(SubStates substate)
	{
		CurrentPlayerSubState = substate;
	}

	void ApplyGravity()
	{
		//Calculates a custom gravity for this rigidbody with contributions from gravity
		_gravityContribution = Mathf.Clamp01(_gravityContribution); //Clamps the contribution between 0 (jumping) and 1 (falling);
		Vector3 gravity = Physics.gravity * _gravityScale * _gravityContribution;
		MainRigidbody.AddForce(gravity, ForceMode.Acceleration);
	}

	void ClampsVelocity()
	{
		Vector3 xzVel = new Vector3(MainRigidbody.velocity.x, 0, MainRigidbody.velocity.z);
		Vector3 yVel = new Vector3(0, MainRigidbody.velocity.y, 0);

		xzVel = Vector3.ClampMagnitude(xzVel, _maxHorizontalSpeed);
		yVel = Vector3.ClampMagnitude(yVel, _maxVerticalSpeed);

		MainRigidbody.velocity = xzVel + yVel;
	}

    void UpdateFowardOrientation(Vector3 directionVector)
    {
        // Update character rotation based on movement direction
        if (directionVector.magnitude < 0.1f || CurrentPlayerSubState == SubStates.Interacting) return;
        
		Quaternion targetRotation = Quaternion.LookRotation(directionVector, Vector3.up);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * _rotationSpeed);
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

	// =============== INPPUT ACTIONS ===============
	// This section is deserved to actions triggered by input events

	void OnJump()
	{
		if (!IsGrounded() || CurrentPlayerState == ActionStates.Jumping) return;

		CurrentPlayerState = ActionStates.Jumping;
		MainRigidbody.velocity += new Vector3(0, _initialJumpForce, 0);
		_jumpBeginTime = Time.time; //Resets jump begin time
	}

	void OnJumpCanceled()
	{		
		ResetPlayerState();
	}

	void OnMove(Vector2 movement)
	{
		SetInputVector(movement);

		if (IsGrounded() && movement != Vector2.zero) ChangePlayerState(ActionStates.Walking);
		else if (IsGrounded() && movement == Vector2.zero) ResetPlayerState(resetSubstateToo: false);
	}

	// =============== GIZMOS ===============

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

