using UnityEngine;

namespace Pockline
{
    /// <summary>
    /// Global VFX prefab references. Per-combination VFX lives on BonusSwapRule directly.
    /// </summary>
    [CreateAssetMenu(menuName = "Pockline/VFX Settings", fileName = "VFXSettings_Default")]
    public class VFXSettings : ScriptableObject
    {
        [Tooltip("Spawned at a gem when it merges into the next tier.")]
        public GameObject mergeVFXPrefab;

        [Tooltip("Spawned at a gem when an invalid swap is attempted.")]
        public GameObject invalidSwapVFXPrefab;

        [Tooltip("Spawned at a gem when it falls into an empty slot.")]
        public GameObject gemSpawnVFXPrefab;

        [Tooltip("Spawned at board centre when the player runs out of swaps.")]
        public GameObject gameOverVFXPrefab;
    }
}
