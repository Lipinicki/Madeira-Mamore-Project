using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "PlayerInput", menuName = "Player Input")]
public class PlayerInput : ScriptableObject, GameControls.IPlayerActions, GameControls.IMenusActions, GameControls.IDebugActions
{
	// ++++ Player ++++

	public Vector3 RawMovementInput { get; private set; }

	public UnityAction crouchEvent;
	public UnityAction crouchCanceledEvent;
	public UnityAction jumpEvent;
	public UnityAction jumpCanceledEvent;
	public UnityAction interactEvent;
	public UnityAction pauseEvent;
	public UnityAction runEvent;
	public UnityAction runCanceledEvent;
	public UnityAction<Vector2> moveEvent;
	public UnityAction<Vector2> cameraLookEvent;

	// ++++ Menus ++++

	public UnityAction menusPauseEvent;
	public UnityAction menusCancelEvent;
	public UnityAction menusSubmitEvent;
	public UnityAction<Vector2> menusNavigateEvent;

	// ++++ Debug ++++

	public UnityAction debugActivePlayerInputEvent;
	public UnityAction debugActiveMenusInputEvent;
	public UnityAction debugNextLevelEvent;

	private GameControls gameControls;

	private void OnEnable()
	{
		if (gameControls == null)
		{
			gameControls = new GameControls();

			gameControls.Player.SetCallbacks(this);
			gameControls.Menus.SetCallbacks(this);
			gameControls.Debug.SetCallbacks(this);
		}
#if UNITY_EDITOR
		gameControls.Debug.Enable();
#endif
	}

	private void OnDisable()
	{
		DisableAllInput();
	}

	public void EnablePlayerInput()
	{
		gameControls.Menus.Disable();

		gameControls.Player.Enable();
	}

	public void EnableMenusInput()
	{
		gameControls.Player.Disable();

		gameControls.Menus.Enable();
	}

	public void DisableAllInput()
	{
		gameControls.Player.Disable();
		gameControls.Menus.Disable();
	}

	/*
	 * Player
	 */

	public void OnCrouch(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Started)
			crouchEvent?.Invoke();

		if (context.phase == InputActionPhase.Canceled)
			crouchCanceledEvent?.Invoke();
	}

	public void OnInteract(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Started)
			interactEvent?.Invoke();
	}

	public void OnJump(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Started)
			jumpEvent?.Invoke();

		if (context.phase == InputActionPhase.Canceled)
			jumpCanceledEvent?.Invoke();
	}

	public void OnLook(InputAction.CallbackContext context)
	{
		cameraLookEvent?.Invoke(context.ReadValue<Vector2>());
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		Vector2 input = context.ReadValue<Vector2>();

		moveEvent?.Invoke(input);

		RawMovementInput = new Vector3(input.x, 0, input.y);
	}

	public void OnPause(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Started)
			pauseEvent?.Invoke();
	}

	public void OnRun(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Performed)
			runEvent?.Invoke();

		if (context.phase == InputActionPhase.Canceled)
			runCanceledEvent?.Invoke();	
	}

	/*
	 * Menus
	 */

	public void OnUnPause(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Started)
			menusPauseEvent?.Invoke();
	}

	public void OnSubmit(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Started)
			menusSubmitEvent?.Invoke();
	}

	public void OnCancel(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Started)
			menusCancelEvent?.Invoke();
	}

	public void OnNavigate(InputAction.CallbackContext context)
	{
		Vector2 input = context.ReadValue<Vector2>();

		menusNavigateEvent?.Invoke(input);
	}

	public void OnPoint(InputAction.CallbackContext context)
	{
		
	}

	public void OnClick(InputAction.CallbackContext context)
	{
		
	}

	public void OnScrollWheel(InputAction.CallbackContext context)
	{
		
	}

	public void OnMiddleClick(InputAction.CallbackContext context)
	{
		
	}

	public void OnRightClick(InputAction.CallbackContext context)
	{
		
	}

	public void OnTrackedDevicePosition(InputAction.CallbackContext context)
	{
		
	}

	public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
	{
		
	}

	/*
	 * Debug
	 */

	public void OnActivatePlayerInput(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Started)
			debugActivePlayerInputEvent?.Invoke();
	}

	public void OnActivateMenusInput(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Started)
			debugActiveMenusInputEvent?.Invoke();
	}

	public void OnNextLevel(InputAction.CallbackContext context)
	{
		if (context.phase == InputActionPhase.Started)
			debugNextLevelEvent?.Invoke();
	}
}
