using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor-only procedural layout tool for Aisle Be Back.
/// Creates a replaceable store layout and never deletes scene objects outside its own generated root.
/// </summary>
public class AisleBeBackStoreGenerator : EditorWindow
{
    private const string AssetRoot = "Assets/ModernSupermarket/Prefabs";
    private const string GeneratedRootName = "GENERATED - Aisle Be Back Store";

    [Header("Store Layout")]
    [SerializeField] private int aisleRows = 5;
    [SerializeField] private int shelfUnitsPerRow = 6;
    [SerializeField] private float shelfSpacing = 3.2f;
    [SerializeField] private float aisleSpacing = 5.0f;
    [SerializeField] private float storeWidth = 42f;
    [SerializeField] private float storeLength = 52f;
    [SerializeField] private float wallHeight = 6f;
    [SerializeField] private float wallThickness = 0.5f;

    [Header("Asset Prefabs (optional; auto-detected on first open)")]
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject shelfPrefab;
    [SerializeField] private GameObject checkoutPrefab;
    [SerializeField] private GameObject productPrefab;
    [SerializeField] private GameObject producePrefab;
    [SerializeField] private GameObject signPrefab;

    [Header("Details")]
    [SerializeField] private int productsPerShelfUnit = 3;
    [SerializeField] private bool addProducts = true;
    [SerializeField] private bool addCheckoutArea = true;
    [SerializeField] private bool addCeiling = false;
    [SerializeField] private int randomSeed = 25;

    private List<GameObject> productOptions = new List<GameObject>();

    [MenuItem("Tools/Aisle Be Back/Store Generator")]
    public static void Open()
    {
        var window = GetWindow<AisleBeBackStoreGenerator>("Store Generator");
        window.minSize = new Vector2(370, 580);
        window.TryAutoAssignAssets();
    }

    private void OnEnable()
    {
        TryAutoAssignAssets();
    }

    private void OnGUI()
    {
        EditorGUILayout.HelpBox(
            "Builds a blockout supermarket using the ModernSupermarket prefab library. " +
            "Generate again to replace only the previous generated store. Existing scene objects are left alone.",
            MessageType.Info);

        EditorGUILayout.LabelField("Store Dimensions", EditorStyles.boldLabel);
        aisleRows = Mathf.Clamp(EditorGUILayout.IntField("Aisle Rows", aisleRows), 1, 12);
        shelfUnitsPerRow = Mathf.Clamp(EditorGUILayout.IntField("Shelf Units Per Row", shelfUnitsPerRow), 1, 20);
        shelfSpacing = Mathf.Max(0.5f, EditorGUILayout.FloatField("Shelf Unit Spacing", shelfSpacing));
        aisleSpacing = Mathf.Max(2f, EditorGUILayout.FloatField("Aisle Spacing", aisleSpacing));
        storeWidth = Mathf.Max(15f, EditorGUILayout.FloatField("Store Width", storeWidth));
        storeLength = Mathf.Max(20f, EditorGUILayout.FloatField("Store Length", storeLength));
        wallHeight = Mathf.Max(2f, EditorGUILayout.FloatField("Wall Height", wallHeight));
        wallThickness = Mathf.Max(0.1f, EditorGUILayout.FloatField("Wall Thickness", wallThickness));

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Prefab Assignments", EditorStyles.boldLabel);
        floorPrefab = (GameObject)EditorGUILayout.ObjectField("Floor", floorPrefab, typeof(GameObject), false);
        shelfPrefab = (GameObject)EditorGUILayout.ObjectField("Shelf / Rack", shelfPrefab, typeof(GameObject), false);
        checkoutPrefab = (GameObject)EditorGUILayout.ObjectField("Checkout", checkoutPrefab, typeof(GameObject), false);
        productPrefab = (GameObject)EditorGUILayout.ObjectField("Product", productPrefab, typeof(GameObject), false);
        producePrefab = (GameObject)EditorGUILayout.ObjectField("Produce", producePrefab, typeof(GameObject), false);
        signPrefab = (GameObject)EditorGUILayout.ObjectField("Sign", signPrefab, typeof(GameObject), false);

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Generation Options", EditorStyles.boldLabel);
        addProducts = EditorGUILayout.Toggle("Place Products", addProducts);
        productsPerShelfUnit = Mathf.Clamp(EditorGUILayout.IntField("Products Per Shelf Unit", productsPerShelfUnit), 0, 8);
        addCheckoutArea = EditorGUILayout.Toggle("Add Checkout Area", addCheckoutArea);
        addCeiling = EditorGUILayout.Toggle("Add Ceiling", addCeiling);
        randomSeed = EditorGUILayout.IntField("Random Seed", randomSeed);

        EditorGUILayout.Space(10);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Auto-Find Assets", GUILayout.Height(30)))
                TryAutoAssignAssets();
            if (GUILayout.Button("Generate / Replace Store", GUILayout.Height(30)))
                GenerateStore();
        }

