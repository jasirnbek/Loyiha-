using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using VirtualPhysicalLab.Core.UI;

public static class LabSceneGenerator
{
    private static readonly string[] SceneNames = BuildSceneNames();

    [MenuItem("Tools/Virtual Physical Lab/Generate Lab Scenes")]
    public static void GenerateAllScenes()
    {
        Directory.CreateDirectory("Assets/Scenes");
        var settings = new List<EditorBuildSettingsScene>();

        GenerateScene("MainMenu");
        settings.Add(new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true));

        for (int i = 1; i <= 21; i++)
        {
            string name = $"Lab{i:00}";
            GenerateScene(name);
            settings.Add(new EditorBuildSettingsScene($"Assets/Scenes/{name}.unity", true));
        }

        EditorBuildSettings.scenes = settings.ToArray();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[Virtual Physical Lab] MainMenu va 21 laboratoriya sahnasi yaratildi.");
    }

    private static void GenerateScene(string sceneName)
    {
        string path = $"Assets/Scenes/{sceneName}.unity";
        if (File.Exists(path)) return;

        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        var bootstrap = new GameObject("LabSceneBootstrap");
        bootstrap.AddComponent<LabSceneBootstrap>();
        EditorSceneManager.SaveScene(scene, path);
    }

    private static string[] BuildSceneNames()
    {
        var names = new string[22];
        names[0] = "MainMenu";
        for (int i = 1; i <= 21; i++) names[i] = $"Lab{i:00}";
        return names;
    }
}