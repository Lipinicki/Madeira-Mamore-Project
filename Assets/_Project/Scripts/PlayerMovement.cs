using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public PlayerInput playerInput;

	[SerializeField] float _gravityScale = 1f;
	[SerializeField] float _groundOffset = .5f;
	[SerializeField] float _groundedRadius = .5f;
	[SerializeField] LayerMask _groundLayers;
	[SerializeField] Vector3 _inputVector;
	[SerializeField] Vector3 _movementVector;
	[SerializeField] float _verticalMovement;
	[SerializeField] float _movementSpeed;
	[SerializeField] float _maxVerticalSpeed = 50.0f;

	Rigidbody _rigidbody;

	//Adds listeners to events triggered in PlayerInput script
	void OnEnable()
	{
		playerInput.crouchEvent += OnCrouch;
		playerInput.crouchCanceledEvent += OnCrouchCanceled;
		playerInput.jumpEvent += OnJump;
		playerInput.jumpCanceledEvent += OnJumpCanceled;
		playerInput.moveEvent += OnMove;
	}

	//Removes all listeners to events coming from PlayerInput script
	void OnDisable()
	{
		playerInput.crouchEvent -= OnCrouch;
		playerInput.crouchCanceledEvent -= OnCrouchCanceled;
		playerInput.jumpEvent -= OnJump;
		playerInput.jumpCanceledEvent -= OnJumpCanceled;
		playerInput.moveEvent -= OnMove;
	}

	void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_rigidbody.useGravity = false; //Disable Physics.gravity influence
	}

	void FixedUpdate()
	{
		//Player falling
		if (!IsGrounded())
		{
			_verticalMovement += Physics.gravity.y * _gravityScale;
			_verticalMovement = Mathf.Clamp(_verticalMovement, -_maxVerticalSpeed, 100f);

			_movementVector = _inputVector * _movementSpeed;
			_movementVector.y = _verticalMovement;

			_rigidbody.AddForce(_movementVector, ForceMode.Acceleration); //Adds gravity
		}
		else
		{
			_verticalMovement = 0f; //Resets vertical velocity
		}

	}

	public bool IsGrounded()
	{
		Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - _groundOffset, transform.position.z);
		bool isGrounded = Physics.CheckSphere(spherePosition, _groundedRadius, _groundLayers, QueryTriggerInteraction.Ignore);
		return isGrounded;
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
		Debug.Log("Jump!");
	}

	void OnJumpCanceled()
	{
		Debug.Log("Jump Canceled!");
	}

	void OnMove(Vector2 movement)
	{
		_inputVector = new Vector3(movement.x, 0f, movement.y);
		Debug.Log("Move :" + movement);
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
