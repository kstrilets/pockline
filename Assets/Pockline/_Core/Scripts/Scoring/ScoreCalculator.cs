using System.Collections.Generic;
using UnityEngine;

namespace Pockline
{
    /// <summary>
    /// Pure static calculation logic. No state, fully testable.
    /// Takes a list of MergeResults plus the current chain depth and
    /// returns the total score for that batch.
    /// </summary>
    public static class ScoreCalculator
    {
        /// <summary>
        /// Calculates the total score for a set of merge results.
        /// chainStep: 0 = first match after a swap, 1 = first chain reaction, etc.
        /// </summary>
        public static int Calculate(
            List<MergeResult> results,
            int chainStep,
            ScoreSettings settings)
        {
            if (results == null || results.Count == 0) return 0;

            float multiplier = Mathf.Pow(settings.chainMultiplier, chainStep);
            multiplier = Mathf.Min(multiplier, settings.chainMultiplierCap);

            int total = 0;
            foreach (var r in results)
            {
                // Base score comes from the evolved tier definition
                // If top tier (evolvedTierIndex == -1) use consumed tier score
                int baseScore = r.evolvedTierIndex >= 0
                    ? r.evolvedGem?.tier?.mergeScore ?? 0
                    : r.consumedGems?[0]?.tier?.mergeScore ?? 0;

                total += Mathf.RoundToInt(baseScore * multiplier);
            }
            return total;
        }
    }
}
