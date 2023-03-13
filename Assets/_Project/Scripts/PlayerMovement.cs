using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerInput playerInput;

	//Adds listeners to events triggered in PlayerInput script
	private void OnEnable()
	{
		playerInput.crouchEvent += OnCrouch;
		playerInput.crouchCanceledEvent += OnCrouchCanceled;
		playerInput.jumpEvent += OnJump;
		playerInput.jumpCanceledEvent += OnJumpCanceled;
		playerInput.moveEvent += OnMove;
	}

	//Removes all listeners to events coming from PlayerInput script
	private void OnDisable()
	{
		playerInput.crouchEvent -= OnCrouch;
		playerInput.crouchCanceledEvent -= OnCrouchCanceled;
		playerInput.jumpEvent -= OnJump;
		playerInput.jumpCanceledEvent -= OnJumpCanceled;
		playerInput.moveEvent -= OnMove;
	}

	private void OnCrouch()
	{
		Debug.Log("Crouch!");
	}

	private void OnCrouchCanceled()
	{
		Debug.Log("Crouch Canceled!");
	}

	private void OnJump()
	{
		Debug.Log("Jump!");
	}

	private void OnJumpCanceled()
	{
		Debug.Log("Jump Canceled!");
	}

	private void OnMove(Vector2 movement)
	{
		Debug.Log("Move :" + movement);
	}
}
