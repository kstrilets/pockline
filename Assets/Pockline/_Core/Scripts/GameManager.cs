using UnityEngine;

namespace Pockline
{
    /// <summary>
    /// Single MonoBehaviour entry point. Constructs and wires every system.
    /// Attach to one GameObject in the scene. Assign gameSettings in the Inspector.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private GameSettings _gameSettings;

        [Header("Scene References")]
        [SerializeField] private BoardView         _boardView;
        [SerializeField] private TouchInputHandler _inputHandler;
        [SerializeField] private AudioManager      _audioManager;
        [SerializeField] private VFXManager        _vfxManager;
        [SerializeField] private SwapCounterUI     _swapCounterUI;

        // ── Systems ──────────────────────────────────────────────────────────

        private BoardController  _board;
        private SwapManager      _swapManager;
        private ScoreManager     _scoreManager;
        private MatchFinder      _matchFinder;
        private GameStateMachine _fsm;

        // States
        private IdleState     _idleState;
        private SwapState     _swapState;
        private MatchState    _matchState;
        private FallState     _fallState;
        private GameOverState _gameOverState;

        // ── Unity ───────────────────────────────────────────────────────────────

        private void Start()
        {
            BuildSystems();
            StartGame();
        }

        private void Update() => _fsm?.Tick(Time.deltaTime);

        // ── Wiring ────────────────────────────────────────────────────────────

        private void BuildSystems()
        {
            var bs = _gameSettings.boardSettings;
            var ss = _gameSettings.swapSettings;
            var sc = _gameSettings.scoreSettings;

            // — Pure C# systems —
            _board        = new BoardController(bs);
            _swapManager  = new SwapManager(ss);
            _scoreManager = new ScoreManager(sc);
            _matchFinder  = new MatchFinder(bs);
            _fsm          = new GameStateMachine();

            // — States (create forward refs first, inject below) —
            _gameOverState = new GameOverState();
            _idleState     = new IdleState(null);   // SwapController injected after
            _matchState    = new MatchState(
                _board, _scoreManager, _swapManager,
                _fsm, null, _gameOverState,         // FallState injected below
                bs.matchPauseDuration);
            _fallState = new FallState(
                _board, _matchFinder, _scoreManager,
                _fsm, _matchState, null,            // IdleState injected below
                _gameOverState,
                bs.fallAnimDurationPerRow * bs.rows);
            _swapState = new SwapState(
                _board, _swapManager, _matchFinder,
                _fsm, _matchState, null,            // IdleState injected below
                _gameOverState,
                bs.swapAnimDuration);

            // Resolve circular refs via reflection-free property setters
            // (States expose internal setters for this purpose)
            _matchState.SetFallState(_fallState);
            _fallState.SetIdleState(_idleState);
            _swapState.SetIdleState(_idleState);

            // — Input —
            var swapCtrl = new SwapController(_board, _swapManager, _fsm, _swapState);
            swapCtrl.RegisterInput(_inputHandler);
            _idleState = new IdleState(swapCtrl);   // rebuild with real SwapController
            _swapState.SetIdleState(_idleState);
            _fallState.SetIdleState(_idleState);

            // — View —
            _boardView.Initialise(_board, bs);

            // — Audio —
            _audioManager.Initialise(
                _gameSettings.audioSettings,
                _board, _swapManager, _gameOverState);

            // — VFX —
            _vfxManager.Initialise(
                _gameSettings.vfxSettings,
                _boardView, _board, _swapManager, _gameOverState);

            // — UI —
            _swapCounterUI?.Initialise(_swapManager);

            // — Game over hook —
            _gameOverState.OnGameOver += HandleGameOver;
        }

        private void StartGame()
        {
            _swapManager.Reset();
            _scoreManager.Reset();
            _board.InitialFill();
            _audioManager.PlayMusic();
            _fsm.ChangeState(_idleState);
        }

        // ── Handlers ───────────────────────────────────────────────────────────

        private void HandleGameOver()
        {
            Debug.Log($"[Pockline] Game Over. Final score: {_scoreManager.TotalScore}");
            // Show game-over UI here (Step 11 / future UI pass)
        }
    }
}
