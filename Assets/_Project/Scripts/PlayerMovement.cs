using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public PlayerInput PlayerInput;

	[SerializeField] float _groundOffset = .5f; //Tunes the IsGrounded sphereCheck position, the higher the value the lower the sphere will be
	[SerializeField] float _groundedRadius = .5f; //Radius of IsGrounded sphere
	[SerializeField] float _gravityScale = 1f; //Scales the Physics.gravity to a new gravity that is used instead
	[SerializeField] float _gravityContribution = 0f; //Represents how fast gravityContributionMultiplier will go back to 1f. The higher, the faster
	[SerializeField] float _gravityComeback = 15f; //The factor which determines how much gravity is affecting verticalMovement
	[SerializeField] float _gravityDivider = .6f; //Is multiplied each frame while jumping trying to "cancel" gravity effect
	[SerializeField] float _jumpInputDuration = .4f; //Maximun time which the player can hold the jump button
	[SerializeField] float _initialJumpForce = 10f; //Sets the player rb.velocity.y to this value when jump is pressed
	[SerializeField] float _movementSpeed = 10.0f; //Speed of players movement
	[SerializeField] float _maxHorizontalSpeed = 10f; //Used to clamp horizontal speed to prevent player walking fast
	[SerializeField] float _maxVerticalSpeed = 30.0f; //Used to clamp player's vertical speed to prevent high fall speeds
	[SerializeField] bool _isJumping;
	[SerializeField] LayerMask _groundLayers;
	[SerializeField] Vector3 _inputVector; 
	[SerializeField] Vector3 _movementVector; //Force applied to move the rigidbody

	float _jumpBeginTime = Mathf.NegativeInfinity;
	Rigidbody _rigidbody;

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
		_rigidbody = GetComponent<Rigidbody>();
		_rigidbody.useGravity = false; //Disable Physics.gravity influence
		_rigidbody.drag = 0.7f;
	}

	void FixedUpdate()
	{
		//Raises gravity contribution starting from 0f at the beginning of the jump
		//and raise it to a maximun of 1f
		if (!IsGrounded())
		{
			_gravityContribution += Time.deltaTime * _gravityComeback;
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
			
		//Player falling
		if (!IsGrounded())
		{

		}
		else
		{
			
		}

		//Calculates a custom gravity for this rigidbody with contributions from gravity
		_gravityContribution = Mathf.Clamp01(_gravityContribution); //Clamps the contribution between 0 (jumping) and 1 (falling)
		Vector3 gravity = Physics.gravity * _gravityScale * _gravityContribution;
		_rigidbody.AddForce(gravity, ForceMode.Acceleration); //Adds gravity

		//Moves the player
		_rigidbody.AddForce(_movementVector, ForceMode.Force);

		ClampsVelocity();

		//Rotate to the movement direction
		_movementVector.y = 0f;
		if (_movementVector.sqrMagnitude >= .02f)
		{
			transform.forward = _movementVector.normalized;
		}
	}

	public bool IsGrounded()
	{
		Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _groundOffset, transform.position.z);
		bool isGrounded = Physics.CheckSphere(spherePosition, _groundedRadius, _groundLayers, QueryTriggerInteraction.Ignore);
		return isGrounded;
	}

	void ClampsVelocity()
	{
		Vector3 xzVel = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
		Vector3 yVel = new Vector3(0, _rigidbody.velocity.y, 0);

		xzVel = Vector3.ClampMagnitude(xzVel, _maxHorizontalSpeed);
		yVel = Vector3.ClampMagnitude(yVel, _maxVerticalSpeed);

		_rigidbody.velocity = xzVel + yVel;
	}

	// =============== PLAYER ACtIONS ===============

	void OnCrouch()
	{
		Debug.Log("Crouch!");
	}

	void OnCrouchCanceled()
	{
		Debug.Log("Crouch Canceled!");
	}

	void OnJump()
	{
		if (IsGrounded())
		{
			_isJumping = true;
			_rigidbody.velocity += new Vector3(0, _initialJumpForce, 0);
			_jumpBeginTime = Time.time; //Resets jump begin time
			Debug.Log("Jump!");
		}
	}

	void OnJumpCanceled()
	{
		_isJumping = false;
		Debug.Log("Jump Canceled!");
	}

	void OnMove(Vector2 movement)
	{
		_inputVector = new Vector3(movement.x, 0f, movement.y);
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
