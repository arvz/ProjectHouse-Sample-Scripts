using System;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// This is the class that holds ALL save data and to be serialized into json
/// </summary>
[Serializable, Searchable]
[JsonObject(MemberSerialization.OptIn)]
[CreateAssetMenu(fileName = "EditorSave", menuName = "House/Editor Save")]
public class SaveData : SerializedScriptableObject
{
   [BoxGroup("Story Save Data")]
   [JsonProperty]
   public StorySaveData StorySaveData;

   //todo ItemSaveData
   //todo EquipmentSaveData

   public void Initialize()
   {
      StorySaveData = new StorySaveData();
      StorySaveData.LoadKeyStores();
      StorySaveData.ResetSaves();
   }
}