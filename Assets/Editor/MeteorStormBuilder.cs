using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MeteorStormBuilder
{
    private const string ATLAS_PATH = "Assets/nave tiro e cometa.png";
    private const string DEEP_SPACE_PATH = "Assets/pixil-frame-0 (1) 2.png";
    private const string NEBULA_PATH = "Assets/pixil-frame-0.png";
    private const string STARS_PATH = "Assets/pixil-frame-0 (1) 3.png";

    [MenuItem("Meteor Storm/Build Game Scene & Prefabs")]
    public static void BuildGame()
    {
        Debug.Log("===> Iniciando montagem do Meteor Storm...");

        EnsureDirectories();

        // 1. Criar Efeitos de Particulas
        GameObject meteorExplosionPrefab = CreateMeteorExplosionEffect();
        GameObject playerExplosionPrefab = CreatePlayerExplosionEffect();
        GameObject hitSparkPrefab = CreateHitSparkEffect();
        GameObject powerUpPickupPrefab = CreatePowerUpPickupEffect();

        // 2. Criar Prefabs de Jogo
        GameObject projectilePrefab = CreateProjectilePrefab(hitSparkPrefab);
        GameObject powerUpPrefab = CreatePowerUpPrefab(powerUpPickupPrefab);
        GameObject meteorSmallPrefab = CreateMeteorPrefab(MeteorSize.Small, "nave tiro e cometa_10", 0.7f, 1, 50, meteorExplosionPrefab, powerUpPrefab);
        GameObject meteorMediumPrefab = CreateMeteorPrefab(MeteorSize.Medium, "nave tiro e cometa_11", 1.05f, 3, 100, meteorExplosionPrefab, powerUpPrefab);
        GameObject meteorLargePrefab = CreateMeteorPrefab(MeteorSize.Large, "nave tiro e cometa_12", 1.55f, 6, 200, meteorExplosionPrefab, powerUpPrefab);
        GameObject playerPrefab = CreatePlayerPrefab(projectilePrefab, playerExplosionPrefab);

        // 3. Montar a Cena jogo.unity
        SetupGameScene(meteorSmallPrefab, meteorMediumPrefab, meteorLargePrefab, playerPrefab);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("===> Montagem do Meteor Storm concluida com sucesso!");
    }

    private static void EnsureDirectories()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
    }

    private static Sprite GetSpriteFromAtlas(string spriteName)
    {
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(ATLAS_PATH);
        foreach (Object obj in assets)
        {
            if (obj is Sprite s && s.name == spriteName)
            {
                return s;
            }
        }
        Debug.LogWarning($"Sprite nao encontrado no atlas: {spriteName}");
        return null;
    }

    private static Sprite GetStandaloneSprite(string path)
    {
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(path);
        foreach (Object obj in assets)
        {
            if (obj is Sprite s) return s;
        }
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    // -------------------------------------------------------------------------
    // SISTEMAS DE PARTICULAS
    // -------------------------------------------------------------------------
    private static GameObject CreateMeteorExplosionEffect()
    {
        GameObject go = new GameObject("MeteorExplosionEffect");
        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.duration = 0.45f;
        main.loop = false;
        main.startLifetime = 0.4f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(2.5f, 6.5f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.15f, 0.4f);
        main.startColor = new ParticleSystem.MinMaxGradient(new Color(0.85f, 0.85f, 0.85f, 1f), new Color(0.4f, 0.4f, 0.4f, 1f));
        main.stopAction = ParticleSystemStopAction.Destroy;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 28) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.25f;

        var col = ps.colorOverLifetime;
        col.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.gray, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        col.color = grad;

        string path = "Assets/Prefabs/MeteorExplosionEffect.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return prefab;
    }

    private static GameObject CreatePlayerExplosionEffect()
    {
        GameObject go = new GameObject("PlayerExplosionEffect");
        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.duration = 0.8f;
        main.loop = false;
        main.startLifetime = 0.65f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(3.5f, 9.0f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.6f);
        main.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 0.7f, 0.1f, 1f), new Color(1f, 0.2f, 0.05f, 1f));
        main.stopAction = ParticleSystemStopAction.Destroy;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 50) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.35f;

        var col = ps.colorOverLifetime;
        col.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.yellow, 0f), new GradientColorKey(Color.red, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
        );
        col.color = grad;

        string path = "Assets/Prefabs/PlayerExplosionEffect.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return prefab;
    }

    private static GameObject CreateHitSparkEffect()
    {
        GameObject go = new GameObject("HitSparkEffect");
        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.duration = 0.2f;
        main.loop = false;
        main.startLifetime = 0.18f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(4.0f, 7.5f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.08f, 0.18f);
        main.startColor = new ParticleSystem.MinMaxGradient(new Color(0.3f, 0.8f, 1f, 1f), new Color(1f, 1f, 1f, 1f));
        main.stopAction = ParticleSystemStopAction.Destroy;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 14) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.1f;

        string path = "Assets/Prefabs/HitSparkEffect.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return prefab;
    }

    private static GameObject CreatePowerUpPickupEffect()
    {
        GameObject go = new GameObject("PowerUpPickupEffect");
        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.duration = 0.35f;
        main.loop = false;
        main.startLifetime = 0.3f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(2.5f, 5.0f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.12f, 0.28f);
        main.startColor = new ParticleSystem.MinMaxGradient(new Color(0.2f, 1f, 0.9f, 1f), new Color(1f, 0.9f, 0.2f, 1f));
        main.stopAction = ParticleSystemStopAction.Destroy;

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 22) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.2f;

        string path = "Assets/Prefabs/PowerUpPickupEffect.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return prefab;
    }

    // -------------------------------------------------------------------------
    // PREFABS DE GAMEPLAY
    // -------------------------------------------------------------------------
    private static GameObject CreateProjectilePrefab(GameObject hitSpark)
    {
        GameObject go = new GameObject("LaserProjectile");
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = GetSpriteFromAtlas("nave tiro e cometa_7");
        sr.sortingOrder = 3;

        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(0.25f, 0.95f);

        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        Projectile proj = go.AddComponent<Projectile>();
        SerializedObject so = new SerializedObject(proj);
        so.FindProperty("projectileSpeed").floatValue = 16f;
        so.FindProperty("projectileDamage").intValue = 1;
        so.FindProperty("despawnY").floatValue = 6.5f;
        so.FindProperty("impactPrefab").objectReferenceValue = hitSpark;
        so.ApplyModifiedPropertiesWithoutUndo();

        string path = "Assets/Prefabs/LaserProjectile.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return prefab;
    }

    private static GameObject CreatePowerUpPrefab(GameObject pickupEffect)
    {
        GameObject go = new GameObject("PowerUpItem");
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = GetSpriteFromAtlas("nave tiro e cometa_16") ?? GetSpriteFromAtlas("nave tiro e cometa_3");
        sr.sortingOrder = 3;
        go.transform.localScale = new Vector3(2.5f, 2.5f, 1f);

        CircleCollider2D col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.35f;

        PowerUp pu = go.AddComponent<PowerUp>();
        SerializedObject so = new SerializedObject(pu);
        so.FindProperty("pickupEffectPrefab").objectReferenceValue = pickupEffect;
        so.FindProperty("spriteRenderer").objectReferenceValue = sr;
        so.ApplyModifiedPropertiesWithoutUndo();

        string path = "Assets/Prefabs/PowerUpItem.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return prefab;
    }

    private static GameObject CreateMeteorPrefab(
        MeteorSize size,
        string spriteName,
        float scale,
        int health,
        int score,
        GameObject explosionPrefab,
        GameObject powerUpPrefab)
    {
        string name = $"Meteor_{size}";
        GameObject go = new GameObject(name);
        go.transform.localScale = new Vector3(scale, scale, 1f);

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = GetSpriteFromAtlas(spriteName);
        sr.sortingOrder = 2;

        CircleCollider2D col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.55f;

        Meteor meteor = go.AddComponent<Meteor>();
        SerializedObject so = new SerializedObject(meteor);
        so.FindProperty("meteorSize").enumValueIndex = (int)size;
        so.FindProperty("health").intValue = health;
        so.FindProperty("scoreValue").intValue = score;
        so.FindProperty("explosionPrefab").objectReferenceValue = explosionPrefab;
        so.FindProperty("powerUpPrefab").objectReferenceValue = powerUpPrefab;
        so.FindProperty("spriteRenderer").objectReferenceValue = sr;
        so.ApplyModifiedPropertiesWithoutUndo();

        string path = $"Assets/Prefabs/{name}.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return prefab;
    }

    private static GameObject CreatePlayerPrefab(GameObject projectilePrefab, GameObject explosionPrefab)
    {
        GameObject go = new GameObject("Player");
        go.tag = "player";

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = GetSpriteFromAtlas("nave tiro e cometa_0");
        sr.sortingOrder = 3;

        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        BoxCollider2D col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(0.9f, 1.25f);

        // Ponto de tiro
        GameObject gun = new GameObject("playergun");
        gun.transform.SetParent(go.transform);
        gun.transform.localPosition = new Vector3(0f, 0.75f, 0f);

        // Visual do Escudo
        GameObject shieldObj = new GameObject("ShieldVisual");
        shieldObj.transform.SetParent(go.transform);
        shieldObj.transform.localPosition = Vector3.zero;
        shieldObj.transform.localScale = new Vector3(2.0f, 2.0f, 1f);
        SpriteRenderer shieldSr = shieldObj.AddComponent<SpriteRenderer>();
        shieldSr.sprite = GetSpriteFromAtlas("nave tiro e cometa_16") ?? GetSpriteFromAtlas("nave tiro e cometa_3");
        shieldSr.color = new Color(0.2f, 0.7f, 1f, 0.6f);
        shieldSr.sortingOrder = 4;
        shieldObj.SetActive(false);

        // Particulas do Motor
        GameObject engineObj = new GameObject("EngineTrail");
        engineObj.transform.SetParent(go.transform);
        engineObj.transform.localPosition = new Vector3(0f, -0.7f, 0f);
        engineObj.transform.localRotation = Quaternion.Euler(90f, 0f, 0f); // Aponta para baixo
        ParticleSystem enginePs = engineObj.AddComponent<ParticleSystem>();
        var emain = enginePs.main;
        emain.duration = 1f;
        emain.loop = true;
        emain.startLifetime = 0.28f;
        emain.startSpeed = new ParticleSystem.MinMaxCurve(3.0f, 5.5f);
        emain.startSize = new ParticleSystem.MinMaxCurve(0.12f, 0.25f);
        emain.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 0.6f, 0.1f, 0.9f), new Color(1f, 0.2f, 0f, 0.7f));

        var eshape = enginePs.shape;
        eshape.shapeType = ParticleSystemShapeType.Cone;
        eshape.angle = 12f;
        eshape.radius = 0.08f;

        var eemission = enginePs.emission;
        eemission.rateOverTime = 35f;

        // PlayerController
        PlayerController pc = go.AddComponent<PlayerController>();
        SerializedObject soPc = new SerializedObject(pc);
        soPc.FindProperty("moveSpeed").floatValue = 8.5f;
        soPc.FindProperty("screenPadding").floatValue = 0.65f;
        soPc.FindProperty("maxLives").intValue = 3;
        soPc.FindProperty("engineTrail").objectReferenceValue = enginePs;
        soPc.FindProperty("explosionPrefab").objectReferenceValue = explosionPrefab;
        soPc.FindProperty("shieldVisual").objectReferenceValue = shieldObj;
        soPc.ApplyModifiedPropertiesWithoutUndo();

        // PlayerShooter
        PlayerShooter ps = go.AddComponent<PlayerShooter>();
        SerializedObject soPs = new SerializedObject(ps);
        soPs.FindProperty("fireRate").floatValue = 0.22f;
        soPs.FindProperty("projectileSpeed").floatValue = 16f;
        soPs.FindProperty("projectileDamage").intValue = 1;
        soPs.FindProperty("projectilePrefab").objectReferenceValue = projectilePrefab;
        soPs.FindProperty("firePoint").objectReferenceValue = gun.transform;
        soPs.ApplyModifiedPropertiesWithoutUndo();

        string path = "Assets/Prefabs/Player.prefab";
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return prefab;
    }

    // -------------------------------------------------------------------------
    // MONTAGEM DA CENA JOGO.UNITY
    // -------------------------------------------------------------------------
    private static void SetupGameScene(
        GameObject meteorSmallPrefab,
        GameObject meteorMediumPrefab,
        GameObject meteorLargePrefab,
        GameObject playerPrefab)
    {
        string scenePath = "Assets/Scenes/jogo.unity";
        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);

        // Limpa objetos antigos para recriar perfeitamente
        GameObject[] rootObjects = scene.GetRootGameObjects();
        foreach (GameObject obj in rootObjects)
        {
            // Preserva apenas a luz global URP caso exista
            if (obj.name.Contains("Global Light") || obj.name.Contains("Directional Light"))
            {
                continue;
            }
            Object.DestroyImmediate(obj);
        }

        // 1. Main Camera
        GameObject camGo = new GameObject("Main Camera");
        camGo.tag = "MainCamera";
        camGo.transform.position = new Vector3(0f, 0f, -10f);
        Camera cam = camGo.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.02f, 0.02f, 0.05f, 1f);
        camGo.AddComponent<AudioListener>();
        CameraShake cameraShake = camGo.AddComponent<CameraShake>();

        // 2. Fundo Espacial com Parallax
        GameObject bgParent = new GameObject("ParallaxBackground");
        ParallaxBackground parallax = bgParent.AddComponent<ParallaxBackground>();

        // Camada 1: Espaco Profundo
        Sprite deepSpaceSprite = GetStandaloneSprite(DEEP_SPACE_PATH);
        Transform deepA = CreateBackgroundLayerObject("DeepSpace_A", deepSpaceSprite, bgParent.transform, 0, new Vector3(0, 0, 0), new Vector3(1.4f, 1.4f, 1f));
        Transform deepB = CreateBackgroundLayerObject("DeepSpace_B", deepSpaceSprite, bgParent.transform, 0, new Vector3(0, 16, 0), new Vector3(1.4f, 1.4f, 1f));

        // Camada 2: Nebulosa / Aglomerado
        Sprite nebulaSprite = GetStandaloneSprite(NEBULA_PATH);
        Transform nebulaA = CreateBackgroundLayerObject("Nebula_A", nebulaSprite, bgParent.transform, 1, new Vector3(0, 0, 0), new Vector3(1.8f, 1.8f, 1f));
        Transform nebulaB = CreateBackgroundLayerObject("Nebula_B", nebulaSprite, bgParent.transform, 1, new Vector3(0, 16, 0), new Vector3(1.8f, 1.8f, 1f));

        // Camada 3: Estrelas
        Sprite starsSprite = GetStandaloneSprite(STARS_PATH);
        Transform starsA = CreateBackgroundLayerObject("Stars_A", starsSprite, bgParent.transform, 1, new Vector3(0, 0, 0), new Vector3(2.5f, 2.5f, 1f));
        Transform starsB = CreateBackgroundLayerObject("Stars_B", starsSprite, bgParent.transform, 1, new Vector3(0, 16, 0), new Vector3(2.5f, 2.5f, 1f));

        SerializedObject soParallax = new SerializedObject(parallax);
        SerializedProperty layersProp = soParallax.FindProperty("layers");
        layersProp.arraySize = 3;

        // Layer 0: Deep Space
        SerializedProperty l0 = layersProp.GetArrayElementAtIndex(0);
        l0.FindPropertyRelative("layerName").stringValue = "DeepSpace";
        l0.FindPropertyRelative("spriteTransformA").objectReferenceValue = deepA;
        l0.FindPropertyRelative("spriteTransformB").objectReferenceValue = deepB;
        l0.FindPropertyRelative("scrollSpeed").floatValue = 0.8f;
        l0.FindPropertyRelative("layerHeight").floatValue = 16f;
        l0.FindPropertyRelative("horizontalParallaxFactor").floatValue = 0.02f;

        // Layer 1: Nebula
        SerializedProperty l1 = layersProp.GetArrayElementAtIndex(1);
        l1.FindPropertyRelative("layerName").stringValue = "Nebula";
        l1.FindPropertyRelative("spriteTransformA").objectReferenceValue = nebulaA;
        l1.FindPropertyRelative("spriteTransformB").objectReferenceValue = nebulaB;
        l1.FindPropertyRelative("scrollSpeed").floatValue = 1.6f;
        l1.FindPropertyRelative("layerHeight").floatValue = 16f;
        l1.FindPropertyRelative("horizontalParallaxFactor").floatValue = 0.05f;

        // Layer 2: Stars
        SerializedProperty l2 = layersProp.GetArrayElementAtIndex(2);
        l2.FindPropertyRelative("layerName").stringValue = "Stars";
        l2.FindPropertyRelative("spriteTransformA").objectReferenceValue = starsA;
        l2.FindPropertyRelative("spriteTransformB").objectReferenceValue = starsB;
        l2.FindPropertyRelative("scrollSpeed").floatValue = 2.4f;
        l2.FindPropertyRelative("layerHeight").floatValue = 16f;
        l2.FindPropertyRelative("horizontalParallaxFactor").floatValue = 0.08f;

        soParallax.ApplyModifiedPropertiesWithoutUndo();

        // 3. AudioManager
        GameObject audioGo = new GameObject("AudioManager");
        AudioManager audioMgr = audioGo.AddComponent<AudioManager>();

        // 4. Player
        GameObject playerInstance = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
        playerInstance.transform.position = new Vector3(0f, -3.5f, 0f);

        // 5. SpawnManager
        GameObject spawnGo = new GameObject("SpawnManager");
        SpawnManager spawnMgr = spawnGo.AddComponent<SpawnManager>();
        SerializedObject soSpawn = new SerializedObject(spawnMgr);
        soSpawn.FindProperty("meteorSmallPrefab").objectReferenceValue = meteorSmallPrefab;
        soSpawn.FindProperty("meteorMediumPrefab").objectReferenceValue = meteorMediumPrefab;
        soSpawn.FindProperty("meteorLargePrefab").objectReferenceValue = meteorLargePrefab;
        soSpawn.FindProperty("spawnRate").floatValue = 1.6f;
        soSpawn.FindProperty("spawnAreaWidth").floatValue = 15f;
        soSpawn.ApplyModifiedPropertiesWithoutUndo();

        // 6. Canvas & UI
        GameObject canvasGo = new GameObject("Canvas");
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGo.AddComponent<CanvasScaler>();
        CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasGo.AddComponent<GraphicRaycaster>();

        // HUD Elements (TextMeshProUGUI)
        TextMeshProUGUI scoreText = CreateTMPText("ScoreText", canvasGo.transform, "SCORE: 0000", 34, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(180f, -60f));
        TextMeshProUGUI highScoreText = CreateTMPText("HighScoreText", canvasGo.transform, "BEST: 0000", 26, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(180f, -110f));
        TextMeshProUGUI livesText = CreateTMPText("LivesText", canvasGo.transform, "LIVES: 3", 34, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-160f, -60f));
        livesText.alignment = TextAlignmentOptions.TopRight;
        TextMeshProUGUI survivalTimeText = CreateTMPText("SurvivalTimeText", canvasGo.transform, "TIME: 00:00", 32, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -60f));
        survivalTimeText.alignment = TextAlignmentOptions.Top;

        // GameOverPanel
        GameObject gameOverPanel = new GameObject("GameOverPanel");
        gameOverPanel.transform.SetParent(canvasGo.transform, false);
        RectTransform goRect = gameOverPanel.AddComponent<RectTransform>();
        goRect.anchorMin = Vector2.zero;
        goRect.anchorMax = Vector2.one;
        goRect.sizeDelta = Vector2.zero;
        Image panelBg = gameOverPanel.AddComponent<Image>();
        panelBg.color = new Color(0.04f, 0.04f, 0.08f, 0.88f);

        // Titulo Game Over
        TextMeshProUGUI gameOverTitle = CreateTMPText("GameOverTitle", gameOverPanel.transform, "GAME OVER", 68, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 160f));
        gameOverTitle.color = new Color(1f, 0.3f, 0.2f, 1f);
        gameOverTitle.fontStyle = FontStyles.Bold;
        gameOverTitle.alignment = TextAlignmentOptions.Center;

        TextMeshProUGUI finalScoreText = CreateTMPText("FinalScoreText", gameOverPanel.transform, "Score: 0000", 38, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 70f));
        finalScoreText.alignment = TextAlignmentOptions.Center;

        TextMeshProUGUI bestScoreText = CreateTMPText("BestScoreText", gameOverPanel.transform, "Best: 0000", 32, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 15f));
        bestScoreText.color = new Color(1f, 0.85f, 0.2f, 1f);
        bestScoreText.alignment = TextAlignmentOptions.Center;

        TextMeshProUGUI finalSurvivalTimeText = CreateTMPText("FinalSurvivalTimeText", gameOverPanel.transform, "Survival Time: 00:00", 28, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -35f));
        finalSurvivalTimeText.alignment = TextAlignmentOptions.Center;

        // Botao Reiniciar
        GameObject restartBtnObj = new GameObject("RestartButton");
        restartBtnObj.transform.SetParent(gameOverPanel.transform, false);
        RectTransform btnRect = restartBtnObj.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.5f);
        btnRect.anchorMax = new Vector2(0.5f, 0.5f);
        btnRect.anchoredPosition = new Vector2(0f, -130f);
        btnRect.sizeDelta = new Vector2(240f, 65f);
        Image btnImg = restartBtnObj.AddComponent<Image>();
        btnImg.color = new Color(0.2f, 0.55f, 1f, 1f);
        Button restartBtn = restartBtnObj.AddComponent<Button>();

        TextMeshProUGUI btnText = CreateTMPText("BtnText", restartBtnObj.transform, "REINICIAR", 26, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero);
        btnText.alignment = TextAlignmentOptions.Center;
        btnText.fontStyle = FontStyles.Bold;

        gameOverPanel.SetActive(false);

        // 7. GameManager
        GameObject gmGo = new GameObject("GameManager");
        GameManager gm = gmGo.AddComponent<GameManager>();
        SerializedObject soGm = new SerializedObject(gm);
        soGm.FindProperty("scoreText").objectReferenceValue = scoreText;
        soGm.FindProperty("highScoreText").objectReferenceValue = highScoreText;
        soGm.FindProperty("livesText").objectReferenceValue = livesText;
        soGm.FindProperty("survivalTimeText").objectReferenceValue = survivalTimeText;
        soGm.FindProperty("gameOverPanel").objectReferenceValue = gameOverPanel;
        soGm.FindProperty("finalScoreText").objectReferenceValue = finalScoreText;
        soGm.FindProperty("bestScoreText").objectReferenceValue = bestScoreText;
        soGm.FindProperty("finalSurvivalTimeText").objectReferenceValue = finalSurvivalTimeText;
        soGm.FindProperty("restartButton").objectReferenceValue = restartBtn;
        soGm.FindProperty("spawnManager").objectReferenceValue = spawnMgr;
        soGm.ApplyModifiedPropertiesWithoutUndo();

        // 8. EventSystem
        GameObject eventGo = new GameObject("EventSystem");
        eventGo.AddComponent<EventSystem>();
        eventGo.AddComponent<InputSystemUIInputModule>();

        // Salvar cena
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("===> Cena jogo.unity salva com sucesso!");
    }

    private static Transform CreateBackgroundLayerObject(string name, Sprite sprite, Transform parent, int sortingOrder, Vector3 localPos, Vector3 localScale)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent);
        go.transform.localPosition = localPos;
        go.transform.localScale = localScale;

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = sortingOrder;
        return go.transform;
    }

    private static TextMeshProUGUI CreateTMPText(string name, Transform parent, string text, float fontSize, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);

        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(350f, 60f);

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Left;
        return tmp;
    }
}
