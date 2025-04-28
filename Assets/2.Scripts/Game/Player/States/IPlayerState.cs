public interface IPlayerState
{
    void Enter(PlayerStateMachine player);
    void Execute(PlayerStateMachine player);
    void Exit(PlayerStateMachine player);
}