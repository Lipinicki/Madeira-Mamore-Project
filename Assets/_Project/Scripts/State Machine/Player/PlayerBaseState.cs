

public abstract class PlayerBaseState : GameState
{
	protected PlayerStateMachine _stateMachine;

	protected PlayerBaseState(PlayerStateMachine stateMachine)
	{
		_stateMachine = stateMachine;
	}
}
