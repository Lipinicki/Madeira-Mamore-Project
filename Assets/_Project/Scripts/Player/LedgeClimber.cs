using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LedgeClimber : MonoBehaviour
{
	public PlayerInput playerInput;

	[SerializeField, Tooltip("Speed in wich the player grabs a ledge")] float ledgeGrabSpeed = 2f;
	[SerializeField] Transform grabDetectionOrigin;

	bool _isLerping = false;
	float _lerpStartTime;
	Vector3 _standPosition = Vector3.zero;
	Vector3 _lerpDestination = Vector3.zero;
	RaycastHit _grabHit;
	Rigidbody _rigidbody;
	Animator _mainAnimator;
	PlayerMovement _playerMovement;

	private const string kLedgeLayer = "Ledges";
	private const string kClimbAnimationParam = "StartClimb";
	private const string kGrabAnimationParam = "isGrabingLedge";

	private void OnEnable()
	{
		playerInput.moveEvent += OnClimb;
	}

	private void OnDisable()
	{
		playerInput.moveEvent -= OnClimb;
	}

	private void Awake()
	{
		_playerMovement = GetComponent<PlayerMovement>();
		_rigidbody = GetComponent<Rigidbody>();
		_mainAnimator = GetComponentInChildren<Animator>();
	}

    void Update()
    {
		//Set Animations
		_mainAnimator.SetBool(kGrabAnimationParam, _playerMovement.CurrentPlayerState == ActionStates.Holding);

		//Checks if can grab a ledge and handle transform interpolation
		HandleLedgeGrab();
		HandleGrabLerp();
	}

	private void HandleLedgeGrab()
	{
		// Setup holding state
		if (_playerMovement.IsGrounded()
			|| _playerMovement.CurrentPlayerState == ActionStates.Holding) return;

		if (Physics.Raycast(grabDetectionOrigin.position, grabDetectionOrigin.forward, out _grabHit, 5f, LayerMask.GetMask(kLedgeLayer)))
		{
			_rigidbody.isKinematic = true;
			_playerMovement.ChangePlayerState(ActionStates.Holding);

			float heightOffset = Vector3.Distance(transform.position, grabDetectionOrigin.position) - 0.15f;
			_lerpDestination = new Vector3(_grabHit.point.x, _grabHit.point.y - heightOffset, _grabHit.point.z);
			_standPosition = _grabHit.point + new Vector3(transform.forward.x, transform.forward.y + heightOffset, transform.forward.z);
			_lerpStartTime = Time.time;

			_isLerping = true;
		}
	}

	private void HandleGrabLerp()
	{
		// Smoothly moves the player towards a ledge
		if (!_isLerping) return;

		float timeSinceStarted = Time.time - _lerpStartTime;
		float percentageComplete = timeSinceStarted / ledgeGrabSpeed;

		transform.position = Vector3.Lerp(transform.position, _lerpDestination, percentageComplete);

		if (percentageComplete >= 1.0f)
		{
			_isLerping = false;
		}

	}

	private void ReleaseLedge()
	{
		//Reset holding state
		_playerMovement.ResetPlayerState();
		_rigidbody.isKinematic = false;
	}

	private async void ClimbFromLedge()
	{
		//Transitions the player from the ledge to the climbing state
		_mainAnimator.SetTrigger(kClimbAnimationParam);
		await Task.Delay(100);

		float currentClipLenght = _mainAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
		int delayInMiliseconds = (int)(currentClipLenght * 1000);
		await Task.Delay(delayInMiliseconds);

		transform.position = _standPosition;
		_mainAnimator.SetTrigger(_playerMovement.KIdleAnimationParam);
		_rigidbody.isKinematic = false;
		_playerMovement.ResetPlayerState();
	}

	private void OnClimb(Vector2 movement)
	{
		if (movement.y > 0 && _playerMovement.CurrentPlayerState == ActionStates.Holding) ClimbFromLedge();
		else if (movement.y < 0 && _playerMovement.CurrentPlayerState == ActionStates.Holding) ReleaseLedge();
	}
}
