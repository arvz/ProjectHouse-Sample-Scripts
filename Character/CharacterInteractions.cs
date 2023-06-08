using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;
using Enums;

/// <summary>
/// Responsible for handling Interactions between InteractHandlers and the Character
/// </summary>
public class CharacterInteractions : MonoBehaviour, IInteractor, IAnimationEventReceiver
{
   public event Action OnInspect;
   public event Action OnUninspect;

   [InspectorPrivate] public InteractHandler CurrentInspected { get; private set; }

   [SerializeField] float _timeToRotateTowardsTarget = 0.2f;
   [SerializeField] float _angleThreshold = 45f;
   [SerializeField] float _maxDistance = 0.6f;
   [InspectorPrivate] CharacterAnimationState _state;

   Animator _animator;
   MyPlayerInput _input;
   Collider _collider;
   CharacterMovement _movement;
   bool _isInteracting;
   bool _animEventPickedUpItem;

   void Awake()
   {
      _input = GetComponent<MyPlayerInput>();
      _animator = GetComponentInChildren<Animator>();
      _collider = GetComponent<Collider>();
      _movement = GetComponent<CharacterMovement>();

   }

   void OnEnable()
   {
      _input.OnInteractButtonPressed += OnInteractButtonPressed;
   }

   void OnDisable()
   {
      _input.OnInteractButtonPressed -= OnInteractButtonPressed;
   }

   void Update()
   {
      InspectionUpdate();
   }

   public void AnimationEventInvoke(AnimationEventType eventType)
   {
      if (eventType == AnimationEventType.PickUpItem)
      {
         _animEventPickedUpItem = true;
      }
   }

   public void LinkSMB()
   {
      SceneLinkedSMB<CharacterInteractions>.Initialise(_animator, this);
      SceneLinkedSMB<IAnimationEventReceiver>.Initialise(_animator, this);
   }

   public void SetState(CharacterAnimationState characterState)
   {
      _state = characterState;
   }

   public async UniTask PreInteract(InteractHandler interactHandler)
   {
      if (interactHandler.InteractionPoint != null)
      {
         //Has an interaction point, so we move to the interact point and rotate to match the rotation of the interaction point
         _collider.enabled = false;
         await _movement.WalkToPosition(interactHandler.InteractionPoint.position);
         await transform.DORotateQuaternion(interactHandler.InteractionPoint.rotation, 1f).AsyncWaitForCompletion();
      }
      else
      {
         //No interaction point, so we just rotate to look towards the interact handler
         var interactHandlerFixedPosition = interactHandler.transform.position.Y0();
         var rotation = Quaternion.LookRotation(interactHandlerFixedPosition - transform.position);
         await transform.DORotateQuaternion(rotation, _timeToRotateTowardsTarget).AsyncWaitForCompletion();
      }
   }

   public async UniTask PlayAnimation(CharacterAnimationState characterAnimationStateType)
   {
      _animEventPickedUpItem = false;

      if (characterAnimationStateType == CharacterAnimationState.SittingDown)
      {
         _animator.SetTrigger("Sit");

         //automatically stand back up after some time (placeholder behaviour)
         await UniTask.Delay(3250);
         _animator.SetTrigger("Stand");

         await UniTask.WaitUntil(() => _state == CharacterAnimationState.Idle);
         await UniTask.Delay(500);
      }
      else if (characterAnimationStateType == CharacterAnimationState.InteractHand)
      {
         _animator.SetTrigger("InteractHand");
         await UniTask.Delay(500);
      }
      else if (characterAnimationStateType == CharacterAnimationState.PickUpFloor)
      {
         _animator.SetTrigger("PickUpFloor");
         await UniTask.WaitUntil(() => _animEventPickedUpItem);
      }
   }

   public void FinishInteraction()
   {
      Messaging.SendMessage(MessageType.CharacterInteractionFinished);
      _isInteracting = false;
      _collider.enabled = true;
   }

   void InspectionUpdate()
   {
      if (_isInteracting) return;

      var validInteractable = FindValidInteractable();

      if (validInteractable != null)
      {
         if (CurrentInspected != validInteractable)
         {
            CurrentInspected = validInteractable;
            OnInspect?.Invoke();
         }

         validInteractable.Inspect();
      }
      else if (CurrentInspected != null)
      {
         CurrentInspected = null;
         OnUninspect?.Invoke();
      }
   }

   void OnInteractButtonPressed()
   {
      if (CurrentInspected != null && enabled && _isInteracting == false)
      {
         CurrentInspected.BeginInteraction(this).Forget();
         _isInteracting = true;
      }
   }

   InteractHandler FindValidInteractable()
   {
      var allInteractables = FindObjectsOfType<InteractHandler>();

      foreach (var interactable in allInteractables)
      {
         var targetPos = new Vector3(interactable.transform.position.x, transform.position.y, interactable.transform.position.z);
         var distance = Vector3.Distance(transform.position, targetPos);

         if (distance <= _maxDistance)
         {
            Vector3 targetDirection = targetPos - transform.position;

            // Calculate the angle between this object's forward vector and the target direction
            float angle = Vector3.Angle(transform.forward, targetDirection);

            // Check if the angle is within the threshold
            if (angle < _angleThreshold)
            {
               return interactable;
            }
         }
      }

      return null;
   }
}