using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class GenerateAnimatorEnumTriggers : Editor
{
   [MenuItem("Assets/Generate Animator Triggers")]
   static void GenerateAnimatorTriggers()
   {
      AnimatorController animatorController = Selection.activeObject as AnimatorController;

      if (animatorController == null)
      {
         Debug.LogError("Selected object is not an AnimatorController asset.");
         return;
      }

      string assetPath = AssetDatabase.GetAssetPath(animatorController);

      GenerateAnimatorTriggersEnum(animatorController, assetPath);
   }

   static void GenerateAnimatorTriggersEnum(AnimatorController animatorController, string assetPath)
   {
      AnimatorControllerParameter[] parameters = animatorController.parameters;
      string controllerName = animatorController.name.Replace(" ", "");

      StringBuilder stringBuilder = new StringBuilder();

      // Add comments to the top of the generated code
      stringBuilder.AppendLine("//********************");
      stringBuilder.AppendLine("// Generated from: " + animatorController.name);
      stringBuilder.AppendLine("// Generated via GenerateAnimatorEnumTriggers.cs");
      stringBuilder.AppendLine("//********************");

      // Add the enum definition
      stringBuilder.AppendLine("public enum " + controllerName + "Triggers");
      stringBuilder.AppendLine("{");

      // Loop through all the parameters and add the trigger values to the enum
      for (int i = 0; i < parameters.Length; i++)
      {
         if (parameters[i].type == AnimatorControllerParameterType.Trigger)
         {
            stringBuilder.AppendLine("   " + parameters[i].name + ",");
         }
      }

      stringBuilder.AppendLine("}");

      string outputPath = Path.GetDirectoryName(assetPath) + "/../_Scripts/AnimatorTriggers/" + controllerName + "Triggers.cs";
      File.WriteAllText(outputPath, stringBuilder.ToString());
      AssetDatabase.Refresh();
      Debug.Log("Animator triggers enum generated and saved to " + outputPath);
   }
}