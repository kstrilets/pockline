using System;
using System.Collections;
using UnityEngine;

namespace Pockline
{
    /// <summary>
    /// Handles all animations for a single GemView using Unity coroutines.
    /// No DOTween dependency — easy to swap in a tween library later.
    /// </summary>
    [RequireComponent(typeof(GemView))]
    public class GemAnimator : MonoBehaviour
    {
        public bool IsAnimating { get; private set; }

        // ── Swap ─────────────────────────────────────────────────────────────

        public void PlaySwap(Vector3 target, float duration)
        {
            StopAllCoroutines();
            StartCoroutine(MoveRoutine(target, duration));
        }

        // ── Fall ─────────────────────────────────────────────────────────────

        public void PlayFall(Vector3 target, float duration)
        {
            StopAllCoroutines();
            StartCoroutine(MoveRoutine(target, duration));
        }

        // ── Merge ────────────────────────────────────────────────────────────

        public void PlayMerge(Action onComplete)
        {
            StopAllCoroutines();
            StartCoroutine(MergeRoutine(onComplete));
        }

        // ── Spawn ────────────────────────────────────────────────────────────

        public void PlaySpawn()
        {
            StopAllCoroutines();
            StartCoroutine(SpawnRoutine());
        }

        // ── Coroutines ─────────────────────────────────────────────────────────

        private IEnumerator MoveRoutine(Vector3 target, float duration)
        {
            IsAnimating = true;
            var start   = transform.position;
            float t     = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / Mathf.Max(duration, 0.001f);
                transform.position = Vector3.Lerp(start, target, Mathf.SmoothStep(0f, 1f, t));
                yield return null;
            }
            transform.position = target;
            IsAnimating = false;
        }

        private IEnumerator MergeRoutine(Action onComplete)
        {
            IsAnimating = true;
            // Scale punch: grow then shrink to zero
            float duration = 0.2f;
            float t = 0f;
            var   originalScale = transform.localScale;
            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                float scale = Mathf.Lerp(1f, 1.4f, Mathf.Sin(t * Mathf.PI));
                transform.localScale = originalScale * scale;
                yield return null;
            }
            transform.localScale = Vector3.zero;
            IsAnimating = false;
            onComplete?.Invoke();
        }

        private IEnumerator SpawnRoutine()
        {
            IsAnimating = true;
            transform.localScale = Vector3.zero;
            float duration = 0.15f;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                transform.localScale = Vector3.one * Mathf.SmoothStep(0f, 1f, t);
                yield return null;
            }
            transform.localScale = Vector3.one;
            IsAnimating = false;
        }
    }
}
