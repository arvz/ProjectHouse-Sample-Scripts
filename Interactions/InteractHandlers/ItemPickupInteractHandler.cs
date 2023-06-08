using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Enums;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

public class ItemPickupInteractHandler : AnimationInteractHandler
{
   [BoxGroup("Item Pick-up")]
   [OdinSerialize] public ItemData Item { get; private set; }

   public override async UniTask Interact(IInteractor interactor)
   {
      await base.Interact(interactor);
      Messaging.SendMessage(MessageType.PickUpItem, Item);
      gameObject.SetActive(false);
   }
}
