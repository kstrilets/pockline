using System.Collections.Generic;
using UnityEngine;

namespace Pockline
{
    /// <summary>
    /// Data produced by MatchFinder for one resolved merge.
    /// Consumed by BoardController, ScoreManager, SwapManager, AudioManager, VFXManager.
    /// </summary>
    public class MergeResult
    {
        /// <summary>All gems that were consumed (removed) by this merge.</summary>
        public List<GemData> consumedGems;

        /// <summary>
        /// The new evolved gem placed at the merge position.
        /// Null when the merge happens at the highest tier (top-tier clear).
        /// </summary>
        public GemData evolvedGem;

        /// <summary>World-space centre of the consumed gems cluster (for VFX spawn).</summary>
        public Vector2 centreGridPos;

        /// <summary>How many gems were consumed.</summary>
        public int Count => consumedGems?.Count ?? 0;

        /// <summary>Tier index that was consumed.</summary>
        public int consumedTierIndex;

        /// <summary>Tier index of the evolved gem (-1 if top tier).</summary>
        public int evolvedTierIndex;

        /// <summary>True if this merge was triggered by a line match.</summary>
        public bool isLineMatch;

        /// <summary>True if this merge was triggered by a cluster match.</summary>
        public bool isClusterMatch;

        /// <summary>Length of the line if isLineMatch is true.</summary>
        public int lineLength;

        public MergeResult(
            List<GemData> consumed,
            GemData evolved,
            int consumedTier,
            int evolvedTier,
            bool lineMatch,
            bool clusterMatch,
            int lineLengthVal = 0)
        {
            consumedGems       = consumed;
            evolvedGem         = evolved;
            consumedTierIndex  = consumedTier;
            evolvedTierIndex   = evolvedTier;
            isLineMatch        = lineMatch;
            isClusterMatch     = clusterMatch;
            lineLength         = lineLengthVal;

            // Compute grid-space centre
            if (consumed != null && consumed.Count > 0)
            {
                float cx = 0f, cy = 0f;
                foreach (var g in consumed) { cx += g.col; cy += g.row; }
                centreGridPos = new Vector2(cx / consumed.Count, cy / consumed.Count);
            }
        }
    }
}
