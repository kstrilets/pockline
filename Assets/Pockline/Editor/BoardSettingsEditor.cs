#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Pockline.Editor
{
    /// <summary>
    /// Custom Inspector for BoardSettings.
    /// Draws a compact grid preview showing column × row count
    /// and flags common misconfiguration.
    /// </summary>
    [CustomEditor(typeof(BoardSettings))]
    public class BoardSettingsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var bs = (BoardSettings)target;

            GUILayout.Space(8);
            GUILayout.Label("Board Preview", EditorStyles.boldLabel);

            // Draw a tiny grid
            float cellPx = 14f;
            float w = bs.columns * cellPx;
            float h = bs.rows    * cellPx;
            var rect = GUILayoutUtility.GetRect(w, h, GUILayout.ExpandWidth(false));

            EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f));
            for (int c = 0; c < bs.columns; c++)
                for (int r = 0; r < bs.rows; r++)
                {
                    var cell = new Rect(
                        rect.x + c * cellPx + 1,
                        rect.y + (bs.rows - 1 - r) * cellPx + 1,
                        cellPx - 2, cellPx - 2);
                    EditorGUI.DrawRect(cell, new Color(0.3f, 0.5f, 0.8f, 0.6f));
                }

            GUILayout.Space(4);
            EditorGUILayout.HelpBox(
                $"{bs.columns} × {bs.rows} = {bs.columns * bs.rows} cells   |   " +
                $"Tiers defined: {(bs.gemTiers != null ? bs.gemTiers.Length : 0)}",
                MessageType.Info);

            // Warnings
            if (bs.gemTiers == null || bs.gemTiers.Length == 0)
                EditorGUILayout.HelpBox("gemTiers is empty — the board will not fill correctly.",
                    MessageType.Error);

            if (bs.minimumLineMatch > bs.columns && bs.matchHorizontal)
                EditorGUILayout.HelpBox(
                    "minimumLineMatch is greater than the column count — horizontal matches are impossible.",
                    MessageType.Warning);

            if (bs.minimumLineMatch > bs.rows && bs.matchVertical)
                EditorGUILayout.HelpBox(
                    "minimumLineMatch is greater than the row count — vertical matches are impossible.",
                    MessageType.Warning);
        }
    }
}
#endif
