#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Scatters Litter FBX instances (and their tripo_part_* grains) around SimpleLitterBox.
/// Menu: Tools/Room Escape/Arrange Litter Around Box
/// </summary>
public static class ArrangeLitterAroundBox
{
    const string LitterAssetPath = "Assets/Resources/Models/Litter/Litter.fbx";
    const string SessionKey = "ArrangeLitterAroundBox_v2";
    const int DesiredClumps = 5;

    static ArrangeLitterAroundBox()
    {
        EditorApplication.delayCall += () =>
        {
            if (SessionState.GetBool(SessionKey, false)) return;
            if (Arrange(silent: true))
                SessionState.SetBool(SessionKey, true);
        };
    }

    [MenuItem("Tools/Room Escape/Arrange Litter Around Box")]
    static void MenuArrange()
    {
        SessionState.SetBool(SessionKey, true);
        Arrange(silent: false);
    }

    /// <summary>Unity batchmode entry: -executeMethod ArrangeLitterAroundBox.BatchArrange</summary>
    public static void BatchArrange()
    {
        var scenePath = "Assets/Scenes/HouseChild.unity";
        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        if (!Arrange(silent: false))
        {
            Debug.LogError("ArrangeLitterAroundBox.BatchArrange failed.");
            EditorApplication.Exit(1);
            return;
        }
        EditorSceneManager.SaveScene(scene);
        Debug.Log("ArrangeLitterAroundBox.BatchArrange saved " + scenePath);
        EditorApplication.Exit(0);
    }

    static bool Arrange(bool silent)
    {
        var box = FindBox();
        if (box == null)
        {
            if (!silent)
                Debug.LogWarning("ArrangeLitterAroundBox: SimpleLitterBox not found in open scenes.");
            return false;
        }

        var litterAsset = AssetDatabase.LoadAssetAtPath<GameObject>(LitterAssetPath);
        if (litterAsset == null)
        {
            if (!silent)
                Debug.LogError("ArrangeLitterAroundBox: missing " + LitterAssetPath);
            return false;
        }

        var clumps = FindLitterRoots().ToList();
        var parent = box.transform.parent != null ? box.transform.parent : box.transform.root;

        while (clumps.Count < DesiredClumps)
        {
            var inst = (GameObject)PrefabUtility.InstantiatePrefab(litterAsset, parent);
            Undo.RegisterCreatedObjectUndo(inst, "Create Litter");
            inst.name = clumps.Count == 0 ? "Litter" : $"Litter ({clumps.Count})";
            clumps.Add(inst.transform);
        }

        // Floor height: litter box sits slightly above floor under Test
        Vector3 boxPos = box.transform.position;
        float floorY = boxPos.y - 0.02f;

        // Clump anchors around the box (local to parent if same space — use world then convert)
        Vector3[] offsets =
        {
            new Vector3(-0.22f, 0f, -0.08f),
            new Vector3(0.28f, 0f, -0.12f),
            new Vector3(0.05f, 0f, 0.26f),
            new Vector3(-0.18f, 0f, 0.22f),
            new Vector3(0.32f, 0f, 0.18f),
            new Vector3(-0.30f, 0f, 0.05f),
            new Vector3(0.12f, 0f, -0.28f),
        };

        var rng = new System.Random(20260929);
        for (int i = 0; i < clumps.Count; i++)
        {
            var root = clumps[i];
            Undo.RecordObject(root, "Arrange Litter");

            Vector3 o = offsets[i % offsets.Length];
            o.x += (float)(rng.NextDouble() * 0.08 - 0.04);
            o.z += (float)(rng.NextDouble() * 0.08 - 0.04);

            Vector3 world = new Vector3(boxPos.x + o.x, floorY, boxPos.z + o.z);
            root.position = world;
            root.rotation = Quaternion.Euler(0f, (float)rng.NextDouble() * 360f, 0f);
            float s = Mathf.Lerp(0.35f, 0.7f, (float)rng.NextDouble());
            root.localScale = new Vector3(s, s, s);

            ScatterParts(root, rng);
            EditorUtility.SetDirty(root);
        }

        EditorSceneManager.MarkSceneDirty(box.scene);
        if (!silent)
            Debug.Log($"ArrangeLitterAroundBox: scattered {clumps.Count} Litter clumps around {box.name}.");
        return true;
    }

    static void ScatterParts(Transform root, System.Random rng)
    {
        for (int i = 0; i < root.childCount; i++)
        {
            var part = root.GetChild(i);
            if (!part.name.StartsWith("tripo_part")) continue;

            Undo.RecordObject(part, "Scatter Litter Part");

            // Keep near clump origin but messy
            Vector3 lp = new Vector3(
                (float)(rng.NextDouble() * 0.22 - 0.11),
                (float)(rng.NextDouble() * 0.02),
                (float)(rng.NextDouble() * 0.22 - 0.11));
            part.localPosition = lp;
            part.localRotation = Quaternion.Euler(
                (float)rng.NextDouble() * 360f,
                (float)rng.NextDouble() * 360f,
                (float)rng.NextDouble() * 360f);
            float ps = Mathf.Lerp(0.7f, 1.35f, (float)rng.NextDouble());
            part.localScale = new Vector3(ps, ps, ps);
            EditorUtility.SetDirty(part);
        }
    }

    static GameObject FindBox()
    {
        foreach (var t in Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (t.name == "SimpleLitterBox") return t.gameObject;
        }
        return null;
    }

    static IEnumerable<Transform> FindLitterRoots()
    {
        var seen = new HashSet<int>();
        foreach (var t in Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (t.parent == null) continue;
            bool isLitterRoot =
                t.name == "Litter" ||
                (t.name.StartsWith("Litter (") && t.name.EndsWith(")"));
            if (!isLitterRoot) continue;
            // Prefer roots that contain tripo_part children
            bool hasParts = false;
            for (int i = 0; i < t.childCount; i++)
            {
                if (t.GetChild(i).name.StartsWith("tripo_part")) { hasParts = true; break; }
            }
            if (!hasParts) continue;
            if (!seen.Add(t.GetInstanceID())) continue;
            yield return t;
        }
    }
}
#endif
