using System.Collections.Generic;

namespace Pockline
{
    /// <summary>
    /// Checks a list of MergeResults against all BonusSwapRules.
    /// Returns the total number of bonus swaps earned.
    /// Pure C# — no MonoBehaviour.
    /// Adding new bonus conditions = add a new BonusSwapRule SO asset. Zero code changes.
    /// </summary>
    public class BonusSwapEvaluator
    {
        private readonly SwapSettings _swapSettings;

        public BonusSwapEvaluator(SwapSettings swapSettings)
        {
            _swapSettings = swapSettings;
        }

        /// <summary>
        /// Evaluates all merge results against all bonus rules.
        /// Returns total bonus swaps earned and populates triggeredRules with
        /// the rules that fired (for UI announcements and SFX/VFX).
        /// </summary>
        public int Evaluate(
            List<MergeResult> results,
            out List<BonusSwapRule> triggeredRules)
        {
            triggeredRules = new List<BonusSwapRule>();
            int total = 0;

            if (_swapSettings.bonusSwapRules == null) return 0;

            foreach (var rule in _swapSettings.bonusSwapRules)
            {
                if (rule == null) continue;
                foreach (var result in results)
                {
                    if (RuleFires(rule, result))
                    {
                        total += rule.bonusSwaps;
                        triggeredRules.Add(rule);
                        break; // one trigger per rule per swap
                    }
                }
            }

            return total;
        }

        // ── Private ─────────────────────────────────────────────────────────

        private static bool RuleFires(BonusSwapRule rule, MergeResult result)
        {
            // Line trigger
            if (result.isLineMatch && result.lineLength >= rule.minimumLineLength)
                return true;

            // Cluster trigger
            if (rule.allowClusterTrigger
                && result.isClusterMatch
                && result.Count >= rule.minimumClusterSize)
                return true;

            return false;
        }
    }
}
