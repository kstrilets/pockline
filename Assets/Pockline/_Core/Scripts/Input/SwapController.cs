using UnityEngine;

namespace Pockline
{
    /// <summary>
    /// Sits between IInputHandler and BoardController.
    /// Validates adjacency, checks swap manager has swaps available,
    /// then tells the GameStateMachine to enter SwapState.
    /// </summary>
    public class SwapController
    {
        private readonly BoardController  _board;
        private readonly SwapManager      _swapManager;
        private readonly GameStateMachine _fsm;
        private readonly SwapState        _swapState;
        private          IInputHandler    _input;
        private          bool             _inputEnabled;

        public SwapController(
            BoardController board,
            SwapManager swapManager,
            GameStateMachine fsm,
            SwapState swapState)
        {
            _board       = board;
            _swapManager = swapManager;
            _fsm         = fsm;
            _swapState   = swapState;
        }

        /// <summary>Registers the input source. Call once during GameManager.Start.</summary>
        public void RegisterInput(IInputHandler input)
        {
            if (_input != null)
                _input.OnSwapRequested -= HandleSwapRequested;

            _input = input;
            _input.OnSwapRequested += HandleSwapRequested;
        }

        /// <summary>Called by IdleState.Enter / Exit to gate input.</summary>
        public void SetInputEnabled(bool enabled)
        {
            _inputEnabled = enabled;
            _input?.SetEnabled(enabled);
        }

        // ── Private ──────────────────────────────────────────────────────────

        private void HandleSwapRequested(Vector2Int from, Vector2Int to)
        {
            if (!_inputEnabled) return;
            if (_swapManager.CurrentSwaps <= 0) return;
            if (!IsAdjacent(from, to)) return;
            if (!InBounds(from) || !InBounds(to)) return;

            _swapState.SetSwap(from.x, from.y, to.x, to.y);
            _fsm.ChangeState(_swapState);
        }

        private bool IsAdjacent(Vector2Int a, Vector2Int b) =>
            (Mathf.Abs(a.x - b.x) == 1 && a.y == b.y) ||
            (Mathf.Abs(a.y - b.y) == 1 && a.x == b.x);

        private bool InBounds(Vector2Int cell) =>
            cell.x >= 0 && cell.x < _board.Columns &&
            cell.y >= 0 && cell.y < _board.Rows;
    }
}
