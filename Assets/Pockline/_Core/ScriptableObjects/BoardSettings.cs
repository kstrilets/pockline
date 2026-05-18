using UnityEngine;

namespace Pockline
{
    /// <summary>
    /// All board configuration a designer can tune without touching code.
    /// </summary>
    [CreateAssetMenu(menuName = "Pockline/Board Settings", fileName = "BoardSettings_Default")]
    public class BoardSettings : ScriptableObject
    {
        [Header("Grid")]
        [Tooltip("Number of columns on the board.")]
        [Range(3, 16)]
        public int columns = 7;

        [Tooltip("Number of rows on the board.")]
        [Range(3, 16)]
        public int rows = 9;

        [Header("Gem Tiers")]
        [Tooltip("Ordered list of gem tiers. Index 0 = lowest tier. " +
                 "Matching 3 of tier N produces 1 of tier N+1. " +
                 "At the last tier a match simply scores and clears.")]
        public GemTierDefinition[] gemTiers;

        [Header("Match Rules")]
        [Tooltip("Minimum consecutive same-tier gems in a line to trigger a merge.")]
        [Range(2, 10)]
        public int minimumLineMatch = 3;

        [Tooltip("Enable horizontal line matching.")]
        public bool matchHorizontal = true;

        [Tooltip("Enable vertical line matching.")]
        public bool matchVertical = true;

        [Tooltip("Enable cluster (connected group) matching in addition to lines.")]
        public bool matchClusters = false;

        [Tooltip("Minimum cluster size to trigger a merge when cluster matching is enabled.")]
        [Range(3, 20)]
        public int minimumClusterMatch = 3;

        [Header("Animations")]
        [Tooltip("Duration in seconds of the swap animation.")]
        public float swapAnimDuration = 0.2f;

        [Tooltip("Duration in seconds of the fall animation per row.")]
        public float fallAnimDurationPerRow = 0.06f;

        [Tooltip("Pause in seconds between a match being found and gems disappearing.")]
        public float matchPauseDuration = 0.3f;

        [Header("Spawn Bias")]
        [Tooltip("0 = fully random tier distribution on spawn. " +
                 "1 = strongly biased toward placing same tiers near each other (easier to match).")]
        [Range(0f, 1f)]
        public float tierClusterBias = 0.25f;
    }
}
