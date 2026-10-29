using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering.Universal;
using System.IO;

public class BossSetupUtility : EditorWindow
{
    private const string SQUARE_SPRITE_GUID = "311925a002f4447b3a28927169b83ea6";
    private const string CIRCLE_SPRITE_GUID = "a86470a33a6bf42c4b3595704624658b";
    private const string URP_LIT_MAT_GUID = "a97c105638bdf8b4a8650670310a4cd3";

    [MenuItem("Raion/Generate All Boss Assets & Test Scene")]
    public static void GenerateAll()
    {
        Debug.Log("[BossSetup] Memulai pembuatan aset Boss...");

        Sprite squareSprite = LoadAssetByGuid<Sprite>(SQUARE_SPRITE_GUID);
        Sprite circleSprite = LoadAssetByGuid<Sprite>(CIRCLE_SPRITE_GUID);
        Material defaultMat = LoadAssetByGuid<Material>(URP_LIT_MAT_GUID);

        if (squareSprite == null)
        {
            squareSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        }

        // 1. Generate Giant Coin Prefab
        GameObject giantCoinPrefab = CreateGiantCoinPrefab(circleSprite, defaultMat);

        // 2. Generate Boss Impact Zone Prefab
        GameObject impactZonePrefab = CreateImpactZonePrefab(circleSprite, defaultMat);

        // 3. Generate Boss Projectile Prefab
        GameObject projectilePrefab = CreateProjectilePrefab(circleSprite, squareSprite, defaultMat);

        // 4. Generate Bulky Hulk Boss Prefab
        GameObject bossPrefab = CreateBulkyBossPrefab(squareSprite, defaultMat, giantCoinPrefab, impactZonePrefab, projectilePrefab);

        // 5. Generate Test Scene
        CreateBossTestScene(bossPrefab);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        if (!Application.isBatchMode) EditorUtility.DisplayDialog("Sukses!", "Semua aset Boss (GiantCoin, ImpactZone, Projectile, Boss Prefab) dan Scene Boss_Test berhasil dibuat!\n\nBuka Assets/Scenes/Boss_Test.unity untuk langsung menguji pertarungan.", "OK");
    }

