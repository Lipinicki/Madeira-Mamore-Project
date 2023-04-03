using UnityEngine;

public class PlayerStateMachine : StateMachine
{
	[field: SerializeField]
	public PlayerInput PlayerInput { get; private set; }

	private void Start()
	{
		SwitchCurrentState(new PlayerIdleState(this));
	}
}
