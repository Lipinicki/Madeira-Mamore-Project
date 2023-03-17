using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using MyBox;
using System.Threading.Tasks;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public PlayerInput PlayerInput;
	
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
	[SerializeField, Tooltip("Speed in wich the player grabs a ledge")] float _ledgeGrabSpeed = 2f;
	[SerializeField, ReadOnly] LayerMask _groundLayers;
	
	[Header("Vectors")]
	[SerializeField, ReadOnly, Tooltip("Force applied to move the rigidbody")] Vector3 _movementVector;
	[SerializeField, ReadOnly] Vector3 _inputVector; 
	
	[Header("Adtional Transforms")]
	[SerializeField] private Transform grabDetectionOrigin;
	
	private Vector3 _standPosition = Vector3.zero;
	private Vector3 _lerpDestination = Vector3.zero;
	private bool _isHoldingLedge = false;
	private bool _isJumping;
	private bool _isMoving => _movementVector != Vector3.zero && !_isHoldingLedge;
	private bool _isLerping = false;
	private float _jumpBeginTime = Mathf.NegativeInfinity;
	private float _lerpStartTime;

	private Rigidbody _rigidbody;
	private Animator _mainAnimator;
	private RaycastHit grabHit;

	private const string kLedgeLayer = "Ledges";
	private const string kWalkingAnimationParam = "isWalking";
	private const string kGrabAnimationParam = "isGrabingLedge";
	private const string kClimbAnimationParam = "StartClimb";
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
		_mainAnimator.SetBool(kWalkingAnimationParam, _isMoving);
		_mainAnimator.SetBool(kGrabAnimationParam, _isHoldingLedge);		
		HandleGrabLerp();
	}

	void FixedUpdate()
	{
		//Checks if can grab a ledge and handle transform interpolation
		HandleLedgeGrab();

		//Raises gravity contribution starting from 0f at the beginning of the jump
		//and raise it to a maximun of 1f
		if (!IsGrounded())
		{
			_gravityContribution += Time.fixedDeltaTime * _gravityComeback;
		}

		//Jumps
		if (_isJumping)
		{
			//The player can only hold the Jump button for so long as
			//JumpInputDuration, _jumpBeginTime is setted when OnJump is
			//called
			if (Time.time >= _jumpBeginTime + _jumpInputDuration)
			{
				_isJumping = false;
				_gravityContribution = 1f; //Gravity influence is reset to full effect
			}
			else
			{
				_gravityContribution *= _gravityDivider; //Reduce the gravity effect
			}
		}

		//Applies the movement to players input direction
		_movementVector = _inputVector * _movementSpeed;

		if (!_isHoldingLedge)
		{
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
		bool isGrounded = Physics.CheckSphere(spherePosition, _groundedRadius, _groundLayers, QueryTriggerInteraction.Ignore);
		return isGrounded;
	}

	private void ApplyGravity()
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

    private void UpdateFowardOrientation(Vector3 directionVector)
    {
        // Update character rotation based on movement direction
        if (directionVector.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionVector, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * _rotationSpeed);
        }
    }
	// =============== PLAYER ACtIONS ===============

	private void HandleLedgeGrab()
	{
		// Setup holding state
		if (IsGrounded() || _isHoldingLedge) return;

		if (Physics.Raycast(grabDetectionOrigin.position, grabDetectionOrigin.forward, out grabHit, 5f, LayerMask.GetMask(kLedgeLayer)))
		{	
			_rigidbody.isKinematic = true;
			_isJumping = false;
			_isHoldingLedge = true;

			float heightOffset = Vector3.Distance(transform.position, grabDetectionOrigin.position) - 0.15f;
			_lerpDestination = new Vector3(grabHit.point.x, grabHit.point.y - heightOffset, grabHit.point.z);
			_standPosition = grabHit.point + new Vector3(transform.forward.x, transform.forward.y + heightOffset, transform.forward.z);
			_lerpStartTime = Time.time;

        	_isLerping = true;
		}
	}

	private void HandleGrabLerp()
	{
		// Smoothly moves the player towards a ledge
		if (!_isLerping) return;
		
		float timeSinceStarted = Time.time - _lerpStartTime;
		float percentageComplete = timeSinceStarted / _ledgeGrabSpeed;

		transform.position = Vector3.Lerp(transform.position, _lerpDestination, percentageComplete);

		if (percentageComplete >= 1.0f)
		{
			_isLerping = false;
		}
		
	}

	private void ReleaseGrab()
	{
		//Reset holding state
		_isHoldingLedge = false;
		_rigidbody.isKinematic = false;
	}
	
	public async void Climb()
	{
		//Transitions the player from the ledge to the climbing state
		_mainAnimator.SetTrigger(kClimbAnimationParam);
		await Task.Delay(100);
		
		float currentClipLenght = _mainAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
		int delayInMiliseconds = (int)(currentClipLenght * 1000);
		await Task.Delay(delayInMiliseconds);
		
		transform.position = _standPosition;
		_mainAnimator.SetTrigger(kIdleAnimationParam);
		_rigidbody.isKinematic = false;
		_isHoldingLedge = false;
	}

	void OnCrouch()
	{

	}

	void OnCrouchCanceled()
	{

	}

	void OnJump()
	{
		if (!IsGrounded() || _isHoldingLedge) return;

		_isJumping = true;
		_rigidbody.velocity += new Vector3(0, _initialJumpForce, 0);
		_jumpBeginTime = Time.time; //Resets jump begin time
	}

	void OnJumpCanceled()
	{
		_isJumping = false;
	}

	void OnMove(Vector2 movement)
	{
		_inputVector = new Vector3(movement.x, 0f, movement.y);

		if (_inputVector.z > 0 && _isHoldingLedge) Climb();
		else if (_inputVector.z < 0 && _isHoldingLedge) ReleaseGrab();
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
