using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Pockline
{
    /// <summary>
    /// Displays the current swap count as a simple integer label.
    /// Reacts to SwapManager.OnSwapCountChanged.
    /// Attach to the swap counter Text/TMP object in the HUD.
    /// </summary>
    public class SwapCounterUI : MonoBehaviour
    {
        [Tooltip("TextMeshPro label. Assign in Inspector.")]
        [SerializeField] private TMP_Text _label;

        private SwapManager _swapManager;

        public void Initialise(SwapManager swapManager)
        {
            _swapManager = swapManager;
            _swapManager.OnSwapCountChanged += UpdateDisplay;
            UpdateDisplay(_swapManager.CurrentSwaps);
        }

        private void OnDestroy()
        {
            if (_swapManager != null)
                _swapManager.OnSwapCountChanged -= UpdateDisplay;
        }

        private void UpdateDisplay(int count)
        {
            if (_label != null)
                _label.text = count.ToString();
        }
    }
}
