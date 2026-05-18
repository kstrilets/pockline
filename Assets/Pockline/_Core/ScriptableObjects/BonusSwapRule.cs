using UnityEngine;

namespace Pockline
{
    /// <summary>
    /// Defines a combination that awards bonus swaps.
    /// New rules can be added as new assets without changing code.
    /// </summary>
    [CreateAssetMenu(menuName = "Pockline/Bonus Swap Rule", fileName = "BonusSwapRule_New")]
    public class BonusSwapRule : ScriptableObject
    {
        [Tooltip("Human-readable name shown in UI (e.g. Five in a Row).")]
        public string ruleName;

        [Tooltip("Minimum number of identical-tier gems that must be matched in a single line to trigger this rule.")]
        [Range(2, 20)]
        public int minimumLineLength = 5;

        [Tooltip("Whether a cluster (non-line) of this size also triggers the rule.")]
        public bool allowClusterTrigger = false;

        [Tooltip("Minimum cluster size if allowClusterTrigger is true.")]
        [Range(2, 30)]
        public int minimumClusterSize = 5;

        [Tooltip("Number of swaps awarded when this rule is triggered.")]
        [Range(1, 50)]
        public int bonusSwaps = 3;

        [Tooltip("Icon displayed in the bonus-swap announcement UI.")]
        public Sprite announcementIcon;

        [Tooltip("SFX played when bonus swaps are awarded.")]
        public AudioClip bonusSFX;

        [Tooltip("VFX prefab spawned at the match centre when bonus swaps are awarded.")]
        public GameObject bonusVFXPrefab;
    }
}
