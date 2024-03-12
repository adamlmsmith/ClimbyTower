using UnityEngine;
using UnityEditor;

public static class MenuItems
{
    [MenuItem("Assets/Open PersistentDataPath in Finder")]
    public static void ShowPersistentDataPath()
    {
        string path = System.IO.Path.GetFullPath(Application.persistentDataPath);
        EditorUtility.RevealInFinder(path);
    }

    [MenuItem("Assets/Open DataPath in Finder")]
    public static void ShowDataPath()
    {
        string path = System.IO.Path.GetFullPath(Application.dataPath);
        EditorUtility.RevealInFinder(path);
    }
}
