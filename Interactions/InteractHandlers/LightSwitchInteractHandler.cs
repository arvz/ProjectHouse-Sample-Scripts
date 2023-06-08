using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.Serialization;
using UnityEngine;

public class LightSwitchInteractHandler : AnimationInteractHandler
{
   [OdinSerialize] public Light Light { get; private set; }

   public override async UniTask Interact(IInteractor interactor)
   {
      await base.Interact(interactor);
      Light.gameObject.SetActive(!Light.gameObject.activeInHierarchy);
   }
}
