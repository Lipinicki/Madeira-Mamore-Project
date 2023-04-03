using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    private GameState _currentGameState;

	private void FixedUpdate()
	{
		_currentGameState.FixedTick(Time.fixedDeltaTime);
	}

	private void Update()
	{
		_currentGameState.Tick(Time.deltaTime);
	}

	public void SwitchCurrentState(GameState gameState)
	{
		_currentGameState.Exit();

		_currentGameState = gameState;

		_currentGameState.Enter();
	}
}
