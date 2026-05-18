namespace Pockline
{
    /// <summary>
    /// Contract every game state must implement.
    /// </summary>
    public interface IGameState
    {
        void Enter();
        void Tick(float deltaTime);
        void Exit();
    }
}
