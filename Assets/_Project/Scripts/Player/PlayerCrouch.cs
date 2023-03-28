using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouch : MonoBehaviour
{
  [SerializeField] private float crouchingHeight = 0.8f;
  [SerializeField] private float transitionSpeed = 5f;

  private float currentHeight;
  private float standingHeight;
  private bool crouchIsRunning = false;
  private PlayerMovement _playerMovement;
  private CapsuleCollider _playerCollider;
  private Coroutine crouchRoutine;

  private void OnEnable()
  {
    _playerMovement = GetComponent<PlayerMovement>();
    _playerCollider = GetComponent<CapsuleCollider>();

    standingHeight = currentHeight = _playerCollider.height;

    _playerMovement.PlayerInput.crouchEvent += Crouch;
  }

  private void OnDisable()
  {
    _playerMovement.PlayerInput.crouchEvent -= Crouch;
  }


  private IEnumerator CrouchCoroutine(float targetHeight)
  {
    var initalTarget = targetHeight;
    Debug.Log(initalTarget);
    crouchIsRunning = true;
    while (Mathf.Abs(currentHeight - targetHeight) > 0.01f)
    {
      var crouchDelta = Time.deltaTime * transitionSpeed;
      currentHeight = Mathf.Lerp(currentHeight, targetHeight, crouchDelta);
      _playerCollider.height = currentHeight;
      yield return null;
    }
    currentHeight = initalTarget;
    crouchIsRunning = false;
  }

  private void Crouch()
  {
    if (crouchIsRunning) return;

    if (_playerMovement.CurrentPlayerSubState != SubStates.Crouching)
    { 
      if (crouchRoutine != null) StopCoroutine(crouchRoutine);
      crouchRoutine = StartCoroutine(CrouchCoroutine(crouchingHeight));
      _playerMovement.ChangePlayerSubstate(SubStates.Crouching);
    }
    else if (CanStand())
    {
      if (crouchRoutine != null) StopCoroutine(crouchRoutine);
      crouchRoutine = StartCoroutine(CrouchCoroutine(standingHeight));
      _playerMovement.ResetPlayerState(true);
    }
  }
  private bool CanStand()
  {
    Vector3 raycastOrigin = transform.position + Vector3.up * (_playerCollider.height / 2);
    return !Physics.Raycast(raycastOrigin, Vector3.up, out var hit, standingHeight - crouchingHeight);
  }
}
