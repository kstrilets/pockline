using UnityEngine;

namespace Pockline
{
    /// <summary>
    /// Plays music and SFX. Subscribes to game events directly.
    /// Uses two AudioSources: one for looping music, one for one-shot SFX.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        private AudioSource _musicSource;
        private AudioSource _sfxSource;
        private AudioSettings _settings;

        // ── Initialisation ────────────────────────────────────────────────

        public void Initialise(
            AudioSettings settings,
            BoardController board,
            SwapManager swapManager,
            GameOverState gameOverState)
        {
            _settings = settings;

            // Music source
            _musicSource        = gameObject.AddComponent<AudioSource>();
            _musicSource.clip   = settings.backgroundMusic;
            _musicSource.loop   = true;
            _musicSource.volume = settings.musicVolume;
            _musicSource.playOnAwake = false;

            // SFX source
            _sfxSource        = gameObject.AddComponent<AudioSource>();
            _sfxSource.loop   = false;
            _sfxSource.volume = settings.sfxVolume;
            _sfxSource.playOnAwake = false;

            // Subscribe to events
            board.OnSwapExecuted  += (c1,r1,c2,r2) => PlaySFX(settings.swapSFX);
            board.OnSwapReverted  += (c1,r1,c2,r2) => PlaySFX(settings.invalidSwapSFX);
            board.OnBoardRefilled += ()             => PlaySFX(settings.gemFallSFX);
            swapManager.OnBonusSwapsAwarded += (amount, rule) =>
            {
                if (rule?.bonusSFX != null) PlaySFX(rule.bonusSFX);
            };
            gameOverState.OnGameOver += () => PlaySFX(settings.gameOverSFX);
        }

        // ── Public API ───────────────────────────────────────────────────────

        public void PlayMusic()
        {
            if (_musicSource == null || _musicSource.clip == null) return;
            _musicSource.Play();
        }

        public void StopMusic() => _musicSource?.Stop();

        public void PlaySFX(AudioClip clip)
        {
            if (clip == null || _sfxSource == null) return;
            _sfxSource.PlayOneShot(clip);
        }

        public void SetMusicVolume(float v)  => _musicSource.volume = Mathf.Clamp01(v);
        public void SetSFXVolume(float v)    => _sfxSource.volume   = Mathf.Clamp01(v);
    }
}
