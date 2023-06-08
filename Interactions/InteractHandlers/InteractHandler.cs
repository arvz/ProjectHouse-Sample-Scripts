using System.Collections.Generic;
using HighlightPlus;
using Sirenix.OdinInspector;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Sirenix.Serialization;
using UnityEngine.AI;
using MessageType = Enums.MessageType;

#if UNITY_EDITOR
using UnityEditor;
#endif

[SelectionBase]
[RequireComponent(typeof(HighlightEffect))]
public class InteractHandler : SerializedMonoBehaviour
{
   [InspectorPrivate] public bool IsInspected { get; private set; }
   bool CanInteract => _requirements.IsRequirementsMet();

   [FoldoutGroup("Interaction Point"), HorizontalGroup("Interaction Point/InteractionPointHorizontal"), OdinSerialize]
   public Transform InteractionPoint { get; private set; }

   [FoldoutGroup("Dialogue")][SerializeField]
   bool _hasDialogue;
   [FoldoutGroup("Dialogue"), ShowIf("_hasDialogue"), SerializeField]
   string _nodeNameValid;
   [FoldoutGroup("Dialogue"), ShowIf("_hasDialogue"), SerializeField]
   string _nodeNameInvalid;
   [FoldoutGroup("Requirements"), ListDrawerSettings(Expanded = true), OdinSerialize]
   HouseRequirements _requirements;

   HighlightEffect _highlightEffect;
   bool _isInspectedThisFrame;
   NavMeshObstacle _navMeshObstacle;
   bool _navMeshObstacleWasEnabled;

   protected virtual void Awake()
   {
      _highlightEffect = GetComponent<HighlightEffect>();
      _navMeshObstacle = GetComponent<NavMeshObstacle>();

   }

   [FoldoutGroup("Interaction Point")]
   [HorizontalGroup("Interaction Point/InteractionPointHorizontal")]
   [HideIf("InteractionPoint")]
   [Button("Create", ButtonHeight = 50)]
   public void CreateInteractionPoint()
   {
      var newinteractionPoint = new GameObject($"{transform.name} Interaction Point", typeof(InteractionPoint));
      newinteractionPoint.transform.parent = transform;
      newinteractionPoint.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);

      InteractionPoint = newinteractionPoint.transform;

#if UNITY_EDITOR
      Selection.activeTransform = InteractionPoint.transform;
#endif
   }

   void LateUpdate()
   {
      if (_isInspectedThisFrame)
      {
         _isInspectedThisFrame = false;
         return;
      }

      if (IsInspected == false)
      {
         Uninspect();
      }

      IsInspected = false;
   }

   public virtual void Inspect()
   {
      _isInspectedThisFrame = true;
      IsInspected = true;

      var highlightProfile = _requirements.IsRequirementsMet()
         ? AssetReferencesManager.Instance.HighlightProfileValid
         : AssetReferencesManager.Instance.HighlightProfileLocked;

      _highlightEffect.ProfileLoad(highlightProfile);
      _highlightEffect.SetHighlighted(true);
   }

   public virtual async UniTask BeginInteraction(IInteractor interactor)
   {
      await PreInteract(interactor);

      if (CanInteract)
      {
         await Interact(interactor);
      }
      else
      {
         await InteractInvalid(interactor);
      }

      await PostInteract(interactor);
      interactor.FinishInteraction();

      FinishInteract();
   }

   public virtual async UniTask Interact(IInteractor interactor)
   {
      Messaging.SendMessage(MessageType.CharacterInteractionStarted);

      if (_hasDialogue)
      {
         var appliedNodeName = SaveDataManager.GetAppliedNodeName(_nodeNameValid);
         if (string.IsNullOrEmpty(appliedNodeName) == false)
         {
            SingletonDialogueRunner.StartDialogue(appliedNodeName);
            await SingletonDialogueRunner.IsDialogueComplete();
         }
      }
   }

   public virtual async UniTask InteractInvalid(IInteractor interactor)
   {
      Messaging.SendMessage(MessageType.CharacterInteractionStarted);

      if (_hasDialogue)
      {
         var appliedNodeName = SaveDataManager.GetAppliedNodeName(_nodeNameInvalid);
         if (string.IsNullOrEmpty(appliedNodeName) == false)
         {
            SingletonDialogueRunner.StartDialogue(appliedNodeName);
            await SingletonDialogueRunner.IsDialogueComplete();
         }
      }
   }

   protected virtual async UniTask PreInteract(IInteractor interactor)
   {
      //Allow the actor to also PreInteract and await its completion
      Messaging.SendMessage(MessageType.CharacterPreInteracting);
      await interactor.PreInteract(this);
   }

   protected virtual UniTask PostInteract(IInteractor interactor)
   {
      return default;
   }

   protected virtual void FinishInteract()
   {
      if (_navMeshObstacle != null && _navMeshObstacleWasEnabled)
      {
         _navMeshObstacle.enabled = true;
      }
   }

   protected virtual void Uninspect()
   {
      IsInspected = false;
      _highlightEffect.SetHighlighted(false);
   }

}