using System;
using System.Collections.Generic;

namespace Pockline
{
    /// <summary>
    /// Owns the swap counter. Deducts cost on each swap, adds bonuses
    /// when BonusSwapEvaluator finds triggered rules, and fires events
    /// for the UI and GameStateMachine to react to.
    /// Pure C# — no MonoBehaviour.
    /// </summary>
    public class SwapManager
    {
        // ── Events ────────────────────────────────────────────────────────────

        /// <summary>Fired whenever the swap count changes. Passes the new count.</summary>
        public event Action<int> OnSwapCountChanged;

        /// <summary>Fired when bonus swaps are awarded. Passes amount and the rule that triggered.</summary>
        public event Action<int, BonusSwapRule> OnBonusSwapsAwarded;

        /// <summary>Fired when the swap count reaches zero.</summary>
        public event Action OnSwapsExhausted;

        // ── State ─────────────────────────────────────────────────────────────

        public int CurrentSwaps { get; private set; }

        private readonly SwapSettings        _settings;
        private readonly BonusSwapEvaluator  _evaluator;

        // ── Constructor ───────────────────────────────────────────────────────

        public SwapManager(SwapSettings settings)
        {
            _settings  = settings;
            _evaluator = new BonusSwapEvaluator(settings);
            CurrentSwaps = settings.startingSwaps;
        }

        // ── Public API ────────────────────────────────────────────────────────

        /// <summary>
        /// Resets the counter to the configured starting value.
        /// Call at the start of a new game.
        /// </summary>
        public void Reset()
        {
            CurrentSwaps = _settings.startingSwaps;
            OnSwapCountChanged?.Invoke(CurrentSwaps);
        }

        /// <summary>
        /// Attempts to spend swapCost swaps.
        /// Returns false (and does not deduct) if the player has no swaps left.
        /// </summary>
        public bool TrySpendSwap()
        {
            if (CurrentSwaps <= 0) return false;

            CurrentSwaps -= _settings.swapCost;
            if (CurrentSwaps < 0) CurrentSwaps = 0;

            OnSwapCountChanged?.Invoke(CurrentSwaps);

            if (CurrentSwaps <= 0)
                OnSwapsExhausted?.Invoke();

            return true;
        }

        /// <summary>
        /// Evaluates merge results against all bonus rules and adds earned swaps.
        /// Call this after every successful match resolution.
        /// </summary>
        public void EvaluateBonuses(List<MergeResult> results)
        {
            int bonus = _evaluator.Evaluate(results, out var triggered);
            if (bonus <= 0) return;

            CurrentSwaps += bonus;
            OnSwapCountChanged?.Invoke(CurrentSwaps);

            foreach (var rule in triggered)
                OnBonusSwapsAwarded?.Invoke(rule.bonusSwaps, rule);
        }

        /// <summary>Adds swaps directly (e.g. from a future power-up system).</summary>
        public void AddSwaps(int amount)
        {
            if (amount <= 0) return;
            CurrentSwaps += amount;
            OnSwapCountChanged?.Invoke(CurrentSwaps);
        }
    }
}
