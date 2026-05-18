using System;

namespace Pockline
{
    /// <summary>
    /// Terminal state. Fires OnGameOver so GameManager / UI can react.
    /// </summary>
    public class GameOverState : IGameState
    {
        public event Action OnGameOver;

        public void Enter()  => OnGameOver?.Invoke();
        public void Tick(float dt) { }
        public void Exit()   { }
    }
}
