using System.Collections.Generic;

namespace Pockline
{
    /// <summary>
    /// Scans the board grid and returns all MergeResults.
    /// Checks horizontal lines, vertical lines, and optionally clusters
    /// depending on BoardSettings flags.
    /// Pure C— no MonoBehaviour.
    /// </summary>
    public class MatchFinder
    {
        private readonly BoardSettings _settings;

        public MatchFinder(BoardSettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Finds all merges on the current grid.
        /// A cell can only participate in one merge result (highest-length line wins).
        /// </summary>
        public List<MergeResult> FindMatches(GemData[,] grid)
        {
            int cols = grid.GetLength(0);
            int rows = grid.GetLength(1);
            bool[,] consumed = new bool[cols, rows];
            var results = new List<MergeResult>();

            // ─ Horizontal lines ─────────────────────────────────────────────
            if (_settings.matchHorizontal)
            {
                for (int r = 0; r < rows; r++)
                {
                    int c = 0;
                    while (c < cols)
                    {
                        if (grid[c, r] == null) { c++; continue; }
                        int tier   = grid[c, r].tierIndex;
                        int length = 1;
                        while (c + length < cols && grid[c + length, r]?.tierIndex == tier)
                            length++;

                        if (length >= _settings.minimumLineMatch)
                        {
                            var line = new List<GemData>();
                            for (int i = 0; i < length; i++)
                            {
                                if (!consumed[c + i, r])
                                {
                                    consumed[c + i, r] = true;
                                    line.Add(grid[c + i, r]);
                                }
                            }
                            if (line.Count >= _settings.minimumLineMatch)
                                results.Add(BuildResult(line, tier, true, false, line.Count, grid));
                        }
                        c += length;
                    }
                }
            }

            // ─ Vertical lines ──────────────────────────────────────────────
            if (_settings.matchVertical)
            {
                for (int c = 0; c < cols; c++)
                {
                    int r = 0;
                    while (r < rows)
                    {
                        if (grid[c, r] == null) { r++; continue; }
                        int tier   = grid[c, r].tierIndex;
                        int length = 1;
                        while (r + length < rows && grid[c, r + length]?.tierIndex == tier)
                            length++;

                        if (length >= _settings.minimumLineMatch)
                        {
                            var line = new List<GemData>();
                            for (int i = 0; i < length; i++)
                            {
                                if (!consumed[c, r + i])
                                {
                                    consumed[c, r + i] = true;
                                    line.Add(grid[c, r + i]);
                                }
                            }
                            if (line.Count >= _settings.minimumLineMatch)
                                results.Add(BuildResult(line, tier, false, false, line.Count, grid));
                        }
                        r += length;
                    }
                }
            }

            // ─ Clusters ─────────────────────────────────────────────────────
            if (_settings.matchClusters)
            {
                bool[,] visited = new bool[cols, rows];
                for (int c = 0; c < cols; c++)
                {
                    for (int r = 0; r < rows; r++)
                    {
                        if (visited[c, r] || consumed[c, r] || grid[c, r] == null) continue;
                        int tier    = grid[c, r].tierIndex;
                        var cluster = new List<GemData>();
                        FloodFill(grid, visited, c, r, tier, cols, rows, cluster);

                        if (cluster.Count >= _settings.minimumClusterMatch)
                        {
                            foreach (var g in cluster) consumed[g.col, g.row] = true;
                            results.Add(BuildResult(cluster, tier, false, true, 0, grid));
                        }
                    }
                }
            }

            return results;
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private MergeResult BuildResult(
            List<GemData> gems, int tier,
            bool isLine, bool isCluster, int lineLen,
            GemData[,] grid)
        {
            var tiers      = _settings.gemTiers;
            int nextTier   = tier + 1;
            GemData evolved = null;

            if (nextTier < tiers.Length)
            {
                // Place evolved gem at the position of the first gem in the list
                // (caller may override this with the swap position)
                var anchor = gems[0];
                evolved = new GemData(nextTier, tiers[nextTier], anchor.col, anchor.row);
            }
            // else top tier — evolved remains null (just clear and score)

            return new MergeResult(gems, evolved, tier, nextTier < tiers.Length ? nextTier : -1,
                                   isLine, isCluster, lineLen);
        }

        private void FloodFill(
            GemData[,] grid, bool[,] visited,
            int c, int r, int tier,
            int cols, int rows,
            List<GemData> result)
        {
            if (c < 0 || c >= cols || r < 0 || r >= rows) return;
            if (visited[c, r] || grid[c, r]?.tierIndex != tier) return;
            visited[c, r] = true;
            result.Add(grid[c, r]);
            FloodFill(grid, visited, c + 1, r, tier, cols, rows, result);
            FloodFill(grid, visited, c - 1, r, tier, cols, rows, result);
            FloodFill(grid, visited, c, r + 1, tier, cols, rows, result);
            FloodFill(grid, visited, c, r - 1, tier, cols, rows, result);
        }
    }
}
