using UnityEngine;

namespace Pockline
{
    /// <summary>
    /// Scoring configuration.
    /// </summary>
    [CreateAssetMenu(menuName = "Pockline/Score Settings", fileName = "ScoreSettings_Default")]
    public class ScoreSettings : ScriptableObject
    {
        [Tooltip("Multiplier applied to the base merge score for each chain reaction step. " +
                 "e.g. 1.5 means chain-2 scores 1.5x, chain-3 scores 2.25x.")]
        [Range(1f, 4f)]
        public float chainMultiplier = 1.5f;

        [Tooltip("Maximum chain multiplier cap.")]
        [Range(1, 32)]
        public int chainMultiplierCap = 8;
    }
}
