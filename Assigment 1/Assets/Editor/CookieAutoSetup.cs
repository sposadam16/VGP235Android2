using System.IO;
using UnityEditor;

[InitializeOnLoad]
public static class CookieAutoSetup
{
    static CookieAutoSetup() { EditorApplication.delayCall += Run; }
    static void Run()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
        { EditorApplication.delayCall += Run; return; }
        if (File.Exists("Assets/Scenes/CookieClicker.unity")) return;
        CreateCookieGame.Build();
    }
}
