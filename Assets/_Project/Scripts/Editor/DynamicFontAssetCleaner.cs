using TMPro;
using UnityEditor;

namespace Unity2DTemplate.Editor
{
    [InitializeOnLoad]
    internal static class DynamicFontAssetCleaner
    {
        private const string FontAssetPath =
            "Assets/_Project/Resources/Fonts/NotoSansJP/NotoSansJP-Dynamic.asset";

        static DynamicFontAssetCleaner()
        {
            EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
        }

        private static void HandlePlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                EditorApplication.delayCall += ClearGeneratedFontData;
            }
        }

        [MenuItem("Tools/Unity 2D Template/Clear Dynamic Font Data")]
        private static void ClearGeneratedFontData()
        {
            var fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontAssetPath);
            if (fontAsset == null ||
                (fontAsset.characterTable.Count == 0 && fontAsset.glyphTable.Count == 0))
            {
                return;
            }

            fontAsset.ClearFontAssetData(true);
            EditorUtility.SetDirty(fontAsset);
            AssetDatabase.SaveAssetIfDirty(fontAsset);
        }
    }
}
