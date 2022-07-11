#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EditorExtension.CuriousInspector.Settings
{
    public class SettingsData : ScriptableObject
    {
        #region Getting the settings data object
        private const string ASSET_FOLDER = "ToolData";
        private const string ASSET_NAME = "SettingsData";

        private static SettingsData settingsData;
        public static SettingsData GetSettingsData()
        {
            if (settingsData == null)
            {
                if (!AssetDatabase.IsValidFolder(GetAssetFolderPath()))
                {
                    AssetDatabase.CreateFolder("Assets", ASSET_FOLDER);
                }
                settingsData = (SettingsData)AssetDatabase.LoadAssetAtPath(GetAssetPath(), typeof(SettingsData));
                if (settingsData == null)
                {
                    AssetDatabase.CreateAsset(CreateInstance<SettingsData>(), GetAssetPath());
                    AssetDatabase.ImportAsset(GetAssetPath());
                    settingsData = (SettingsData)AssetDatabase.LoadAssetAtPath(GetAssetPath(), typeof(SettingsData));
                    if (settingsData == null)
                    {
                        Debug.LogError("The new pages data file could not be created at path: " + GetAssetPath() + " !");
                    }
                    return null;
                }
            }
            return settingsData;
        }

        private static string GetAssetFolderPath()
        {
            return "Assets/" + ASSET_FOLDER;
        }

        private static string GetAssetPath()
        {
            return "Assets/" + ASSET_FOLDER + "/" + ASSET_NAME + ".asset";
        }

        #endregion

        public Color TextLabelColor = Color.white;

        public bool InspectorDebug;
        public bool ContainersDebug;
        public bool DrawDebug;
    }
}
#endif
