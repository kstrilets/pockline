using System;
using UnityEngine;

namespace Pockline
{
    /// <summary>
    /// Abstraction over input source. Swap the implementation to change
    /// from touch to keyboard, AI, replay, etc. without touching any other system.
    /// </summary>
    public interface IInputHandler
    {
        /// <summary>
        /// Fired when the player completes a drag gesture between two grid cells.
        /// Vector2Int = (col, row).
        /// </summary>
        event Action<Vector2Int, Vector2Int> OnSwapRequested;

        void SetEnabled(bool enabled);
    }
}
