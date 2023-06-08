using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine.Device;
using Debug = UnityEngine.Debug;

public class SaveDataManagerEditor
{
   [MenuItem("Save System/Save Now")]
   public static void Save()
   {
      SaveDataManager.Save();
   }

   [MenuItem("Save System/Reinitialize")]
   public static void Reinitialize()
   {
      SaveDataManager.Reinitialize();
   }

   [MenuItem("Save System/Load")]
   public static void Load()
   {
      SaveDataManager.Load();
   }

   [MenuItem("Save System/Open Profile Folder")]
   public static void OpenProfileFolder()
   {
      if (Directory.Exists(Application.persistentDataPath))
      {
         string persistentDataPath = Application.persistentDataPath.Replace("/", "\\"); // Replace forward slashes with backslashes
         Process.Start("explorer.exe", persistentDataPath);
      }
      else
      {
         Debug.LogError("Folder does not exist: " + Application.persistentDataPath);
      }
   }
}