using System.Collections.Generic;
using UnityEngine;

namespace Pockline
{
    /// <summary>
    /// Owns the GemView[,] grid. Subscribes to BoardController events
    /// and drives GemView animations. No game logic lives here.
    /// </summary>
    public class BoardView : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GemViewPool _pool;

        [Header("Layout")]
        [Tooltip("World-space position of the centre of cell (0,0).")]
        [SerializeField] private Vector2 _originOffset = Vector2.zero;
        [SerializeField] private float   _cellSize = 1f;

        private GemView[,]      _views;
        private BoardController _board;
        private BoardSettings   _settings;

        // ── Initialisation ─────────────────────────────────────────────────

        /// <summary>Called by GameManager after the board is constructed.</summary>
        public void Initialise(BoardController board, BoardSettings settings)
        {
            _board    = board;
            _settings = settings;
            _views    = new GemView[settings.columns, settings.rows];

            _board.OnSwapExecuted  += HandleSwapExecuted;
            _board.OnSwapReverted  += HandleSwapReverted;
            _board.OnBoardRefilled += HandleBoardRefilled;
        }

        private void OnDestroy()
        {
            if (_board == null) return;
            _board.OnSwapExecuted  -= HandleSwapExecuted;
            _board.OnSwapReverted  -= HandleSwapReverted;
            _board.OnBoardRefilled -= HandleBoardRefilled;
        }

        // ── Event handlers ─────────────────────────────────────────────────

        private void HandleSwapExecuted(int c1, int r1, int c2, int r2)
        {
            var v1 = _views[c1, r1];
            var v2 = _views[c2, r2];
            if (v1 == null || v2 == null) return;

            float dur = _settings.swapAnimDuration;
            v1.PlaySwap(GridToWorld(c2, r2), dur);
            v2.PlaySwap(GridToWorld(c1, r1), dur);

            // Swap references in the view grid
            _views[c1, r1] = v2;
            _views[c2, r2] = v1;
        }

        private void HandleSwapReverted(int c1, int r1, int c2, int r2)
        {
            // Same visual as a forward swap
            HandleSwapExecuted(c1, r1, c2, r2);
        }

        private void HandleBoardRefilled()
        {
            SyncViewsToGrid();
        }

        // ── Grid sync ──────────────────────────────────────────────────────────

        /// <summary>
        /// Walks the board grid. Returns pooled views for removed gems,
        /// rents new views for new gems, and animates falls.
        /// </summary>
        private void SyncViewsToGrid()
        {
            int cols = _board.Columns;
            int rows = _board.Rows;

            // Collect views whose data no longer matches the grid (merged/removed)
            var toReturn = new List<GemView>();
            for (int c = 0; c < cols; c++)
                for (int r = 0; r < rows; r++)
                {
                    var view = _views[c, r];
                    var data = _board.Grid[c, r];
                    if (view != null && (data == null || view.Data != data))
                    {
                        toReturn.Add(view);
                        _views[c, r] = null;
                    }
                }

            foreach (var v in toReturn)
                _pool.Return(v);

            // Spawn / fall views for data that has no view yet
            float fallDurPerRow = _settings.fallAnimDurationPerRow;
            for (int c = 0; c < cols; c++)
                for (int r = 0; r < rows; r++)
                {
                    var data = _board.Grid[c, r];
                    if (data == null) continue;
                    if (_views[c, r] != null) continue;

                    var view = _pool.Rent();
                    view.Bind(data);
                    view.transform.SetParent(transform);
                    view.SnapTo(GridToWorld(c, rows)); // start above board
                    view.PlayFall(GridToWorld(c, r), fallDurPerRow * (rows - r));
                    _views[c, r] = view;
                }
        }

        // ── Layout ─────────────────────────────────────────────────────────────

        public Vector3 GridToWorld(int col, int row) =>
            new Vector3(
                _originOffset.x + col * _cellSize,
                _originOffset.y + row * _cellSize,
                0f);
    }
}
