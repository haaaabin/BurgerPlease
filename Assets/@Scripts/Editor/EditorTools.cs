using UnityEngine;
using UnityEditor;
using System.IO;

public class EditorTools : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/RemoveSaveData")]
    public static void RemoveSaveData()
    {
        string path = Application.persistentDataPath + "/SaveData.json";
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("SaveFile Deleted");
        }
        else
        {
            Debug.Log("No SaveFile Detected");
        }
    }
#endif
}
