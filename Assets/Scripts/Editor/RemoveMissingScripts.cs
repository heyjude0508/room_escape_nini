using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class RemoveMissingScripts
{
    [MenuItem("Tools/Remove Missing Scripts In Open Scene")]
    static void RemoveInOpenScene()
    {
        int removed = RemoveMissingFromObjects(
            Object.FindObjectsOfType<GameObject>(true));

        if (removed > 0)
        {
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        Debug.Log($"Removed {removed} missing script component(s) from the open scene.");
    }

    [MenuItem("Tools/Remove Missing Scripts In Project Scenes")]
    static void RemoveInProjectScenes()
    {
        string activeScenePath = SceneManager.GetActiveScene().path;
        int totalRemoved = 0;

        foreach (string guid in AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" }))
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            int removed = RemoveMissingFromObjects(
                Object.FindObjectsOfType<GameObject>(true));

            if (removed > 0)
            {
                EditorSceneManager.SaveScene(scene);
                Debug.Log($"Removed {removed} missing script component(s) from {scenePath}.");
                totalRemoved += removed;
            }
        }

        if (!string.IsNullOrEmpty(activeScenePath))
        {
            EditorSceneManager.OpenScene(activeScenePath, OpenSceneMode.Single);
        }

        Debug.Log($"Finished. Removed {totalRemoved} missing script component(s) from project scenes.");
    }

    [MenuItem("Tools/Remove Missing Scripts In Prefabs")]
    static void RemoveInPrefabs()
    {
        int totalRemoved = 0;

        foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" }))
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
            int removed = RemoveMissingFromObjects(prefabRoot.GetComponentsInChildren<Transform>(true));

            if (removed > 0)
            {
                PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
                Debug.Log($"Removed {removed} missing script component(s) from {prefabPath}.");
                totalRemoved += removed;
            }

            PrefabUtility.UnloadPrefabContents(prefabRoot);
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Finished. Removed {totalRemoved} missing script component(s) from prefabs.");
    }

    static int RemoveMissingFromObjects(GameObject[] gameObjects)
    {
        int removed = 0;

        foreach (GameObject gameObject in gameObjects)
        {
            removed += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);
        }

        return removed;
    }

    static int RemoveMissingFromObjects(Transform[] transforms)
    {
        int removed = 0;

        foreach (Transform transform in transforms)
        {
            removed += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(transform.gameObject);
        }

        return removed;
    }
}
