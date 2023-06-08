using System;
using UnityEngine;

public enum ControllerType
{
   Gamepad,
   MouseKeyboard
}

public class MyPlayerInput : MonoBehaviour
{
   public event Action OnInteractButtonPressed;
   public event Action OnEquipItemButtonPressed;

   public Vector2 MoveVectorRaw => GetMoveVector();
   public Vector3 MousePosition => Input.mousePosition;

   PlayerInputActions _actions;

   ControllerType _currentControllerType;
   Vector2 _lookVector;
   Vector2 _moveVectorRaw;
   bool _blockPlayerInput;

   void Awake()
   {
      _actions = new PlayerInputActions();
      _actions.Enable();
   }

   void Update()
   {
      if (_blockPlayerInput)
      {
         return;
      }

      UpdateInteract();
      UpdateEquipItem();
   }

   void UpdateInteract()
   {
      if (_actions.Player.Interact.WasPressedThisFrame())
      {
         OnInteractButtonPressed?.Invoke();
      }
   }

   void UpdateEquipItem()
   {
      if (_actions.Player.EquipItem.WasPressedThisFrame())
      {
         OnEquipItemButtonPressed?.Invoke();
      }
   }

   Vector2 GetMoveVector()
   {
      _moveVectorRaw = _actions.Player.Move.ReadValue<Vector2>();
      return _moveVectorRaw;
   }
}