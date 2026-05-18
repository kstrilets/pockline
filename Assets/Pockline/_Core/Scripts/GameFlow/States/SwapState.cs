namespace Pockline
{
    /// <summary>
    /// Spends one swap, tells BoardController to execute the swap,
    /// then waits for the view animation to finish before transitioning.
    /// Reverts and returns to Idle if no match is found after the swap.
    /// </summary>
    public class SwapState : IGameState
    {
        private readonly BoardController  _board;
        private readonly SwapManager      _swapManager;
        private readonly MatchFinder      _matchFinder;
        private readonly GameStateMachine _fsm;
        private readonly MatchState       _matchState;
        private readonly IdleState        _idleState;
        private readonly GameOverState    _gameOverState;

        private int _c1, _r1, _c2, _r2;
        private bool _waitingForAnim;
        private float _animTimer;
        private float _animDuration;

        public SwapState(
            BoardController board,
            SwapManager swapManager,
            MatchFinder matchFinder,
            GameStateMachine fsm,
            MatchState matchState,
            IdleState idleState,
            GameOverState gameOverState,
            float animDuration)
        {
            _board          = board;
            _swapManager    = swapManager;
            _matchFinder    = matchFinder;
            _fsm            = fsm;
            _matchState     = matchState;
            _idleState      = idleState;
            _gameOverState  = gameOverState;
            _animDuration   = animDuration;
        }

        /// <summary>Called by GameManager before entering this state.</summary>
        public void SetSwap(int c1, int r1, int c2, int r2)
        {
            _c1 = c1; _r1 = r1; _c2 = c2; _r2 = r2;
        }

        public void Enter()
        {
            // Deduct swap cost first
            if (!_swapManager.TrySpendSwap())
            {
                // Ran out of swaps before the swap could happen
                _fsm.ChangeState(_gameOverState);
                return;
            }

            _board.TrySwap(_c1, _r1, _c2, _r2);
            _waitingForAnim = true;
            _animTimer = 0f;
        }

        public void Tick(float dt)
        {
            if (!_waitingForAnim) return;
            _animTimer += dt;
            if (_animTimer < _animDuration) return;

            _waitingForAnim = false;

            // Check for matches
            var matches = _matchFinder.FindMatches(_board.Grid);
            if (matches.Count > 0)
            {
                _matchState.SetMatches(matches);
                _fsm.ChangeState(_matchState);
            }
            else
            {
                _board.RevertSwap(_c1, _r1, _c2, _r2);
                // Refund the swap cost on invalid match
                _swapManager.AddSwaps(_swapManager.CurrentSwaps == 0 ? 1 : 0);
                _fsm.ChangeState(_idleState);
            }
        }

        public void Exit() { }
    }
}