        EditorGUILayout.HelpBox(
            "Tip: inspect the selected prefab's scale and pivot if pieces overlap or sit too high/low. " +
            "The generated layout uses Unity units and places the store around the scene origin.",
            MessageType.None);
    }

    private void TryAutoAssignAssets()
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { AssetRoot });
        var prefabs = guids.Select(g => AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(g)))
            .Where(p => p != null).ToList();

        if (prefabs.Count == 0)
            return;

        if (floorPrefab == null)
            floorPrefab = FindByName(prefabs, "floor");
        if (shelfPrefab == null)
            shelfPrefab = FindByName(prefabs, "shelf", "rack", "gondola");
        if (checkoutPrefab == null)
            checkoutPrefab = FindByName(prefabs, "checkout", "cashier", "cash_register", "register");
        if (productPrefab == null)
            productPrefab = FindByName(prefabs, "product", "box", "can", "bottle");
        if (producePrefab == null)
            producePrefab = FindByName(prefabs, "produce", "fruit", "vegetable");
        if (signPrefab == null)
            signPrefab = FindByName(prefabs, "sign");

        productOptions = prefabs.Where(p =>
        {
            string n = p.name.ToLowerInvariant();
            return n.Contains("product") || n.Contains("fruit") || n.Contains("vegetable") ||
                   n.Contains("can") || n.Contains("bottle") || n.Contains("box") || n.Contains("produce");
        }).ToList();

        Repaint();
    }

    private static GameObject FindByName(List<GameObject> prefabs, params string[] keywords)
    {
        foreach (string keyword in keywords)
        {
            var match = prefabs.FirstOrDefault(p => p.name.ToLowerInvariant().Contains(keyword));
            if (match != null) return match;
        }
        return null;
    }

    private void GenerateStore()
    {
        if (shelfPrefab == null)
        {
            EditorUtility.DisplayDialog("Shelf prefab needed",
                "Auto-Find could not identify a shelf/rack prefab. Drag one from Assets/ModernSupermarket/Prefabs into the Shelf / Rack field, then generate again.",
                "OK");
            return;
        }

        var oldRoot = GameObject.Find(GeneratedRootName);
        if (oldRoot != null && !EditorUtility.DisplayDialog("Replace generated store?",
            "This deletes and rebuilds only '" + GeneratedRootName + "'. Other scene objects will not be touched.",
            "Replace", "Cancel"))
            return;

        if (oldRoot != null)
            Undo.DestroyObjectImmediate(oldRoot);

        Random.InitState(randomSeed);
        var root = new GameObject(GeneratedRootName);
        Undo.RegisterCreatedObjectUndo(root, "Generate Aisle Be Back Store");

        Transform architecture = CreateGroup(root.transform, "Architecture");
        Transform shelves = CreateGroup(root.transform, "Aisle Shelving");
        Transform products = CreateGroup(root.transform, "Products");
        Transform checkout = CreateGroup(root.transform, "Checkout Area");
        Transform signs = CreateGroup(root.transform, "Signs");

        CreateFloor(architecture);
        CreateWalls(architecture);

        float totalRowsWidth = (aisleRows - 1) * aisleSpacing;
        float startX = -totalRowsWidth / 2f;
        float rowLength = Mathf.Min((shelfUnitsPerRow - 1) * shelfSpacing + 2.5f, storeLength - 10f);
        float startZ = -rowLength / 2f + 3f;

        for (int row = 0; row < aisleRows; row++)
        {
            float x = startX + row * aisleSpacing;
            for (int unit = 0; unit < shelfUnitsPerRow; unit++)
            {
                float z = startZ + unit * shelfSpacing;
                // Alternate row orientation to create clear aisles and visual variety.
                float yaw = row % 2 == 0 ? 0f : 180f;
                var shelf = InstantiatePrefab(shelfPrefab, new Vector3(x, 0f, z), Quaternion.Euler(0f, yaw, 0f), shelves, "Aisle " + (row + 1) + " Shelf " + (unit + 1));
                if (shelf != null && addProducts)
                    PlaceProductsOnShelf(shelf.transform, products, row, unit);
            }
        }

        if (addCheckoutArea)
            CreateCheckout(checkout, signs, root.transform);

        if (addCeiling)
            CreateCeiling(architecture);

        Selection.activeGameObject = root.gameObject;
        EditorSceneManagerMarkDirty();
        Debug.Log("Aisle Be Back store generated. Rows: " + aisleRows + ", shelf units: " + (aisleRows * shelfUnitsPerRow) + ".");
    }

    private void CreateFloor(Transform parent)
    {
        if (floorPrefab != null)
        {
            var floor = InstantiatePrefab(floorPrefab, Vector3.zero, Quaternion.identity, parent, "Store Floor");
            if (floor != null)
            {
                var bounds = GetCombinedBounds(floor);
                if (bounds.size.x > 0.01f && bounds.size.z > 0.01f)
                    floor.transform.localScale = Vector3.Scale(floor.transform.localScale,
                        new Vector3(storeWidth / bounds.size.x, 1f, storeLength / bounds.size.z));
            }
        }
        else
        {
            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Store Floor (generated)";
            floor.transform.SetParent(parent);
            floor.transform.position = new Vector3(0f, -0.25f, 0f);
            floor.transform.localScale = new Vector3(storeWidth, 0.5f, storeLength);
            Undo.RegisterCreatedObjectUndo(floor, "Create Store Floor");
        }
    }

    private void CreateWalls(Transform parent)
    {
        MakeCube(parent, "Back Wall", new Vector3(0f, wallHeight / 2f, storeLength / 2f), new Vector3(storeWidth, wallHeight, wallThickness));
        MakeCube(parent, "Left Wall", new Vector3(-storeWidth / 2f, wallHeight / 2f, 0f), new Vector3(wallThickness, wallHeight, storeLength));
        MakeCube(parent, "Right Wall", new Vector3(storeWidth / 2f, wallHeight / 2f, 0f), new Vector3(wallThickness, wallHeight, storeLength));
        // Front wall is split to leave an entrance in the center.
        float sideWidth = Mathf.Max(2f, (storeWidth - 8f) / 2f);
        MakeCube(parent, "Front Wall Left", new Vector3(-(4f + sideWidth / 2f), wallHeight / 2f, -storeLength / 2f), new Vector3(sideWidth, wallHeight, wallThickness));
        MakeCube(parent, "Front Wall Right", new Vector3(4f + sideWidth / 2f, wallHeight / 2f, -storeLength / 2f), new Vector3(sideWidth, wallHeight, wallThickness));
    }

    private void CreateCheckout(Transform parent, Transform signs, Transform root)
    {
        float z = -storeLength / 2f + 7f;
        int count = 3;
        for (int i = 0; i < count; i++)
        {
            float x = (i - (count - 1) / 2f) * 4f;
            if (checkoutPrefab != null)
                InstantiatePrefab(checkoutPrefab, new Vector3(x, 0f, z), Quaternion.identity, parent, "Checkout " + (i + 1));
            else
                MakeCube(parent, "Checkout Counter " + (i + 1), new Vector3(x, 0.6f, z), new Vector3(2.5f, 1.2f, 1.2f));

            if (signPrefab != null)
                InstantiatePrefab(signPrefab, new Vector3(x, 2.2f, z + 0.5f), Quaternion.identity, signs, "Checkout Sign " + (i + 1));
        }
    }

    private void CreateCeiling(Transform parent)
    {
        MakeCube(parent, "Ceiling", new Vector3(0f, wallHeight + 0.2f, 0f), new Vector3(storeWidth, 0.4f, storeLength));
    }

    private void PlaceProductsOnShelf(Transform shelf, Transform parent, int row, int unit)
    {
        GameObject chosen = null;
        if (productOptions.Count > 0)
            chosen = productOptions[Random.Range(0, productOptions.Count)];
        if (chosen == null) chosen = producePrefab != null ? producePrefab : productPrefab;
        if (chosen == null) return;

        Bounds bounds = GetCombinedBounds(shelf.gameObject);
        float width = Mathf.Max(0.8f, bounds.size.x);
        float depth = Mathf.Max(0.4f, bounds.size.z);
        float height = Mathf.Max(1f, bounds.size.y);
        int count = Mathf.Max(0, productsPerShelfUnit);
        for (int i = 0; i < count; i++)
        {
            float px = Random.Range(-width * 0.28f, width * 0.28f);
            float pz = Random.Range(-depth * 0.25f, depth * 0.25f);
            float py = height * Random.Range(0.25f, 0.85f);
            var item = InstantiatePrefab(chosen, shelf.transform.TransformPoint(new Vector3(px, py, pz)),
                Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), parent,
                "Product R" + (row + 1) + " S" + (unit + 1) + " " + (i + 1));
            if (item != null)
                item.transform.localScale *= Random.Range(0.85f, 1.15f);
        }
    }

    private static Transform CreateGroup(Transform parent, string name)
    {
        var group = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(group, "Create " + name);
        group.transform.SetParent(parent, false);
        return group.transform;
    }

    private static GameObject MakeCube(Transform parent, string name, Vector3 position, Vector3 scale)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.SetParent(parent, true);
        cube.transform.position = position;
        cube.transform.localScale = scale;
        Undo.RegisterCreatedObjectUndo(cube, "Create " + name);
        return cube;
    }

    private static GameObject InstantiatePrefab(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent, string objectName)
    {
        if (prefab == null) return null;
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        if (instance == null) instance = Instantiate(prefab);
        Undo.RegisterCreatedObjectUndo(instance, "Place " + objectName);
        instance.name = objectName;
        instance.transform.SetParent(parent, true);
        instance.transform.SetPositionAndRotation(position, rotation);
        return instance;
    }

    private static Bounds GetCombinedBounds(GameObject target)
    {
        var renderers = target.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(target.transform.position, Vector3.one);
        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);
        return bounds;
    }

    private static void EditorSceneManagerMarkDirty()
    {
        if (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().IsValid())
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }
}