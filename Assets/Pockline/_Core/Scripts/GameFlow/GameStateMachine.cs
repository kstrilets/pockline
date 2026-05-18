using System;

namespace Pockline
{
    /// <summary>
    /// Minimal push-down automaton for the game loop.
    /// Transitions are driven by external callers (GameManager) reacting to events.
    /// Pure C# — no MonoBehaviour.
    /// </summary>
    public class GameStateMachine
    {
        public IGameState CurrentState { get; private set; }

        public event Action<IGameState, IGameState> OnStateChanged;

        public void ChangeState(IGameState next)
        {
            var prev = CurrentState;
            CurrentState?.Exit();
            CurrentState = next;
            CurrentState?.Enter();
            OnStateChanged?.Invoke(prev, next);
        }

        /// <summary>Forward Unity Update tick to the active state.</summary>
        public void Tick(float deltaTime) => CurrentState?.Tick(deltaTime);
    }
}
