using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEditor.Build.Reporting;

public static class CreateCookieGame
{
    static Font font;
    static Color background = new Color(0.86f, 0.87f, 0.89f);
    static RectTransform Rect(string name, Transform parent, float x0, float y0, float x1, float y1)
    {
        GameObject obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        RectTransform r = (RectTransform)obj.transform;
        r.anchorMin = new Vector2(x0, y0); r.anchorMax = new Vector2(x1, y1);
        r.offsetMin = r.offsetMax = Vector2.zero;
        return r;
    }
    static Text Label(string name, Transform parent, string value, int size, float x0, float y0, float x1, float y1)
    {
        Text text = Rect(name, parent, x0, y0, x1, y1).gameObject.AddComponent<Text>();
        text.font = font; text.text = value; text.fontSize = size;
        text.color = new Color(0.15f, 0.16f, 0.19f); text.alignment = TextAnchor.MiddleCenter;
        text.raycastTarget = false;
        return text;
    }
    static Button Button(string name, Transform parent, string value, float x0, float y0, float x1, float y1, UnityAction action)
    {
        RectTransform r = Rect(name, parent, x0, y0, x1, y1);
        r.gameObject.AddComponent<Image>().color = Color.white;
        Button b = r.gameObject.AddComponent<Button>();
        b.targetGraphic = r.GetComponent<Image>();
        Label("Label", r, value, 38, 0.04f, 0.04f, 0.96f, 0.96f);
        if (action != null) UnityEventTools.AddPersistentListener(b.onClick, action);
        return b;
    }
    static UIWindow Window(string name, Transform parent)
    {
        RectTransform r = Rect(name, parent, 0, 0, 1, 1);
        r.gameObject.AddComponent<Image>().color = background;
        r.gameObject.AddComponent<CanvasGroup>();
        UIWindow window = r.gameObject.AddComponent<UIWindow>();
        Label("Title", r, name, 60, .05f, .86f, .95f, .96f);
        Button("Close", r, "Back", .25f, .025f, .75f, .115f, window.Close);
        return window;
    }
    public static void Create()
    {
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        Camera camera = new GameObject("Main Camera", typeof(Camera)).GetComponent<Camera>();
        camera.tag = "MainCamera"; camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = background;
        camera.transform.position = new Vector3(0, 0, -10);
        GameManager game = new GameObject("GameManager").AddComponent<GameManager>();
        Directory.CreateDirectory("Assets/ScriptableObjects");
        game.clickUpgrade = Upgrade("ClickUpgrade", "Better clicks", 10);
        game.autoUpgrade = Upgrade("AutoUpgrade", "Auto baker", 25);
        Canvas canvas = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)).GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920); scaler.matchWidthOrHeight = 0;
        RectTransform safe = Rect("Safe Area", canvas.transform, 0, 0, 1, 1);
        safe.gameObject.AddComponent<SafeArea>();
        game.scoreText = Label("Score", safe, "Cookies: 0", 64, .04f, .88f, .96f, .98f);
        game.rateText = Label("Rates", safe, "1 per click | 0 per second", 36, .02f, .81f, .98f, .88f);
        Button cookie = Button("Cookie", safe, "", .20f, .29f, .80f, .75f, game.ClickCookie);
        Image cookieImage = cookie.GetComponent<Image>();
        cookieImage.sprite = AssetDatabase.LoadAllAssetsAtPath("Assets/Images/cookie.png").OfType<Sprite>().First();
        cookieImage.preserveAspect = true;
        Label("Hint", safe, "Tap the cookie!", 38, .10f, .20f, .90f, .28f);
        UIWindow upgrades = Window("Upgrades", safe);
        game.clickBuyButton = Button("Click upgrade", upgrades.transform, "", .08f, .53f, .92f, .78f, game.BuyClickUpgrade);
        game.autoBuyButton = Button("Auto upgrade", upgrades.transform, "", .08f, .23f, .92f, .48f, game.BuyAutoUpgrade);
        game.clickLabel = game.clickBuyButton.GetComponentInChildren<Text>();
        game.autoLabel = game.autoBuyButton.GetComponentInChildren<Text>();
        UIWindow shop = Window("Cash Shop", safe);
        Label("Simulation notice", shop.transform, "Demo purchases - no real money charged", 30, .03f, .80f, .97f, .86f);
        RectTransform viewport = Rect("Viewport", shop.transform, .06f, .14f, .94f, .78f);
        viewport.gameObject.AddComponent<Image>().color = new Color(.78f, .79f, .81f);
        viewport.gameObject.AddComponent<RectMask2D>();
        ScrollRect scroll = viewport.gameObject.AddComponent<ScrollRect>();
        scroll.horizontal = false; scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.viewport = viewport;
        RectTransform content = Rect("Content", viewport, 0, 1, 1, 1);
        content.pivot = new Vector2(.5f, 1);
        GridLayoutGroup grid = content.gameObject.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(900, 220); grid.spacing = new Vector2(0, 24);
        grid.padding = new RectOffset(24, 24, 24, 24);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount; grid.constraintCount = 1;
        ContentSizeFitter fitter = content.gameObject.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        scroll.content = content;
        int[] amounts = { 1000, 5000, 12000, 30000, 75000, 200000 };
        string[] prices = { "$0.99", "$2.99", "$4.99", "$9.99", "$19.99", "$39.99" };
        for (int i = 0; i < amounts.Length; i++)
        {
            Button buy = Button("Pack " + (i + 1), content, amounts[i].ToString("N0") + " cookies\n" + prices[i] + " - Get (demo)", 0, 0, 1, 1, null);
            ShopItem item = buy.gameObject.AddComponent<ShopItem>(); item.game = game; item.points = amounts[i];
            UnityEventTools.AddPersistentListener(buy.onClick, item.Buy);
        }
        Button("Open upgrades", safe, "Upgrades", .05f, .05f, .47f, .15f, upgrades.Open);
        Button("Open shop", safe, "Cash Shop", .53f, .05f, .95f, .15f, shop.Open);
        upgrades.transform.SetAsLastSibling(); shop.transform.SetAsLastSibling();
        upgrades.gameObject.SetActive(false); shop.gameObject.SetActive(false);
        new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        PlayerSettings.companyName = "Student"; PlayerSettings.productName = "Cookie Clicker";
        PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.Android, "com.student.cookieclicker");
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel26;
        PlayerSettings.SetScriptingBackend(UnityEditor.Build.NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene(), "Assets/Scenes/CookieClicker.unity");
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene("Assets/Scenes/CookieClicker.unity", true) };
        AssetDatabase.SaveAssets();
        Debug.Log("COOKIE_SCENE_READY");
    }
    static UpgradeData Upgrade(string file, string title, int cost)
    {
        string path = "Assets/ScriptableObjects/" + file + ".asset";
        UpgradeData data = AssetDatabase.LoadAssetAtPath<UpgradeData>(path);
        if (data == null) { data = ScriptableObject.CreateInstance<UpgradeData>(); AssetDatabase.CreateAsset(data, path); }
        data.upgradeName = title; data.baseCost = cost; data.pointsPerLevel = 1; data.costMultiplier = 1.5f;
        EditorUtility.SetDirty(data);
        return data;
    }
    public static void Build()
    {
        Create();
        Directory.CreateDirectory("Builds");
        BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
            scenes = new[] { "Assets/Scenes/CookieClicker.unity" },
            locationPathName = "Builds/CookieClicker.apk", target = BuildTarget.Android, options = BuildOptions.None
        });
        if (report.summary.result != BuildResult.Succeeded) throw new System.Exception("Android build failed: " + report.summary.result);
        Debug.Log("COOKIE_APK_READY");
    }
}
