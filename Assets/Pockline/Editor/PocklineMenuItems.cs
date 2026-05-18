#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Pockline.Editor
{
    /// <summary>
    /// Top-level Pockline menu in the Unity Editor menu bar.
    /// </summary>
    public static class PocklineMenuItems
    {
        private const string kMenu = "Pockline/";

        // ── ScriptableObject quick-create ─────────────────────────────────────

        [MenuItem(kMenu + "Create/Game Settings")]
        public static void CreateGameSettings()
            => CreateAsset<GameSettings>("Assets/Pockline/Settings", "GameSettings_Default");

        [MenuItem(kMenu + "Create/Board Settings")]
        public static void CreateBoardSettings()
            => CreateAsset<BoardSettings>("Assets/Pockline/Settings", "BoardSettings_Default");

        [MenuItem(kMenu + "Create/Swap Settings")]
        public static void CreateSwapSettings()
            => CreateAsset<SwapSettings>("Assets/Pockline/Settings", "SwapSettings_Default");

        [MenuItem(kMenu + "Create/Score Settings")]
        public static void CreateScoreSettings()
            => CreateAsset<ScoreSettings>("Assets/Pockline/Settings", "ScoreSettings_Default");

        [MenuItem(kMenu + "Create/Audio Settings")]
        public static void CreateAudioSettings()
            => CreateAsset<AudioSettings>("Assets/Pockline/Settings", "AudioSettings_Default");

        [MenuItem(kMenu + "Create/VFX Settings")]
        public static void CreateVFXSettings()
            => CreateAsset<VFXSettings>("Assets/Pockline/Settings", "VFXSettings_Default");

        [MenuItem(kMenu + "Create/Gem Tier Definition")]
        public static void CreateGemTierDefinition()
            => CreateAsset<GemTierDefinition>("Assets/Pockline/Gems", "GemTier_New");

        [MenuItem(kMenu + "Create/Bonus Swap Rule")]
        public static void CreateBonusSwapRule()
            => CreateAsset<BonusSwapRule>("Assets/Pockline/Settings", "BonusSwapRule_New");

        // ── Validation ─────────────────────────────────────────────────────────

        [MenuItem(kMenu + "Validate Settings")]
        public static void ValidateSettings()
        {
            var allSettings = AssetDatabaseHelper.FindAll<GameSettings>();
            if (allSettings.Length == 0)
            {
                Debug.LogWarning("[Pockline] No GameSettings asset found in the project.");
                return;
            }

            int errors = 0;
            foreach (var gs in allSettings)
            {
                if (gs.boardSettings == null)  { Debug.LogError($"[Pockline] {gs.name}: boardSettings is null.");  errors++; }
                if (gs.swapSettings  == null)  { Debug.LogError($"[Pockline] {gs.name}: swapSettings is null.");   errors++; }
                if (gs.scoreSettings == null)  { Debug.LogError($"[Pockline] {gs.name}: scoreSettings is null.");  errors++; }
                if (gs.audioSettings == null)  { Debug.LogError($"[Pockline] {gs.name}: audioSettings is null.");  errors++; }
                if (gs.vfxSettings   == null)  { Debug.LogError($"[Pockline] {gs.name}: vfxSettings is null.");    errors++; }

                if (gs.boardSettings != null && (gs.boardSettings.gemTiers == null || gs.boardSettings.gemTiers.Length == 0))
                { Debug.LogError($"[Pockline] {gs.name}: boardSettings.gemTiers is empty."); errors++; }
            }

            if (errors == 0)
                Debug.Log("[Pockline] All settings validated ✅ No issues found.");
            else
                Debug.LogError($"[Pockline] Validation complete — {errors} error(s) found.");
        }

        // ── Helpers ────────────────────────────────────────────────────────────

        private static void CreateAsset<T>(string folder, string assetName) where T : ScriptableObject
        {
            if (!AssetDatabase.IsValidFolder(folder))
            {
                System.IO.Directory.CreateDirectory(folder);
                AssetDatabase.Refresh();
            }
            var asset = ScriptableObject.CreateInstance<T>();
            string path = AssetDatabase.GenerateUniqueAssetPath($"{folder}/{assetName}.asset");
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
            Debug.Log($"[Pockline] Created {typeof(T).Name} at {path}");
        }
    }

    internal static class AssetDatabaseHelper
    {
        public static T[] FindAll<T>() where T : ScriptableObject
        {
            var guids  = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            var result = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++)
                result[i] = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[i]));
            return result;
        }
    }
}
#endif
