namespace Pockline
{
    /// <summary>
    /// Checks board conditions without modifying any state.
    /// Pure C# — no MonoBehaviour.
    /// </summary>
    public class BoardValidator
    {
        private readonly BoardSettings _settings;

        public BoardValidator(BoardSettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Returns true if at least one swap on the board would produce a match.
        /// Used to detect game-over condition.
        /// </summary>
        public bool HasValidMove(GemData[,] grid)
        {
            int cols = grid.GetLength(0);
            int rows = grid.GetLength(1);

            // Try every horizontal swap
            for (int c = 0; c < cols - 1; c++)
                for (int r = 0; r < rows; r++)
                    if (SwapCreatesMatch(grid, c, r, c + 1, r)) return true;

            // Try every vertical swap
            for (int c = 0; c < cols; c++)
                for (int r = 0; r < rows - 1; r++)
                    if (SwapCreatesMatch(grid, c, r, c, r + 1)) return true;

            return false;
        }

        // ── Private ──────────────────────────────────────────────────────────

        private bool SwapCreatesMatch(GemData[,] grid, int c1, int r1, int c2, int r2)
        {
            // Temporarily swap
            SwapCells(grid, c1, r1, c2, r2);
            bool hasMatch = HasMatchAt(grid, c1, r1) || HasMatchAt(grid, c2, r2);
            // Undo
            SwapCells(grid, c1, r1, c2, r2);
            return hasMatch;
        }

        private bool HasMatchAt(GemData[,] grid, int col, int row)
        {
            if (grid[col, row] == null) return false;
            int tier = grid[col, row].tierIndex;
            int min  = _settings.minimumLineMatch;

            if (_settings.matchHorizontal && CountLine(grid, col, row, 1, 0, tier) >= min) return true;
            if (_settings.matchVertical   && CountLine(grid, col, row, 0, 1, tier) >= min) return true;
            if (_settings.matchClusters   && CountCluster(grid, col, row, tier) >= _settings.minimumClusterMatch) return true;

            return false;
        }

        private int CountLine(GemData[,] grid, int col, int row, int dc, int dr, int tier)
        {
            int cols  = grid.GetLength(0);
            int rows  = grid.GetLength(1);
            int count = 0;

            // Walk backwards to find start of the run
            int sc = col, sr = row;
            while (sc - dc >= 0 && sc - dc < cols && sr - dr >= 0 && sr - dr < rows
                   && grid[sc - dc, sr - dr]?.tierIndex == tier)
            { sc -= dc; sr -= dr; }

            // Count forward
            int c = sc, r = sr;
            while (c >= 0 && c < cols && r >= 0 && r < rows
                   && grid[c, r]?.tierIndex == tier)
            { count++; c += dc; r += dr; }

            return count;
        }

        private int CountCluster(GemData[,] grid, int col, int row, int tier)
        {
            int cols    = grid.GetLength(0);
            int rows    = grid.GetLength(1);
            bool[,] vis = new bool[cols, rows];
            return FloodCount(grid, vis, col, row, tier, cols, rows);
        }

        private int FloodCount(GemData[,] grid, bool[,] vis, int c, int r, int tier, int cols, int rows)
        {
            if (c < 0 || c >= cols || r < 0 || r >= rows) return 0;
            if (vis[c, r] || grid[c, r]?.tierIndex != tier) return 0;
            vis[c, r] = true;
            return 1
                + FloodCount(grid, vis, c + 1, r, tier, cols, rows)
                + FloodCount(grid, vis, c - 1, r, tier, cols, rows)
                + FloodCount(grid, vis, c, r + 1, tier, cols, rows)
                + FloodCount(grid, vis, c, r - 1, tier, cols, rows);
        }

        private static void SwapCells(GemData[,] grid, int c1, int r1, int c2, int r2)
        {
            var tmp      = grid[c1, r1];
            grid[c1, r1] = grid[c2, r2];
            grid[c2, r2] = tmp;
            // Update position references
            if (grid[c1, r1] != null) { grid[c1, r1].col = c1; grid[c1, r1].row = r1; }
            if (grid[c2, r2] != null) { grid[c2, r2].col = c2; grid[c2, r2].row = r2; }
        }
    }
}
