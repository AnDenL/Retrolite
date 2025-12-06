using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using System.IO;

public static class AutoAddressables
{
    private const string RootFolder = "Assets/Game";

    [MenuItem("Tools/Rebuild Addressables Map")]
    public static void RebuildNow() => Run();

    [InitializeOnLoadMethod]
    static void OnLoad()
    {
        EditorApplication.delayCall += () =>
        {
            Run();
        };
    }

    static void Run()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null) 
            return;

        var guids = AssetDatabase.FindAssets("", new[] { RootFolder });

        var group = settings.DefaultGroup;

        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);

            if (AssetDatabase.IsValidFolder(path))
                continue;

            var entry = settings.CreateOrMoveEntry(guid, group);

            var relative = path.Substring(RootFolder.Length + 1);
            relative = Path.ChangeExtension(relative, null);
            relative = relative.Replace('\\', '/');

            entry.address = relative;
        }

        AssetDatabase.SaveAssets();
    }
}