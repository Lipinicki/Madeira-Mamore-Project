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
    public PlayerInput PlayerInput;
	public string KIdleAnimationParam { get { return kIdleAnimationParam; } }
	public Vector3 InputVector { get { return _inputVector; } }
	public ActionStates CurrentPlayerState { get { return _currentPlayerState; } }
	public SubStates CurrentPlayerSubState { get { return _currentSubstate; } }

	[Header("Character Movement")]
	[SerializeField, Tooltip("Tunes the IsGrounded sphereCheck position, the higher the value the lower the sphere will be")] float _groundOffset = .5f;
	[SerializeField, Tooltip("Radius of IsGrounded sphere")] float _groundedRadius = .5f;
	[SerializeField, Tooltip("Scales the Physics.gravity to a new gravity that is used instead")] float _gravityScale = 1f;
	[SerializeField, Tooltip("Represents how fast gravityContributionMultiplier will go back to 1f. The higher, the faster")] float _gravityContribution = 0f;
	[SerializeField, Tooltip("The factor which determines how much gravity is affecting verticalMovement")] float _gravityComeback = 15f;
	[SerializeField, Tooltip("Is multiplied each frame while jumping trying to 'cancel' gravity effect")] float _gravityDivider = .6f;
	[SerializeField, Tooltip("Maximun time which the player can hold the jump button")] float _jumpInputDuration = .4f;
	[SerializeField, Tooltip("Sets the player rb.velocity.y to this value when jump is pressed")] float _initialJumpForce = 10f;
	[SerializeField, Tooltip("Speed of players movement")] float _movementSpeed = 10.0f;
	[SerializeField, Tooltip("Speed in wich the player turn around its own axis")] float _rotationSpeed = 12f;
	[SerializeField, Tooltip("Used to clamp horizontal speed to prevent player walking fast")] float _maxHorizontalSpeed = 10f;
	[SerializeField, Tooltip("Used to clamp player's vertical speed to prevent high fall speeds")] float _maxVerticalSpeed = 30.0f;
	
	[Space]
	[SerializeField] List<LayerMask> _groundLayers;

	[Header("Vectors")]
	[SerializeField, ReadOnly, Tooltip("Force applied to move the rigidbody")] Vector3 _movementVector;
	[SerializeField, ReadOnly] Vector3 _inputVector; 
	
	private bool blockVerticalAxis = false;
	private bool blockHorizontalAxis = false;
	private bool _canApplyMovement => (int)_currentPlayerState < 3;
	private float _jumpBeginTime = Mathf.NegativeInfinity;
	private ActionStates _currentPlayerState = ActionStates.Idle;
	private SubStates _currentSubstate = SubStates.None;
	private Rigidbody _rigidbody;
	private Animator _mainAnimator;

	private const string kWalkingAnimationParam = "isWalking";
	private const string kIdleAnimationParam = "StartIdle";

	//Adds listeners to events triggered in PlayerInput script
	void OnEnable()
	{
		PlayerInput.crouchEvent += OnCrouch;
		PlayerInput.crouchCanceledEvent += OnCrouchCanceled;
		PlayerInput.jumpEvent += OnJump;
		PlayerInput.jumpCanceledEvent += OnJumpCanceled;
		PlayerInput.moveEvent += OnMove;
	}

	//Removes all listeners to events coming from PlayerInput script
	void OnDisable()
	{
		PlayerInput.crouchEvent -= OnCrouch;
		PlayerInput.crouchCanceledEvent -= OnCrouchCanceled;
		PlayerInput.jumpEvent -= OnJump;
		PlayerInput.jumpCanceledEvent -= OnJumpCanceled;
		PlayerInput.moveEvent -= OnMove;
	}

	void Awake()
	{
		_mainAnimator = GetComponentInChildren<Animator>();
		_rigidbody = GetComponent<Rigidbody>();
		_rigidbody.useGravity = false; //Disable Physics.gravity influence
		_rigidbody.drag = 0.75f;
	}

	private void Update() {
		// set animation parameters
		_mainAnimator.SetBool(kWalkingAnimationParam, _currentPlayerState == ActionStates.Walking);
	}

	void FixedUpdate()
	{
		//Raises gravity contribution starting from 0f at the beginning of the jump
		//and raise it to a maximun of 1f
		if (!IsGrounded())
		{
			_gravityContribution += Time.fixedDeltaTime * _gravityComeback;
		}

		//Jumps
		if (_currentPlayerState == ActionStates.Jumping)
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
		

		if (_canApplyMovement)
		{
			//Applies the movement to players input direction
			float currentMoveSpeed = _currentSubstate == SubStates.Interacting ?
			_movementSpeed * 0.4f : _movementSpeed;

			_movementVector = _inputVector * currentMoveSpeed;
			ApplyGravity();  //Adds gravity
			
			//Moves the player
			_rigidbody.AddForce(_movementVector * _rigidbody.mass, ForceMode.Force);
			ClampsVelocity();
			
			//Rotate to the movement direction
			UpdateFowardOrientation(_movementVector.normalized);
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

	public bool IsWalking() => _inputVector.magnitude > 0.01f && IsGrounded() ? true : false; 

	public void ResetPlayerState(bool resetSubstateToo = true)
	{	
		_currentPlayerState = ActionStates.Idle;
		if (resetSubstateToo == false) return; 
		_currentSubstate = SubStates.None;
	}

	public void ChangePlayerState(ActionStates state, SubStates substate = SubStates.None)
	{
		_currentPlayerState = state;
		if (substate == SubStates.None) return; 
		_currentSubstate = substate;
	}

	public void ChangePlayerSubstate(SubStates substate)
	{
		_currentSubstate = substate;
	}

	void ApplyGravity()
	{
		//Calculates a custom gravity for this rigidbody with contributions from gravity
		_gravityContribution = Mathf.Clamp01(_gravityContribution); //Clamps the contribution between 0 (jumping) and 1 (falling);
		Vector3 gravity = Physics.gravity * _gravityScale * _gravityContribution;
		_rigidbody.AddForce(gravity, ForceMode.Acceleration);
	}

	void ClampsVelocity()
	{
		Vector3 xzVel = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
		Vector3 yVel = new Vector3(0, _rigidbody.velocity.y, 0);

		xzVel = Vector3.ClampMagnitude(xzVel, _maxHorizontalSpeed);
		yVel = Vector3.ClampMagnitude(yVel, _maxVerticalSpeed);

		_rigidbody.velocity = xzVel + yVel;
	}

    void UpdateFowardOrientation(Vector3 directionVector)
    {
        // Update character rotation based on movement direction
        if (directionVector.magnitude < 0.1f || _currentSubstate == SubStates.Interacting) return;
        
		Quaternion targetRotation = Quaternion.LookRotation(directionVector, Vector3.up);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * _rotationSpeed);
    }

	// =============== INPPUT ACTIONS ===============
	// This section is deserved to actions triggered by input events

	void OnCrouch()
	{

	}

	void OnCrouchCanceled()
	{

	}

	void OnJump()
	{
		if (!IsGrounded() || _currentPlayerState == ActionStates.Jumping) return;

		_currentPlayerState = ActionStates.Jumping;
		_rigidbody.velocity += new Vector3(0, _initialJumpForce, 0);
		_jumpBeginTime = Time.time; //Resets jump begin time
	}

	void OnJumpCanceled()
	{		
		ResetPlayerState();
	}

	void OnMove(Vector2 movement)
	{
		_inputVector = new Vector3(movement.x, 0f, movement.y);
		_inputVector.x *= blockHorizontalAxis ? 0f : 1f;
		_inputVector.z *= blockVerticalAxis ? 0f : 1f;

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

