using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pockline
{
    /// <summary>
    /// Owns the GemData[,] grid and all state transitions on it.
    /// Pure C# — no MonoBehaviour, no Transform, no Unity rendering.
    /// All communication outward is via C# events.
    /// </summary>
    public class BoardController
    {
        // ── Events ───────────────────────────────────────────────────────────

        /// <summary>Fired after two gems are swapped in the grid data.</summary>
        public event Action<int, int, int, int> OnSwapExecuted;

        /// <summary>Fired when a swap does not result in a match and is reverted.</summary>
        public event Action<int, int, int, int> OnSwapReverted;

        /// <summary>Fired with all match results found after a swap.</summary>
        public event Action<List<MergeResult>> OnMatchesFound;

        /// <summary>Fired after matched gems are removed and new gems have been placed.</summary>
        public event Action OnBoardRefilled;

        /// <summary>Fired when no valid swap exists anywhere on the board.</summary>
        public event Action OnNoMovesAvailable;

        // ── State ────────────────────────────────────────────────────────────

        public GemData[,] Grid { get; private set; }
        public int Columns { get; private set; }
        public int Rows    { get; private set; }

        private readonly BoardSettings   _settings;
        private readonly BoardFiller     _filler;
        private readonly BoardValidator  _validator;

        // ── Constructor ──────────────────────────────────────────────────────

        public BoardController(BoardSettings settings)
        {
            _settings  = settings;
            Columns    = settings.columns;
            Rows       = settings.rows;
            Grid       = new GemData[Columns, Rows];
            _filler    = new BoardFiller(settings);
            _validator = new BoardValidator(settings);
        }

        // ── Initialisation ───────────────────────────────────────────────────

        /// <summary>Fills every cell with a new randomly-selected gem.</summary>
        public void InitialFill()
        {
            for (int c = 0; c < Columns; c++)
                for (int r = 0; r < Rows; r++)
                    Grid[c, r] = _filler.CreateGem(c, r, Grid);

            OnBoardRefilled?.Invoke();
        }

        // ── Swap ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Attempts to swap the two cells. Returns false if the positions are
        /// not adjacent or are out of bounds. The caller is responsible for
        /// running MatchFinder and calling ApplyMatches / RevertSwap.
        /// </summary>
        public bool TrySwap(int col1, int row1, int col2, int row2)
        {
            if (!IsAdjacent(col1, row1, col2, row2)) return false;
            if (!InBounds(col1, row1) || !InBounds(col2, row2)) return false;

            ExecuteSwap(col1, row1, col2, row2);
            OnSwapExecuted?.Invoke(col1, row1, col2, row2);
            return true;
        }

        /// <summary>Reverts a previously executed swap (no match found).</summary>
        public void RevertSwap(int col1, int row1, int col2, int row2)
        {
            ExecuteSwap(col1, row1, col2, row2); // swap again = undo
            OnSwapReverted?.Invoke(col1, row1, col2, row2);
        }

        // ── Match application ────────────────────────────────────────────────

        /// <summary>
        /// Removes matched gems, places evolved gems, drops remaining gems
        /// downward, fills empty cells from the top, then fires OnBoardRefilled.
        /// </summary>
        public void ApplyMatches(List<MergeResult> results)
        {
            // 1. Mark all matched gems
            foreach (var result in results)
                foreach (var gem in result.consumedGems)
                    gem.isMatched = true;

            // 2. Clear matched cells and place evolved gem at merge position
            foreach (var result in results)
            {
                foreach (var gem in result.consumedGems)
                    Grid[gem.col, gem.row] = null;

                // Place the new evolved gem (may be null at top tier)
                if (result.evolvedGem != null)
                    Grid[result.evolvedGem.col, result.evolvedGem.row] = result.evolvedGem;
            }

            // 3. Apply gravity — slide non-null gems down
            ApplyGravity();

            // 4. Fill remaining nulls from the top
            FillEmpty();

            OnBoardRefilled?.Invoke();

            // 5. Check if any moves remain
            if (!_validator.HasValidMove(Grid))
                OnNoMovesAvailable?.Invoke();
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private void ExecuteSwap(int c1, int r1, int c2, int r2)
        {
            var tmp     = Grid[c1, r1];
            Grid[c1, r1] = Grid[c2, r2];
            Grid[c2, r2] = tmp;

            if (Grid[c1, r1] != null) { Grid[c1, r1].col = c1; Grid[c1, r1].row = r1; }
            if (Grid[c2, r2] != null) { Grid[c2, r2].col = c2; Grid[c2, r2].row = r2; }
        }

        private void ApplyGravity()
        {
            for (int c = 0; c < Columns; c++)
            {
                int writeRow = 0;
                for (int r = 0; r < Rows; r++)
                {
                    if (Grid[c, r] != null)
                    {
                        var gem = Grid[c, r];
                        Grid[c, r]        = null;
                        gem.row           = writeRow;
                        Grid[c, writeRow] = gem;
                        writeRow++;
                    }
                }
            }
        }

        private void FillEmpty()
        {
            for (int c = 0; c < Columns; c++)
                for (int r = 0; r < Rows; r++)
                    if (Grid[c, r] == null)
                        Grid[c, r] = _filler.CreateGem(c, r, Grid);
        }

        private bool InBounds(int c, int r) =>
            c >= 0 && c < Columns && r >= 0 && r < Rows;

        private bool IsAdjacent(int c1, int r1, int c2, int r2) =>
            (Mathf.Abs(c1 - c2) == 1 && r1 == r2) ||
            (Mathf.Abs(r1 - r2) == 1 && c1 == c2);
    }
}
