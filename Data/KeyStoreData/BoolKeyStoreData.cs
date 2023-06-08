using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBoolData", menuName = "House/Bool Key Data")]
public class BoolKeyStoreData : KeyStoreData
{
   public override object Value => BoolValue;

   [BoxGroup("Value")]
   public bool BoolValue;
}