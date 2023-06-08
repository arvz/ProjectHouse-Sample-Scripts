using System;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// Responsible for equipping and unequipping items.
/// </summary>
[RequireComponent(typeof(MyPlayerInput))]
public class CharacterEquipment : MonoBehaviour
{
   [SerializeField] EquipmentItemData _flashlightData;
   [SerializeField] Transform _rightHand;

   Animator _animator;
   MyPlayerInput _input;
   EquipmentItemData _currentEquipped;

   GameObject _equippedItemGameObject;

   void Awake()
   {
      _input = GetComponent<MyPlayerInput>();
      _animator = GetComponentInChildren<Animator>();
   }

   void Start()
   {
      _input.OnEquipItemButtonPressed += OnEquipItemButtonPressed;
   }

   void OnDisable()
   {
      _input.OnEquipItemButtonPressed -= OnEquipItemButtonPressed;
   }

   void OnEquipItemButtonPressed()
   {
      if (_currentEquipped != null)
      {
         UnequipItem();
      }
      else if (ItemsManager.Instance.ItemIsOwned(_flashlightData)) //placeholder, always equip flashlight for now
      {
         EquipItem(_flashlightData);
      }
   }

   void UnequipItem()
   {
      Destroy(_equippedItemGameObject);
      SetAnimationLayer(_currentEquipped.AnimationLayer, 0);
      _currentEquipped = null;
   }

   void EquipItem(EquipmentItemData equipmentData)
   {
      var parent = GetEquipmentParent(equipmentData.EquipmentParent);
      _currentEquipped = equipmentData;
      _equippedItemGameObject = Instantiate(equipmentData.Prefab, parent);
      SetAnimationLayer(equipmentData.AnimationLayer, 1);
      _animator.SetTrigger(equipmentData.Trigger.ToString());
   }

   void SetAnimationLayer(CharacterAnimationLayer layer, float weight)
   {
      var layerIndex = (int)layer;
      var duration = 1f;
      DOTween.To(() => _animator.GetLayerWeight(layerIndex), x => _animator.SetLayerWeight(layerIndex, x), weight, duration);
   }

   Transform GetEquipmentParent(EquipmentParent parent)
   {
      switch (parent)
      {
         case EquipmentParent.RightHand:
            return _rightHand;
         default:
            throw new NotSupportedException("Not yet supported");
      }
   }
}