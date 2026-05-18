using System;
using UnityEngine;

namespace Pockline
{
    /// <summary>
    /// Translates touch / mouse drag into grid-space swap requests.
    /// Attach to a GameObject in the scene. Assign boardOrigin and cellSize
    /// to match your BoardView layout.
    /// </summary>
    public class TouchInputHandler : MonoBehaviour, IInputHandler
    {
        public event Action<Vector2Int, Vector2Int> OnSwapRequested;

        [Tooltip("World-space position of the bottom-left corner of cell (0,0).")]
        public Vector2 boardOrigin;

        [Tooltip("World-space size of one cell.")]
        public float cellSize = 1f;

        private bool      _enabled;
        private bool      _dragging;
        private Vector2   _dragStart;
        private Vector2Int _startCell;

        // ── IInputHandler ────────────────────────────────────────────────

        public void SetEnabled(bool enabled) => _enabled = enabled;

        // ── Unity messages ───────────────────────────────────────────────

        private void Update()
        {
            if (!_enabled) return;

#if UNITY_EDITOR || UNITY_STANDALONE
            HandleMouse();
#else
            HandleTouch();
#endif
        }

        // ── Mouse (editor / standalone) ──────────────────────────────────

        private void HandleMouse()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _dragStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _startCell = WorldToGrid(_dragStart);
                _dragging  = true;
            }

            if (_dragging && Input.GetMouseButtonUp(0))
            {
                _dragging = false;
                var endWorld = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var endCell  = WorldToGrid(endWorld);
                TryFireSwap(_startCell, endCell);
            }
        }

        // ── Touch (mobile) ─────────────────────────────────────────────────

        private void HandleTouch()
        {
            if (Input.touchCount == 0) return;
            var touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                _dragStart = Camera.main.ScreenToWorldPoint(touch.position);
                _startCell = WorldToGrid(_dragStart);
                _dragging  = true;
            }

            if (_dragging && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
            {
                _dragging = false;
                var endWorld = (Vector2)Camera.main.ScreenToWorldPoint(touch.position);
                var endCell  = WorldToGrid(endWorld);
                TryFireSwap(_startCell, endCell);
            }
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private void TryFireSwap(Vector2Int from, Vector2Int to)
        {
            if (from == to) return; // tap, not drag
            OnSwapRequested?.Invoke(from, to);
        }

        private Vector2Int WorldToGrid(Vector2 world)
        {
            var local = world - boardOrigin;
            return new Vector2Int(
                Mathf.FloorToInt(local.x / cellSize),
                Mathf.FloorToInt(local.y / cellSize));
        }
    }
}
