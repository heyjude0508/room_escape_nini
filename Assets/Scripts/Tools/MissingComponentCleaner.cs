using UnityEditor;
using UnityEngine;

public class CleanMissingScripts : EditorWindow
{
    [MenuItem("Tools/Missing Scripts Cleaner")]
    public static void CleanAllMissingScripts()
    {
        int count = 0;
        GameObject[] allObjects = Object.FindObjectsOfType<GameObject>(includeInactive: true);

        foreach (GameObject go in allObjects)
        {
            count += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
        }

        EditorUtility.DisplayDialog("清理完成", $"共删除了 {count} 个 Missing Script 组件", "确定");
    }

}
