using UnityEngine;

namespace Pockline
{
    /// <summary>
    /// Top-level game configuration asset. Drag this one SO into GameManager.
    /// Everything else is reachable from here.
    /// </summary>
    [CreateAssetMenu(menuName = "Pockline/Game Settings", fileName = "GameSettings_Default")]
    public class GameSettings : ScriptableObject
    {
        [Tooltip("Board layout, gem tiers, match rules and animation timings.")]
        public BoardSettings boardSettings;

        [Tooltip("Swap economy: starting swaps, cost, bonus rules.")]
        public SwapSettings swapSettings;

        [Tooltip("Score multipliers and chain cap.")]
        public ScoreSettings scoreSettings;

        [Tooltip("All audio clips and volumes.")]
        public AudioSettings audioSettings;

        [Tooltip("Global VFX prefabs.")]
        public VFXSettings vfxSettings;
    }
}
