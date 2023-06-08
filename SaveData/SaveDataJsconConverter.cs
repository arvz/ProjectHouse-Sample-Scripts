using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

/// <summary>
/// Converts SaveData json back into SaveData
/// </summary>
public class SaveDataConverter : JsonConverter<SaveData>
{
   public override SaveData ReadJson(JsonReader reader, Type objectType, SaveData existingValue, bool hasExistingValue, JsonSerializer serializer)
   {
      var saveData = SaveDataManager.SaveData;

      JObject jo = JObject.Load(reader);
      foreach(var j in jo["StorySaveData"]["KeyStoreDatas"])
      {
         var key = j["Name"].ToString();
         var value = j["Value"];

         if (value != null && value.Type == JTokenType.Boolean)
         {
            var boolValue = value.Value<bool>();
            saveData.StorySaveData.SetValue(key, boolValue);
         }
         else if (value != null && value.Type == JTokenType.Float)
         {
            var floatValue = value.Value<float>();
            saveData.StorySaveData.SetValue(key, floatValue);
         }
         else if (value != null && value.Type == JTokenType.String)
         {
            var stringValue = value.Value<string>();
            saveData.StorySaveData.SetValue(key, stringValue);
         }
         else
         {
            Debug.LogError($"#arvz# Unknown type {value.Type}");
         }

      }

      return saveData;
   }

   public override void WriteJson(JsonWriter writer, SaveData value, JsonSerializer serializer)
   {
      // Use the default serialization.
      serializer.Serialize(writer, value);
   }
}