    private static T LoadAssetByGuid<T>(string guid) where T : UnityEngine.Object
    {
        string path = AssetDatabase.GUIDToAssetPath(guid);
        if (!string.IsNullOrEmpty(path))
        {
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
        return null;
    }

    private static GameObject CreateGiantCoinPrefab(Sprite circleSprite, Material mat)
    {
        GameObject root = new GameObject("GiantGoldCoin");
        root.tag = "Untagged";
        CircleCollider2D col = root.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.5f;

        Coin coinScript = root.AddComponent<Coin>();
        coinScript.coinValue = 25; // Koin bernilai tinggi dari Boss!

        root.AddComponent<GiantCoinEffect>();

        GameObject spriteContainer = new GameObject("Sprite");
        spriteContainer.transform.SetParent(root.transform);
        spriteContainer.transform.localPosition = Vector3.zero;

        // Outer Gold Circle
        GameObject outer = new GameObject("Outer");
        outer.transform.SetParent(spriteContainer.transform);
        outer.transform.localPosition = Vector3.zero;
        outer.transform.localScale = Vector3.one * 2.5f;
        SpriteRenderer rendOuter = outer.AddComponent<SpriteRenderer>();
        rendOuter.sprite = circleSprite;
        rendOuter.material = mat;
        rendOuter.color = new Color(1f, 0.86f, 0.05f, 1f);
        rendOuter.sortingOrder = -2;

        // Inner Dark Gold Circle
        GameObject inner = new GameObject("Inner");
        inner.transform.SetParent(spriteContainer.transform);
        inner.transform.localPosition = Vector3.zero;
        inner.transform.localScale = Vector3.one * 1.8f;
        SpriteRenderer rendInner = inner.AddComponent<SpriteRenderer>();
        rendInner.sprite = circleSprite;
        rendInner.material = mat;
        rendInner.color = new Color(0.88f, 0.71f, 0.20f, 1f);
        rendInner.sortingOrder = -1;

        string path = "Assets/Prefabs/GiantGoldCoin.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        Debug.Log("[BossSetup] GiantGoldCoin prefab dibuat di " + path);
        return prefab;
    }

    private static GameObject CreateImpactZonePrefab(Sprite circleSprite, Material mat)
    {
        GameObject root = new GameObject("BossImpactZone");
        SpriteRenderer rend = root.AddComponent<SpriteRenderer>();
        rend.sprite = circleSprite;
        rend.material = mat;
        rend.color = new Color(1f, 0.15f, 0.15f, 0.35f);
        rend.sortingOrder = -3;

        BossImpactZone zone = root.AddComponent<BossImpactZone>();
        zone.impactRadius = 1.6f;
        zone.windupDuration = 2.0f;

        string path = "Assets/Prefabs/BossImpactZone.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        Debug.Log("[BossSetup] BossImpactZone prefab dibuat di " + path);
        return prefab;
    }

    private static GameObject CreateProjectilePrefab(Sprite circleSprite, Sprite squareSprite, Material mat)
    {
        GameObject root = new GameObject("BossProjectile");
        root.tag = "Untagged";

        CircleCollider2D col = root.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.28f;

        BossProjectile proj = root.AddComponent<BossProjectile>();
        proj.damage = 1;
        proj.speed = 7.0f;
        proj.lifeTime = 4.5f;

        // Visual Bullet (Red Energy Core + Outer Glow)
        GameObject core = new GameObject("Core");
        core.transform.SetParent(root.transform);
        core.transform.localPosition = Vector3.zero;
        core.transform.localScale = Vector3.one * 0.45f;
        SpriteRenderer coreRend = core.AddComponent<SpriteRenderer>();
        coreRend.sprite = circleSprite != null ? circleSprite : squareSprite;
        coreRend.material = mat;
        coreRend.color = new Color(1f, 0.2f, 0.2f, 1f);
        coreRend.sortingOrder = 5;

        GameObject glow = new GameObject("Glow");
        glow.transform.SetParent(root.transform);
        glow.transform.localPosition = Vector3.zero;
        glow.transform.localScale = Vector3.one * 0.65f;
        SpriteRenderer glowRend = glow.AddComponent<SpriteRenderer>();
        glowRend.sprite = circleSprite != null ? circleSprite : squareSprite;
        glowRend.material = mat;
        glowRend.color = new Color(1f, 0.5f, 0.2f, 0.45f);
        glowRend.sortingOrder = 4;

        string path = "Assets/Prefabs/BossProjectile.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        Debug.Log("[BossSetup] BossProjectile prefab dibuat di " + path);
        return prefab;
    }

    private static GameObject CreateBulkyBossPrefab(Sprite squareSprite, Material mat, GameObject coinPrefab, GameObject zonePrefab, GameObject projPrefab)
    {
        GameObject root = new GameObject("Boss");
        root.tag = "Enemy";

        BoxCollider2D col = root.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(2.4f, 2.3f);

        Rigidbody2D rb = root.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        Health health = root.AddComponent<Health>();
        health.maxHealth = 60;
        health.Coin = coinPrefab;

        BossController controller = root.AddComponent<BossController>();
        controller.moveSpeed = 1.2f;
        controller.dashSpeed = 16f;
        controller.dashCooldown = 9f;
        controller.windupDuration = 2.0f;
        controller.contactDamage = 3;
        controller.projectilePrefab = projPrefab;
        controller.impactZonePrefab = zonePrefab;

        // Fire Point
        GameObject firePoint = new GameObject("FirePoint");
        firePoint.transform.SetParent(root.transform);
        firePoint.transform.localPosition = new Vector3(0f, 0.35f, 0f);
        controller.firePoint = firePoint.transform;

        // Visual Container
        GameObject visualContainer = new GameObject("Sprite");
        visualContainer.transform.SetParent(root.transform);
        visualContainer.transform.localPosition = Vector3.zero;
        controller.visualContainer = visualContainer.transform;

        // Warna Acuan Pixel Monster Rubah
        Color colHeadOrange   = new Color(0.9245f, 0.4557f, 0.1448f, 1f);
        Color colBodyNavy     = new Color(0.1823f, 0.1789f, 0.2528f, 1f);
        Color colLegsNavy     = new Color(0.1594f, 0.1594f, 0.1925f, 1f);
        Color colArmsNavy     = new Color(0.1626f, 0.1626f, 0.2000f, 1f);
        Color colArmorMetal   = new Color(0.25f, 0.25f, 0.28f, 1f);
        Color colWhiteMuzzle  = Color.white;
        Color colRedEyes      = new Color(1f, 0.08f, 0.08f, 1f);
        Color colEyeSclera    = Color.white;
        Color colNoseGray     = new Color(0.434f, 0.363f, 0.363f, 1f);
        Color colEarInner     = new Color(0.5472f, 0.2739f, 0.0939f, 1f);
        Color colBadgeCyan    = new Color(0.1294f, 0.6012f, 0.9608f, 1f);
        Color colBadgeOrange  = new Color(0.9623f, 0.4979f, 0.1289f, 1f);

        // 1. Kaki Kokoh (Stance Lebar)
        AddSquareBlock(visualContainer, "Leg_L", new Vector3(-0.46f, -0.66f, 0), new Vector3(0.68f, 0.42f, 1), colLegsNavy, 0, squareSprite, mat);
        AddSquareBlock(visualContainer, "Leg_R", new Vector3(0.46f, -0.66f, 0), new Vector3(0.68f, 0.42f, 1), colLegsNavy, 0, squareSprite, mat);

        // 2. Dada Bidang & Perut Tebal (Hulk Torso)
        AddSquareBlock(visualContainer, "Torso_Chest", new Vector3(0f, 0.05f, 0), new Vector3(1.85f, 1.15f, 1), colBodyNavy, 1, squareSprite, mat);

        // 3. Otot Punggung / Trapezius Bahu Hulk
        AddSquareBlock(visualContainer, "Trap_L", new Vector3(-0.68f, 0.62f, 0), new Vector3(0.75f, 0.45f, 1), colBodyNavy, 2, squareSprite, mat);
        AddSquareBlock(visualContainer, "Trap_R", new Vector3(0.68f, 0.62f, 0), new Vector3(0.75f, 0.45f, 1), colBodyNavy, 2, squareSprite, mat);

        // 4. Lengan Biceps Raksasa
        AddSquareBlock(visualContainer, "Bicep_L", new Vector3(-1.18f, 0.22f, 0), new Vector3(0.68f, 0.75f, 1), colArmsNavy, 1, squareSprite, mat);
        AddSquareBlock(visualContainer, "Bicep_R", new Vector3(1.18f, 0.22f, 0), new Vector3(0.68f, 0.75f, 1), colArmsNavy, 1, squareSprite, mat);

        // 5. Lengan Bawah / Kepalan Tangan Tebal
        AddSquareBlock(visualContainer, "Forearm_L", new Vector3(-1.28f, -0.42f, 0), new Vector3(0.62f, 0.72f, 1), colArmsNavy, 2, squareSprite, mat);
        AddSquareBlock(visualContainer, "Forearm_R", new Vector3(1.28f, -0.42f, 0), new Vector3(0.62f, 0.72f, 1), colArmsNavy, 2, squareSprite, mat);

        // Aksesori: Gelang Armor Besi Gelap pada Pergelangan
        AddSquareBlock(visualContainer, "ArmorBand_L", new Vector3(-1.28f, -0.22f, 0), new Vector3(0.70f, 0.16f, 1), colArmorMetal, 3, squareSprite, mat);
        AddSquareBlock(visualContainer, "ArmorBand_R", new Vector3(1.28f, -0.22f, 0), new Vector3(0.70f, 0.16f, 1), colArmorMetal, 3, squareSprite, mat);

        // 6. Kepala Berotot yang Menunduk/Menyatu ke Bahu
        AddSquareBlock(visualContainer, "Head", new Vector3(0f, 0.75f, 0), new Vector3(1.20f, 0.95f, 1), colHeadOrange, 2, squareSprite, mat);

        // 7. Dagu / Janggut Putih Lebar
        AddSquareBlock(visualContainer, "Muzzle_Beard", new Vector3(0f, 0.46f, 0), new Vector3(1.42f, 0.38f, 1), colWhiteMuzzle, 3, squareSprite, mat);

        // 8. Hidung Hitam/Abu-abu
        AddSquareBlock(visualContainer, "Nose", new Vector3(0f, 0.64f, 0), new Vector3(0.22f, 0.15f, 1), colNoseGray, 4, squareSprite, mat);

        // 9. Mata Merah Menyala yang Garang (Glowing Red Pupils)
        AddSquareBlock(visualContainer, "EyeWhite_L", new Vector3(-0.34f, 0.78f, 0), new Vector3(0.32f, 0.18f, 1), colEyeSclera, 3, squareSprite, mat);
        AddSquareBlock(visualContainer, "EyeWhite_R", new Vector3(0.34f, 0.78f, 0), new Vector3(0.32f, 0.18f, 1), colEyeSclera, 3, squareSprite, mat);
        AddSquareBlock(visualContainer, "EyeRed_L", new Vector3(-0.34f, 0.78f, 0), new Vector3(0.18f, 0.18f, 1), colRedEyes, 4, squareSprite, mat);
        AddSquareBlock(visualContainer, "EyeRed_R", new Vector3(0.34f, 0.78f, 0), new Vector3(0.18f, 0.18f, 1), colRedEyes, 4, squareSprite, mat);

        // 10. Telinga Kokoh
        AddSquareBlock(visualContainer, "EarOuter_L", new Vector3(-0.42f, 1.28f, 0), new Vector3(0.38f, 0.40f, 1), colHeadOrange, 1, squareSprite, mat);
        AddSquareBlock(visualContainer, "EarInner_L", new Vector3(-0.42f, 1.28f, 0), new Vector3(0.22f, 0.28f, 1), colEarInner, 2, squareSprite, mat);
        AddSquareBlock(visualContainer, "EarOuter_R", new Vector3(0.42f, 1.28f, 0), new Vector3(0.38f, 0.40f, 1), colHeadOrange, 1, squareSprite, mat);
        AddSquareBlock(visualContainer, "EarInner_R", new Vector3(0.42f, 1.28f, 0), new Vector3(0.22f, 0.28f, 1), colEarInner, 2, squareSprite, mat);

        // 11. Pin/Badge di Dada (Identitas Tim)
        AddSquareBlock(visualContainer, "Badge_Cyan", new Vector3(0.48f, 0.22f, 0), new Vector3(0.12f, 0.07f, 1), colBadgeCyan, 3, squareSprite, mat);
        AddSquareBlock(visualContainer, "Badge_Orange", new Vector3(0.48f, 0.28f, 0), new Vector3(0.12f, 0.07f, 1), colBadgeOrange, 3, squareSprite, mat);

        string path = "Assets/Prefabs/Boss.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(root, path);
        DestroyImmediate(root);
        Debug.Log("[BossSetup] Boss prefab dibuat di " + path);
        return prefab;
    }

    private static void AddSquareBlock(GameObject parent, string name, Vector3 localPos, Vector3 localScale, Color color, int sortingOrder, Sprite sprite, Material mat)
    {
        GameObject block = new GameObject(name);
        block.transform.SetParent(parent.transform);
        block.transform.localPosition = localPos;
        block.transform.localScale = localScale;

        SpriteRenderer rend = block.AddComponent<SpriteRenderer>();
        rend.sprite = sprite;
        rend.material = mat;
        rend.color = color;
        rend.sortingOrder = sortingOrder;
    }

    private static void CreateBossTestScene(GameObject bossPrefab)
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // 1. Main Camera
        GameObject camObj = new GameObject("Main Camera");
        camObj.tag = "MainCamera";
        Camera cam = camObj.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        camObj.transform.position = new Vector3(0, 0, -10f);

        // 2. Global Light 2D
        GameObject lightObj = new GameObject("Global Light 2D");
        Light2D light = lightObj.AddComponent<Light2D>();
        light.lightType = Light2D.LightType.Global;
        light.color = Color.white;
        light.intensity = 1.0f;

        // 3. LevelManager
        GameObject lmObj = new GameObject("LevelManager");
        LevelManager lm = lmObj.AddComponent<LevelManager>();

        // 4. Player Instance
        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player.prefab");
        if (playerPrefab != null)
        {
            GameObject player = PrefabUtility.InstantiatePrefab(playerPrefab) as GameObject;
            player.name = "Player";
            player.tag = "Player";
            player.transform.position = new Vector3(-3.5f, 0, 0);

            // Tingkatkan HP Player menjadi 20 sesuai kebutuhan pengujian
            Health pHealth = player.GetComponent<Health>();
            if (pHealth != null)
            {
                pHealth.maxHealth = 20;
            }
        }

        // 5. Boss Instance
        if (bossPrefab != null)
        {
            GameObject boss = PrefabUtility.InstantiatePrefab(bossPrefab) as GameObject;
            boss.name = "Boss";
            boss.transform.position = new Vector3(3.5f, 0, 0);
        }

        string scenePath = "Assets/Scenes/Boss_Test.unity";
        EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log("[BossSetup] Test Scene dibuat di " + scenePath);
    }
}
