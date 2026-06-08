using UnityEditor;
using UnityEngine;
using System.IO;

public class MissingPrefabCleaner : EditorWindow
{
    [MenuItem("Tools/Missing Prefab Cleaner")]
    public static void CleanPrefabMissingScripts()
    {
        int totalRemoved = 0;
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");

        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab == null) continue;

            // 打开Prefab编辑模式
            PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(prefab);
            if (prefabType == PrefabAssetType.Regular || prefabType == PrefabAssetType.Variant)
            {
                int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(prefab);
                if (removed > 0)
                {
                    totalRemoved += removed;
                    PrefabUtility.SavePrefabAsset(prefab);
                    Debug.Log($"清理了Prefab {path} 里的 {removed} 个Missing Script");
                }
            }
        }

        EditorUtility.DisplayDialog("清理完成", $"所有Prefab里共删除了 {totalRemoved} 个Missing Script组件", "确定");
    }
}