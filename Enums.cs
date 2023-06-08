public enum CharacterAnimationState
{
   Idle,
   Walking,
   SittingDown,
   Sitting,
   StandingUp,
   InteractHand,
   PickUpFloor,
}


namespace Enums
{
   public enum MessageType
   {
      CharacterPreInteracting,
      CharacterInteractionStarted,
      CharacterInteractionFinished,
      PickUpItem, //Item
      ReinitializeSLSMB,
      CharacterEntersRoom, //RoomController
      CharacterExitsRoom, //RoomController
      FloorTransition, //Floor, Floor
      FloorTransitionComplete
   }
}

public enum AnimationEventType
{
   PickUpItem,
   Footstep
}

public enum EquipmentParent
{
   RightHand,
   LeftHand,
   Head
}

public enum CharacterAnimationLayer
{
   BaseLayer = 0,
   RightHandLayer = 1,
   // Add more layers here...
}