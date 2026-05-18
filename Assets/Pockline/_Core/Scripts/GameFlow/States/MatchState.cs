using System.Collections.Generic;

namespace Pockline
{
    /// <summary>
    /// Applies matches to the board, registers score, evaluates bonus swaps,
    /// then moves on to FallState.
    /// </summary>
    public class MatchState : IGameState
    {
        private readonly BoardController  _board;
        private readonly ScoreManager     _score;
        private readonly SwapManager      _swapManager;
        private readonly GameStateMachine _fsm;
        private readonly FallState        _fallState;
        private readonly GameOverState    _gameOverState;
        private readonly float            _pauseDuration;

        private List<MergeResult> _matches;
        private float _timer;
        private bool  _pausing;

        public MatchState(
            BoardController board,
            ScoreManager score,
            SwapManager swapManager,
            GameStateMachine fsm,
            FallState fallState,
            GameOverState gameOverState,
            float pauseDuration)
        {
            _board          = board;
            _score          = score;
            _swapManager    = swapManager;
            _fsm            = fsm;
            _fallState      = fallState;
            _gameOverState  = gameOverState;
            _pauseDuration  = pauseDuration;
        }

        public void SetMatches(List<MergeResult> matches) => _matches = matches;

        public void Enter()
        {
            _timer  = 0f;
            _pausing = true;
        }

        public void Tick(float dt)
        {
            if (!_pausing) return;
            _timer += dt;
            if (_timer < _pauseDuration) return;
            _pausing = false;

            // Register score (chain depth already tracked inside ScoreManager)
            _score.RegisterMatches(_matches);

            // Evaluate bonus swaps
            _swapManager.EvaluateBonuses(_matches);

            // Apply to board data (removes gems, places evolved gem, gravity)
            _board.ApplyMatches(_matches);

            _fsm.ChangeState(_fallState);
        }

        public void Exit() { }
    }
}
