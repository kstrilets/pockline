using System;

namespace Pockline
{
    /// <summary>
    /// Pure runtime data for one gem on the board.
    /// No MonoBehaviour — fully testable without Unity.
    /// Shape is stored but ignored by all match logic (cosmetic only).
    /// </summary>
    public class GemData
    {
        // ── Identity ────────────────────────────────────────────────────────

        /// <summary>Index into BoardSettings.gemTiers[]. This is what match logic compares.</summary>
        public int tierIndex;

        /// <summary>Reference to the tier SO for display data (icon, tint, score).</summary>
        public GemTierDefinition tier;

        // ── Grid position ────────────────────────────────────────────────────

        public int col;
        public int row;

        // ── State flags ──────────────────────────────────────────────────────

        /// <summary>Flagged true by MatchFinder when this gem is part of a resolved match.</summary>
        public bool isMatched;

        /// <summary>True while the gem is animating (falling, swapping). BoardController waits for this.</summary>
        public bool isAnimating;

        // ── Constructor ──────────────────────────────────────────────────────

        public GemData(int tierIndex, GemTierDefinition tier, int col, int row)
        {
            this.tierIndex  = tierIndex;
            this.tier       = tier;
            this.col        = col;
            this.row        = row;
            isMatched       = false;
            isAnimating     = false;
        }

        public override string ToString() =>
            $"Gem[{col},{row}] tier={tierIndex}({tier?.tierName ?? "null"}), matched={isMatched}";
    }
}
