using UnityEngine;

public abstract class StateMachine : MonoBehaviour
{
    private GameState _currentGameState;

	// Calls exit methods of the current state to avoid memory leaks
	private void OnDestroy()
	{
		_currentGameState?.Exit();
	}

	private void FixedUpdate()
	{
		_currentGameState?.FixedTick(Time.fixedDeltaTime);
	}

	private void Update()
	{
		_currentGameState?.Tick(Time.deltaTime);
	}

	public void SwitchCurrentState(GameState gameState)
	{
		_currentGameState?.Exit();

		_currentGameState = gameState;

		_currentGameState?.Enter();
	}
}
