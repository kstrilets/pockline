using UnityEngine;

namespace Pockline
{
    /// <summary>
    /// Configures the swap economy for a session.
    /// </summary>
    [CreateAssetMenu(menuName = "Pockline/Swap Settings", fileName = "SwapSettings_Default")]
    public class SwapSettings : ScriptableObject
    {
        [Tooltip("Number of swaps the player starts with.")]
        [Range(1, 500)]
        public int startingSwaps = 30;

        [Tooltip("Cost in swaps for each player-initiated swap. Always 1 by design.")]
        [Range(1, 5)]
        public int swapCost = 1;

        [Tooltip("Bonus swap rules — each asset defines a combination that awards extra swaps. " +
                 "Add new BonusSwapRule assets here to extend without changing code.")]
        public BonusSwapRule[] bonusSwapRules;
    }
}
