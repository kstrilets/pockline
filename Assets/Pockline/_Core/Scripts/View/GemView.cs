using UnityEngine;

namespace Pockline
{
    /// <summary>
    /// Represents one gem on screen. Holds references to the sprite renderer
    /// and delegates animation to GemAnimator.
    /// </summary>
    public class GemView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;

        public GemData Data   { get; private set; }
        public bool IsAnimating => _animator != null && _animator.IsAnimating;

        private GemAnimator _animator;

        private void Awake()
        {
            _animator = GetComponent<GemAnimator>();
        }

        // ── Setup ───────────────────────────────────────────────────────────

        /// <summary>Binds this view to a GemData and applies visual state.</summary>
        public void Bind(GemData data)
        {
            Data = data;
            ApplyVisuals();
        }

        private void ApplyVisuals()
        {
            if (Data?.tier == null) return;
            _spriteRenderer.sprite = Data.tier.icon;
            _spriteRenderer.color  = Data.tier.tintColor;
        }

        // ── Animation forwarding ───────────────────────────────────────────

        public void PlaySwap(Vector3 targetWorld, float duration) =>
            _animator?.PlaySwap(targetWorld, duration);

        public void PlayFall(Vector3 targetWorld, float duration) =>
            _animator?.PlayFall(targetWorld, duration);

        public void PlayMerge(System.Action onComplete) =>
            _animator?.PlayMerge(onComplete);

        public void PlaySpawn() =>
            _animator?.PlaySpawn();

        /// <summary>Snaps to a world position with no animation.</summary>
        public void SnapTo(Vector3 worldPos) =>
            transform.position = worldPos;
    }
}
