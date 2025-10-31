using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class ProjectStructureCreator : EditorWindow
{
    // Lista de carpetas que vamos a crear (bajo Assets/)
    static readonly string[] folders = new string[]
    {
        "Assets/Scenes",
        "Assets/Art",
        "Assets/Art/Sprites",
        "Assets/Art/Sprites/Cars",
        "Assets/Art/Sprites/Environment",
        "Assets/Art/Sprites/UI",
        "Assets/Art/Textures",
        "Assets/Art/Particles",
        "Assets/Art/Materials",
        "Assets/Animation",
        "Assets/Prefabs",
        "Assets/Audio",
        "Assets/Audio/SFX",
        "Assets/Audio/Music",
        "Assets/Scripts",
        "Assets/Scripts/Editor",
        "Assets/Scripts/Controllers",
        "Assets/Scripts/Systems",
        "Assets/Resources",
        "Assets/Plugins",
        "Assets/Documentation",
        "Assets/Documentation/Notes",
        "Assets/Builds",
        "Assets/Tests",
        "Assets/Fonts",
        "Assets/UI"
    };

    [MenuItem("Tools/Create Project Structure")]
    public static void CreateStructure()
    {
        int created = 0;
        foreach (string folder in folders)
        {
            if (!AssetDatabase.IsValidFolder(folder))
            {
                string parent = Path.GetDirectoryName(folder).Replace('\\', '/');
                string newFolderName = Path.GetFileName(folder);
                if (AssetDatabase.IsValidFolder(parent))
                {
                    AssetDatabase.CreateFolder(parent, newFolderName);
                    created++;
                }
                else
                {
                    // Fallback: create from Assets root if parent doesn't exist yet
                    // This rarely happens because folders is ordered top-down.
                    Directory.CreateDirectory(folder);
                    created++;
                }
            }
            // crear .gitkeep si la carpeta está vacía
            string gitkeep = folder + "/.gitkeep";
            if (!File.Exists(gitkeep))
            {
                File.WriteAllText(gitkeep, "This file keeps the folder in source control.");
                AssetDatabase.ImportAsset(gitkeep);
            }
        }

        // Crear README básico en Documentation si no existe
        string readmePath = "Assets/Documentation/README_ProjectStructure.txt";
        if (!File.Exists(readmePath))
        {
            File.WriteAllText(readmePath,
@"Proyecto: Simulador de Choques 2D
Estructura generada automáticamente por ProjectStructureCreator.
Edite y organize recursos en las carpetas correspondientes.
");
            AssetDatabase.ImportAsset(readmePath);
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Project Structure", $"Estructura creada/actualizada. Carpetas nuevas: {created}", "OK");
    }
}