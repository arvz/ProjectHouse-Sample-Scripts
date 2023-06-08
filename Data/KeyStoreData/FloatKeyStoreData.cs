using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFloatData", menuName = "House/Float Key Data")]
public class FloatKeyStoreData : KeyStoreData
{
   public override object Value => FloatValue;

   [BoxGroup("Value")]
   public float FloatValue;
}