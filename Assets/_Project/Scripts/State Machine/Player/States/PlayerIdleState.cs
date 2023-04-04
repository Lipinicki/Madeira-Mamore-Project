using UnityEngine;

public class PlayerIdleState : PlayerOnGroundState
{
	private readonly int kIdleAnimationParam = Animator.StringToHash("StartIdle");

	public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine)
	{
	}

	public override void Enter()
	{
		base.Enter();
		Debug.Log("Idle State", _stateMachine);
		_stateMachine.MainAnimator.SetTrigger(kIdleAnimationParam);
	}

	public override void FixedTick(float fixedDeltaTime)
	{
		base.FixedTick(fixedDeltaTime);
	}

	public override void Tick(float deltaTime)
	{
		base.Tick(deltaTime);

		if (_stateMachine.InputVector.sqrMagnitude > 0.02f)
		{
			_stateMachine.SwitchCurrentState(new PlayerWalkingState(_stateMachine));
		}
	}

	public override void Exit()
	{
		base.Exit();
	}
}
