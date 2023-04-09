

public abstract class PlayerBaseState : GameState
{
	protected PlayerStateMachine _ctx;

	protected PlayerBaseState(PlayerStateMachine stateMachine)
	{
		_ctx = stateMachine;
	}
}
