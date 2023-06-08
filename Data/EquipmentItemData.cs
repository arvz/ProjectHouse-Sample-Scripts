using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

/// <summary>
/// Stores info about equipment. Equipment is a special kind of Item that the player can equip
/// </summary>
[CreateAssetMenu(fileName = "New Equipment", menuName = "PI/New Equipment")]
public class EquipmentItemData : ItemData
{
   [BoxGroup("Equipment"), OdinSerialize, LabelWidth(100), PropertyOrder(10)]
   public EquipmentType EquipmentType { get; private set; }

   [BoxGroup("Assets"), OdinSerialize, LabelWidth(100), PropertyOrder(10)]
   public GameObject Prefab { get; private set; }

   /// <summary>
   /// Which gameobject parent to attach the equipment to
   /// </summary>
   [BoxGroup("Settings"), OdinSerialize, LabelWidth(100), PropertyOrder(20)]
   public EquipmentParent EquipmentParent { get; private set; }

   /// <summary>
   /// Which animation trigger to fire when this item is equipped
   /// </summary>
   [BoxGroup("Settings"), OdinSerialize, LabelWidth(100), PropertyOrder(21)]
   public WomanAnimatorTriggers Trigger { get; private set; }

   [BoxGroup("Settings"), OdinSerialize, LabelWidth(100), PropertyOrder(22)]
   public CharacterAnimationLayer AnimationLayer { get; private set; }
}