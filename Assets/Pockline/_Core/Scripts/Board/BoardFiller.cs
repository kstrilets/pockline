using UnityEngine;

namespace Pockline
{
    /// <summary>
    /// Responsible for creating new GemData instances when the board needs filling.
    /// Respects tierClusterBias from BoardSettings to make matches more or less likely.
    /// Pure C# — no MonoBehaviour.
    /// </summary>
    public class BoardFiller
    {
        private readonly BoardSettings _settings;

        public BoardFiller(BoardSettings settings)
        {
            _settings = settings;
        }

        /// <summary>
        /// Creates a single new gem for the given cell.
        /// Uses weighted random selection with optional neighbour-bias.
        /// </summary>
        public GemData CreateGem(int col, int row, GemData[,] existingGrid)
        {
            int tierIndex = SelectTierIndex(col, row, existingGrid);
            var tierDef   = _settings.gemTiers[tierIndex];
            return new GemData(tierIndex, tierDef, col, row);
        }

        private int SelectTierIndex(int col, int row, GemData[,] existingGrid)
        {
            var tiers = _settings.gemTiers;
            if (tiers == null || tiers.Length == 0) return 0;

            // Bias: if a neighbour exists and random roll passes, copy its tier
            if (_settings.tierClusterBias > 0f && Random.value < _settings.tierClusterBias)
            {
                int cols = existingGrid.GetLength(0);
                int rows = existingGrid.GetLength(1);

                // Check left, right, down neighbours
                int[] dc = { -1, 1, 0, 0 };
                int[] dr = { 0, 0, -1, 1 };
                for (int i = 0; i < 4; i++)
                {
                    int nc = col + dc[i];
                    int nr = row + dr[i];
                    if (nc >= 0 && nc < cols && nr >= 0 && nr < rows)
                    {
                        var neighbour = existingGrid[nc, nr];
                        if (neighbour != null)
                            return neighbour.tierIndex; // copy neighbour tier
                    }
                }
            }

            // Weighted random selection across all tiers
            float totalWeight = 0f;
            foreach (var t in tiers) totalWeight += Mathf.Max(0.001f, t.spawnWeight);

            float roll = Random.value * totalWeight;
            float accumulated = 0f;
            for (int i = 0; i < tiers.Length; i++)
            {
                accumulated += Mathf.Max(0.001f, tiers[i].spawnWeight);
                if (roll <= accumulated) return i;
            }
            return tiers.Length - 1;
        }
    }
}
