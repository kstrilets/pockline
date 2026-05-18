using UnityEngine;

namespace Pockline
{
    /// <summary>
    /// Spawns VFX prefabs at world positions in response to game events.
    /// All prefabs are expected to self-destruct via a ParticleSystem
    /// Stop Action or a timed Destroy call on the prefab itself.
    /// </summary>
    public class VFXManager : MonoBehaviour
    {
        private VFXSettings     _settings;
        private BoardView       _boardView;

        // ── Initialisation ────────────────────────────────────────────────

        public void Initialise(
            VFXSettings settings,
            BoardView boardView,
            BoardController board,
            SwapManager swapManager,
            GameOverState gameOverState)
        {
            _settings  = settings;
            _boardView = boardView;

            board.OnMatchesFound += results =>
            {
                foreach (var r in results)
                    SpawnAt(settings.mergeVFXPrefab, r.centreGridPos);
            };

            board.OnSwapReverted += (c1, r1, c2, r2) =>
                SpawnAt(settings.invalidSwapVFXPrefab,
                    new Vector2((c1 + c2) * 0.5f, (r1 + r2) * 0.5f));

            board.OnBoardRefilled += () =>
            {
                // Spawn a subtle gem-spawn VFX at every refilled cell
                // (BoardView already tracks which ones are new — we just
                //  do a light board-wide pulse here for polish)
                SpawnAt(settings.gemSpawnVFXPrefab,
                    new Vector2(board.Columns * 0.5f, board.Rows * 0.5f));
            };

            swapManager.OnBonusSwapsAwarded += (amount, rule) =>
            {
                if (rule?.bonusVFXPrefab != null)
                    SpawnAt(rule.bonusVFXPrefab,
                        new Vector2(board.Columns * 0.5f, board.Rows * 0.5f));
            };

            gameOverState.OnGameOver += () =>
                SpawnAt(settings.gameOverVFXPrefab,
                    new Vector2(board.Columns * 0.5f, board.Rows * 0.5f));
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private void SpawnAt(GameObject prefab, Vector2 gridPos)
        {
            if (prefab == null || _boardView == null) return;
            var world = _boardView.GridToWorld((int)gridPos.x, (int)gridPos.y);
            Instantiate(prefab, world, Quaternion.identity);
        }
    }
}
