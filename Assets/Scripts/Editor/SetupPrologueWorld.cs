using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SetupPrologueWorld
{
    const string ScenePath = "Assets/Scenes/Prologue.unity";
    const string FpsPrefabPath =
        "Assets/Imports/ModularFirstPersonController/FirstPersonController/FirstPersonController.prefab";

    [MenuItem("Tools/Prologue/Setup World (Floor + FPS + Cube + Spotlight)")]
    public static void Setup()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        DisableObject("Main Camera");

        EnsureFloor();
        EnsureCube();
        Light spotlight = EnsureSpotlight();
        EnsureFirstPersonController();

        PrologueUiImpl ui = Object.FindObjectOfType<PrologueUiImpl>();
        if (ui != null)
        {
            SerializedObject so = new SerializedObject(ui);
            so.FindProperty("cubeSpotlight").objectReferenceValue = spotlight;
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(ui);
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("Prologue world setup complete.");
    }

    static void DisableObject(string objectName)
    {
        GameObject go = GameObject.Find(objectName);
        if (go != null)
        {
            go.SetActive(false);
        }
    }

    static void EnsureFloor()
    {
        GameObject floor = GameObject.Find("Floor");
        if (floor == null)
        {
            floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Floor";
        }

        floor.transform.position = Vector3.zero;
        floor.transform.rotation = Quaternion.identity;
        floor.transform.localScale = new Vector3(4f, 1f, 4f);
        ApplyMaterial(floor, "Assets/Materials/PrologueFloor.mat");
    }

    static void EnsureCube()
    {
        GameObject cube = GameObject.Find("Cube");
        if (cube == null)
        {
            cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "Cube";
        }

        cube.transform.position = new Vector3(0f, 0.5f, 4f);
        cube.transform.rotation = Quaternion.identity;
        cube.transform.localScale = Vector3.one;
        ApplyMaterial(cube, "Assets/Materials/PrologueCube.mat");
    }

    static void ApplyMaterial(GameObject target, string materialPath)
    {
        Renderer renderer = target.GetComponent<Renderer>();
        Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (renderer == null || material == null)
        {
            Debug.LogError("Failed to apply material: " + materialPath);
            return;
        }

        renderer.sharedMaterial = material;
    }

    static Light EnsureSpotlight()
    {
        GameObject spotlightObject = GameObject.Find("CubeSpotlight");
        if (spotlightObject == null)
        {
            spotlightObject = new GameObject("CubeSpotlight");
        }

        spotlightObject.transform.position = new Vector3(0f, 4f, 4f);
        spotlightObject.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        Light light = spotlightObject.GetComponent<Light>();
        if (light == null)
        {
            light = spotlightObject.AddComponent<Light>();
        }

        light.type = LightType.Spot;
        light.range = 12f;
        light.spotAngle = 55f;
        light.intensity = 12f;
        light.color = Color.white;
        light.shadows = LightShadows.Soft;
        light.enabled = false;
        return light;
    }

    static void EnsureFirstPersonController()
    {
        GameObject existing = GameObject.Find("FirstPersonController");
        if (existing != null)
        {
            existing.transform.position = new Vector3(0f, 1f, 0f);
            existing.transform.rotation = Quaternion.identity;
            return;
        }

        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(FpsPrefabPath);
        if (prefab == null)
        {
            Debug.LogError("FirstPersonController prefab not found at " + FpsPrefabPath);
            return;
        }

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.name = "FirstPersonController";
        instance.transform.position = new Vector3(0f, 1f, 0f);
        instance.transform.rotation = Quaternion.identity;
    }
}
