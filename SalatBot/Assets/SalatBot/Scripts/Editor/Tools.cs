using UnityEditor;
using UnityEngine;

public class Tools : EditorWindow
{
    [MenuItem("Tools/Clear PlayerPrefs")]
    static void DeleteAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    [MenuItem("Tools/ClearUsersData")]
    static void ClearUsersData()
    {
        if (System.IO.File.Exists(Application.persistentDataPath + "/usersData.json"))
        {
            System.IO.File.Delete(Application.persistentDataPath + "/usersData.json");
        }
    }
    
    [MenuItem("Tools/ClearCache")]
    static void ClearCache()
    {
        Caching.ClearCache();
    }
}