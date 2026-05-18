using System;
using System.Collections.Generic;

namespace Pockline
{
    /// <summary>
    /// Tracks session score and chain depth.
    /// Calls ScoreCalculator for the actual math.
    /// Pure C# — no MonoBehaviour.
    /// </summary>
    public class ScoreManager
    {
        // ── Events ───────────────────────────────────────────────────────────

        /// <summary>Fired after every score update. Passes total score and points just added.</summary>
        public event Action<int, int> OnScoreChanged;

        /// <summary>Fired when a chain reaction increments. Passes new chain depth.</summary>
        public event Action<int> OnChainUpdated;

        // ── State ────────────────────────────────────────────────────────────

        public int TotalScore  { get; private set; }
        public int ChainDepth  { get; private set; }

        private readonly ScoreSettings _settings;

        // ── Constructor ──────────────────────────────────────────────────────

        public ScoreManager(ScoreSettings settings)
        {
            _settings = settings;
        }

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>Resets score and chain for a new game session.</summary>
        public void Reset()
        {
            TotalScore = 0;
            ChainDepth = 0;
        }

        /// <summary>
        /// Call once per match-resolve batch.
        /// Increments chain depth, calculates score, fires events.
        /// </summary>
        public void RegisterMatches(List<MergeResult> results)
        {
            if (results == null || results.Count == 0) return;

            int points = ScoreCalculator.Calculate(results, ChainDepth, _settings);
            TotalScore += points;
            ChainDepth++;

            OnChainUpdated?.Invoke(ChainDepth);
            OnScoreChanged?.Invoke(TotalScore, points);
        }

        /// <summary>
        /// Call when the board is refilled with no further matches.
        /// Resets chain depth back to zero.
        /// </summary>
        public void ResetChain()
        {
            ChainDepth = 0;
        }
    }
}
