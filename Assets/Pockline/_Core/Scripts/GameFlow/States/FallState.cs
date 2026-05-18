namespace Pockline
{
    /// <summary>
    /// Waits for the fall/fill animation to complete, then checks
    /// for chain reactions. If new matches exist, loops back to MatchState.
    /// Otherwise resets chain and returns to Idle (or GameOver).
    /// </summary>
    public class FallState : IGameState
    {
        private readonly BoardController  _board;
        private readonly MatchFinder      _matchFinder;
        private readonly ScoreManager     _score;
        private readonly GameStateMachine _fsm;
        private readonly MatchState       _matchState;
        private readonly IdleState        _idleState;
        private readonly GameOverState    _gameOverState;
        private readonly float            _fallAnimDuration;

        private float _timer;

        public FallState(
            BoardController board,
            MatchFinder matchFinder,
            ScoreManager score,
            GameStateMachine fsm,
            MatchState matchState,
            IdleState idleState,
            GameOverState gameOverState,
            float fallAnimDuration)
        {
            _board            = board;
            _matchFinder      = matchFinder;
            _score            = score;
            _fsm              = fsm;
            _matchState       = matchState;
            _idleState        = idleState;
            _gameOverState    = gameOverState;
            _fallAnimDuration = fallAnimDuration;
        }

        public void Enter()  => _timer = 0f;

        public void Tick(float dt)
        {
            _timer += dt;
            if (_timer < _fallAnimDuration) return;

            // Check for chain reaction
            var chain = _matchFinder.FindMatches(_board.Grid);
            if (chain.Count > 0)
            {
                _matchState.SetMatches(chain);
                _fsm.ChangeState(_matchState);
            }
            else
            {
                _score.ResetChain();
                _fsm.ChangeState(_idleState);
            }
        }

        public void Exit() { }
    }
}
