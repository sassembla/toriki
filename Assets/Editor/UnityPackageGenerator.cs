using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class UnityPackageGenerator
{
    [MenuItem("Window/Toriki/Generate UnityPackage")]
    public static void UnityPackage()
    {
        var assetPaths = new List<string>();

        var torikiPath = "Assets/Toriki";
        CollectPathRecursive(torikiPath, assetPaths);

        var pluginsPath = "Assets/Plugins";
        CollectPathRecursive(pluginsPath, assetPaths);

        AssetDatabase.ExportPackage(assetPaths.ToArray(), "Toriki.unitypackage", ExportPackageOptions.IncludeDependencies);
    }

    private static void CollectPathRecursive(string path, List<string> collectedPaths)
    {
        var filePaths = Directory.GetFiles(path);
        foreach (var filePath in filePaths)
        {
            collectedPaths.Add(filePath);
        }

        var modulePaths = Directory.GetDirectories(path);
        foreach (var folderPath in modulePaths)
        {
            CollectPathRecursive(folderPath, collectedPaths);
        }
    }
}