using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class DoorInteractHandler : AnimationInteractHandler
{
   Animator Animator => _animator = _animator != null ? _animator : GetComponentInChildren<Animator>();

   [BoxGroup("Door")]
   [SerializeField] bool _isOpen;

   Animator _animator;

   public override async UniTask Interact(IInteractor interactor)
   {
      await base.Interact(interactor);
      ToggleDoorState();
   }

   void ToggleDoorState()
   {
      if (_isOpen)
      {
         Animator.Play("Close");
         _isOpen = false;
      }
      else
      {
         Animator.Play("Open");
         _isOpen = true;
      }
   }
}