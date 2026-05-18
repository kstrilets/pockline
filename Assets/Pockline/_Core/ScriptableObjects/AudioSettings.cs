using UnityEngine;

namespace Pockline
{
    /// <summary>
    /// All audio configuration in one designer-friendly asset.
    /// </summary>
    [CreateAssetMenu(menuName = "Pockline/Audio Settings", fileName = "AudioSettings_Default")]
    public class AudioSettings : ScriptableObject
    {
        [Header("Music")]
        public AudioClip backgroundMusic;
        [Range(0f, 1f)] public float musicVolume = 0.6f;

        [Header("SFX")]
        public AudioClip swapSFX;
        public AudioClip invalidSwapSFX;
        public AudioClip mergeSFX;
        public AudioClip gemFallSFX;
        public AudioClip gameOverSFX;
        [Range(0f, 1f)] public float sfxVolume = 0.9f;
    }
}
