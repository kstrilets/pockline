#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Pockline.Editor
{
    /// <summary>
    /// Custom Inspector for GemTierDefinition.
    /// Shows a live preview of the tinted icon sprite.
    /// </summary>
    [CustomEditor(typeof(GemTierDefinition))]
    public class GemTierEditor : UnityEditor.Editor
    {
        private const float kPreviewSize = 80f;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var def = (GemTierDefinition)target;
            if (def.icon == null) return;

            GUILayout.Space(8);
            GUILayout.Label("Icon Preview", EditorStyles.boldLabel);

            var rect = GUILayoutUtility.GetRect(kPreviewSize, kPreviewSize,
                GUILayout.ExpandWidth(false));

            // Draw the sprite texture tinted by the chosen color
            var prevColor = GUI.color;
            GUI.color = def.tintColor;
            var tex = AssetPreview.GetAssetPreview(def.icon);
            if (tex != null)
                GUI.DrawTexture(rect, tex, ScaleMode.ScaleToFit, true);
            else
                EditorGUI.DrawRect(rect, def.tintColor * 0.5f);
            GUI.color = prevColor;
        }
    }
}
#endif
