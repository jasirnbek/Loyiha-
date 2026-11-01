using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class UnityBuilder
{
    public static void BuildProject()
    {
        LabSceneGenerator.GenerateAllScenes();

        string[] scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled && File.Exists(s.path))
            .Select(s => s.path)
            .ToArray();

        if (scenes.Length == 0)
            throw new BuildFailedException("Build uchun sahnalar yaratilmagan. Assets/Scenes papkasini tekshiring.");

        string targetArg = GetArg("-buildTarget", Environment.GetCommandLineArgs());
        BuildTarget target = string.Equals(targetArg, "Android", StringComparison.OrdinalIgnoreCase)
            ? BuildTarget.Android : BuildTarget.WebGL;
        string outputPath = target == BuildTarget.Android
            ? "build/Android/VirtualPhysicalLab.apk" : "build/WebGL";
        string directory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);

        var options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = outputPath,
            target = target,
            options = BuildOptions.None
        };

        Debug.Log($"[CI] {scenes.Length} ta sahna bilan {target} build boshlandi.");
        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
            throw new BuildFailedException($"Build muvaffaqiyatsiz. Natija: {report.summary.result}; xatolar: {report.summary.totalErrors}.");

        Debug.Log($"[CI] Build muvaffaqiyatli. Hajmi: {report.summary.totalSize} bayt");
    }

    private static string GetArg(string name, string[] args)
    {
        for (int i = 0; i < args.Length - 1; i++)
            if (args[i] == name) return args[i + 1];
        return null;
    }
}