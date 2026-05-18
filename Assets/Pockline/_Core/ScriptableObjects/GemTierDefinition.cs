using UnityEngine;

namespace Pockline
{
    /// <summary>
    /// Defines one tier in the gem evolution chain.
    /// Tiers are ordered by their index in BoardSettings.gemTiers[].
    /// Shape is purely cosmetic — match logic only compares tier index.
    /// </summary>
    [CreateAssetMenu(menuName = "Pockline/Gem Tier Definition", fileName = "GemTier_New")]
    public class GemTierDefinition : ScriptableObject
    {
        [Tooltip("Display name shown in UI (e.g. Grey, Green, Blue, Yellow)")]
        public string tierName;

        [Tooltip("Sprite rendered on the gem.")]
        public Sprite icon;

        [Tooltip("Tint color applied to the gem sprite renderer.")]
        public Color tintColor = Color.white;

        [Tooltip("Base score awarded when a gem of this tier is produced by a merge.")]
        public int mergeScore = 100;

        [Tooltip("Relative spawn weight on the board. Higher = appears more often during fill.")]
        [Range(0.1f, 10f)]
        public float spawnWeight = 1f;
    }
}
