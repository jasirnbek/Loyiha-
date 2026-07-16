using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class UnityBuilder
{
    public static void BuildProject()
    {
        // 1. Aktiv sahnalarni olish (Siz yaratadigan barcha laboratoriya sahnalari)
        string[] scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();

        if (scenes.Length == 0)
        {
            // Sahna belgilanmagan bo'lsa, loyihadagi birinchi topilgan sahnani avtomat oladi
            scenes = AssetDatabase.FindAssets("t:Scene")
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .ToArray();
        }

        // 2. Buyruqlar qatoridan (Command Line) parametrlarni o'qish
        string[] args = Environment.GetCommandLineArgs();
        string buildTargetArg = GetArg("-buildTarget", args);
        
        BuildTarget target = BuildTarget.WebGL;
        string buildPath = "build/WebGL";

        if (buildTargetArg != null)
        {
            if (buildTargetArg.Equals("Android", StringComparison.OrdinalIgnoreCase))
            {
                target = BuildTarget.Android;
                buildPath = "build/Android/virtual_lab.apk";
            }
            else if (buildTargetArg.Equals("WebGL", StringComparison.OrdinalIgnoreCase))
            {
                target = BuildTarget.WebGL;
                buildPath = "build/WebGL";
            }
        }

        // Papkani yaratish
        string directory = Path.GetDirectoryName(buildPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // 3. Build parametrlarini shakllantirish
        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = buildPath,
            target = target,
            options = BuildOptions.None
        };

        // 4. Buildni boshlash
        Debug.Log($"[CI-CD] Build boshlanmoqda. Target: {target}, Path: {buildPath}");
        var report = BuildPipeline.BuildPlayer(options);
        
        if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log("[CI-CD] Build muvaffaqiyatli yakunlandi!");
            EditorApplication.Exit(0);
        }
        else
        {
            Debug.LogError($"[CI-CD] Buildda xatolik yuz berdi: {report.summary.totalErrors} ta xato.");
            EditorApplication.Exit(1);
        }
    }

    private static string GetArg(string name, string[] args)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == name && i + 1 < args.Length)
            {
                return args[i + 1];
            }
        }
        return null;
    }
}
