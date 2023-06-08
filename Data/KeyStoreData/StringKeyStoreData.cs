using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "NewStringData", menuName = "House/String Key Data")]
public class StringKeyStoreData : KeyStoreData
{
   public override object Value => StringValue;

   [BoxGroup("Value")]
   public string StringValue;
}