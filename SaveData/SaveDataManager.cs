using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using Newtonsoft.Json;

public static class SaveDataManager
{
   public static SaveData SaveData
   {
      get
      {
         if (_SaveData == null)
         {
            Load();
         }

         return _SaveData;
      }
   }

   static string _FilePath => Path.Combine(Application.persistentDataPath, "save.json");

   static SaveData _SaveData;

   public static string GetAppliedNodeName(string nodeName)
   {
      string pattern = @"\{\$(.*?)\}"; // Pattern to match anything between {$ and }
      var result = nodeName;
      // Use Regex to find matches
      var matches = Regex.Matches(nodeName, pattern);

      foreach (Match match in matches)
      {
         string key = "$" + match.Groups[1].Value;
         var valueString = SaveData.StorySaveData.GetValue(key);
         if (valueString != null)
         {
            // Replace the match with the value from the dictionary
            var savedValue = valueString.ToString();
            result = result.Replace(match.Value, savedValue);
         }
         else
         {
            Debug.LogError($"Unable to resolve variable {key} in the save system");
         }
      }

      return result;
   }

   public static void Save()
   {
      var json = JsonConvert.SerializeObject(SaveData, Formatting.Indented);
      Debug.Log($"#Save# Save to {_FilePath}");
      File.WriteAllText(_FilePath, json);
   }

   public static void Reinitialize()
   {
      Debug.Log($"#Save# Reinitialize Saves");
      _SaveData = Resources.Load<SaveData>("EditorSave");
      _SaveData.Initialize();
      Save();
   }

   [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
   public static void Load()
   {
      if (_SaveData == null)
      {
         _SaveData = Resources.Load<SaveData>("EditorSave");
      }

      if (File.Exists(_FilePath))
      {
         Debug.Log($"#Save# Load Save from {_FilePath}");

         var json = File.ReadAllText(_FilePath);
         JsonConvert.DeserializeObject<SaveData>(json, new SaveDataConverter());
      }
      else
      {
         Debug.Log($"#arvz# {_FilePath} does not exist");
      }
   }

}