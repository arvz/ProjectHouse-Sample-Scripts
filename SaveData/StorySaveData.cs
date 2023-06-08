using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;
#endif

[Serializable]
public class StorySaveData
{
   public const string KeyStoresLocation = "GameData/KeyStores";
   [ListDrawerSettings(
      HideRemoveButton = true,
      Expanded = true,
      OnTitleBarGUI = "DrawReinitializeButton",
      OnBeginListElementGUI = "BeginDrawListElement",
      OnEndListElementGUI = "EndDrawListElement")]
   public List<KeyStoreData> KeyStoreDatas;

   public void LoadKeyStores()
   {
      KeyStoreDatas = Resources.LoadAll<KeyStoreData>(KeyStoresLocation).ToList();
   }

   public bool HasKey(string variableName)
   {
      return KeyStoreDatas.Any(keyData => keyData.name == variableName);
   }

   public int GetStoryPhase()
   {
      var storyPhaseKeyData = KeyStoreDatas.FirstOrDefault(keyData => keyData.name == "$storyPhase");
      if (storyPhaseKeyData == null)
      {
         Debug.LogError("No story phase key data found");
         return -1;
      }

      if (storyPhaseKeyData is FloatKeyStoreData floatKeyData)
      {
         return (int)floatKeyData.FloatValue;
      }

      return -1;
   }

   public object GetValue(string variableName)
   {
      var keyStoreData = KeyStoreDatas.FirstOrDefault(keyData => keyData.name.Equals(variableName));
      if (keyStoreData == null)
      {
         return null;
      }

      return keyStoreData.Value;
   }

   public void SetValue(string variableName, bool boolValue)
   {
      variableName = GetCheckedFixedKey(variableName);

      var keyData = KeyStoreDatas.FirstOrDefault(keyData => keyData.name == variableName);

      if (keyData == null)
      {
         var newBoolKeyData = ScriptableObject.CreateInstance<BoolKeyStoreData>();
         newBoolKeyData.BoolValue = boolValue;

#if UNITY_EDITOR
         AssetDatabase.CreateAsset(newBoolKeyData, $"Assets/Resources/{KeyStoresLocation}/{variableName}.asset");
#endif
         Debug.Log($"#Save# Created new bool value: {variableName} = {boolValue}");
      }
      else
      {
         if (keyData is BoolKeyStoreData boolKeyData)
         {
            boolKeyData.BoolValue = boolValue;
            Debug.Log($"#Save# Set {variableName} to {boolValue}");
         }
         else
         {
            Debug.LogError($"#Save# Tried to set {variableName} to {boolValue} but it was not a bool key data");
         }
      }

      SaveDataManager.Save();
   }

   public void SetValue(string variableName, string stringValue)
   {
      variableName = GetCheckedFixedKey(variableName);

      var keyData = KeyStoreDatas.FirstOrDefault(keyData => keyData.name == variableName);
      if (keyData == null)
      {
         var newStringKeyData = ScriptableObject.CreateInstance<StringKeyStoreData>();
         newStringKeyData.StringValue = stringValue;

#if UNITY_EDITOR
         AssetDatabase.CreateAsset(newStringKeyData, $"Assets/Resources/{KeyStoresLocation}/{variableName}.asset");
#endif
         Debug.Log($"#Save# Created new string value: {variableName} = {stringValue}");

      }
      else
      {
         if (keyData is StringKeyStoreData stringKeyData)
         {
            stringKeyData.StringValue = stringValue;
            Debug.Log($"#Save# Set {variableName} to {stringValue}");
         }
         else
         {
            Debug.LogError($"#Save# Tried to set {variableName} to {stringValue} but it was not a string key data");
         }
      }

      SaveDataManager.Save();
   }

   public void SetValue(string variableName, float floatValue)
   {
      variableName = GetCheckedFixedKey(variableName);

      var keyData = KeyStoreDatas.FirstOrDefault(keyData => keyData.name == variableName);
      if (keyData == null)
      {
         var newFloatKeyData = ScriptableObject.CreateInstance<FloatKeyStoreData>();
         newFloatKeyData.FloatValue = floatValue;
#if UNITY_EDITOR
         AssetDatabase.CreateAsset(newFloatKeyData, $"Assets/Resources/{KeyStoresLocation}/{variableName}.asset");
#endif
         Debug.Log($"#Save# Created new float value: {variableName} = {floatValue}");

      }
      else
      {
         if (keyData is FloatKeyStoreData floatKeyData)
         {
            floatKeyData.FloatValue = floatValue;
            Debug.Log($"#Save# Set {variableName} to {floatValue}");
         }
         else
         {
            Debug.LogError($"#Save# Tried to set {variableName} to {floatValue} but it was not a float key data");
         }
      }

      SaveDataManager.Save();
   }

   bool VariableNameIsValid(string variableName)
   {
      if (variableName.StartsWith("$") == false)
      {
         Debug.LogError(
            $"{variableName} is not a valid variable name: Variable names must start with a '$'. " +
            $"(Did you mean to use '${variableName}'?)");

         return false;
      }

      return true;
   }

   public string GetCheckedFixedKey(string key)
   {
      if (key.StartsWith("$"))
      {
         return key;
      }

      return $"${key}";
   }

   public void ResetSaves()
   {
      foreach (var key in KeyStoreDatas.ToList())
      {
         if (key is StringKeyStoreData stringKeyData) stringKeyData.StringValue = "";
         else if (key is FloatKeyStoreData floatKeyData) floatKeyData.FloatValue = 0f;
         else if (key is BoolKeyStoreData boolKeyData) boolKeyData.BoolValue = false;
         else Debug.LogError($"Unknown type for {key}: {key.GetType()}");
      }
   }


#if UNITY_EDITOR

   public void DrawReinitializeButton()
   {
      if (SirenixEditorGUI.ToolbarButton(EditorIcons.Refresh))
      {
         SaveDataManager.Reinitialize();
      }
   }

   void BeginDrawListElement(int index)
   {
      SirenixEditorGUI.BeginHorizontalToolbar();
   }

   void EndDrawListElement(InspectorProperty property, int index)
   {
      var childProperty = property.Children[index];
      if (childProperty.State.Expanded)
      {
         SirenixEditorGUI.EndHorizontalToolbar();
         return;
      }

      EditorGUI.BeginChangeCheck();

      if (KeyStoreDatas[index] is BoolKeyStoreData boolData)
      {
         boolData.BoolValue = EditorGUILayout.Toggle(boolData.BoolValue, GUILayout.Width(60));
      }
      else if (KeyStoreDatas[index] is StringKeyStoreData stringData)
      {
         stringData.StringValue = EditorGUILayout.TextField(stringData.StringValue, GUILayout.Width(60));
      }
      else if (KeyStoreDatas[index] is FloatKeyStoreData floatData)
      {
         floatData.FloatValue = EditorGUILayout.FloatField(floatData.FloatValue, GUILayout.Width(60));
      }

      if (EditorGUI.EndChangeCheck())
      {
         SaveDataManager.Save();
      }

      SirenixEditorGUI.EndHorizontalToolbar();
   }

#endif
}