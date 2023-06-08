using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Interact Handler that plays an animation on the Interactor
/// </summary>
public class AnimationInteractHandler : InteractHandler
{
   [FoldoutGroup("Character Animation"), PropertyOrder(-10)]
   public CharacterAnimationState characterAnimationState;

   [FoldoutGroup("Interaction Point")]
   [ShowIf("characterAnimationState", CharacterAnimationState.InteractHand)]
   [HorizontalGroup("Interaction Point/HandInteractPointHorizontal")]
   public Transform HandInteractionPoint;

   //Used in Odin attribute
   bool ShowCreateButton => characterAnimationState == CharacterAnimationState.InteractHand && HandInteractionPoint == null;

   public override async UniTask Interact(IInteractor interactor)
   {
      await base.Interact(interactor);
      await interactor.PlayAnimation(characterAnimationState);
   }

   [FoldoutGroup("Interaction Point")]
   [ShowIf("ShowCreateButton")]
   [HorizontalGroup("Interaction Point/HandInteractPointHorizontal")]
   [Button("Create", ButtonHeight = 20)]
   public void CreateHandInteractPoint()
   {
      var newinteractionPoint = new GameObject($"{transform.name} Hand Interact Point", typeof(InteractionPoint));
      newinteractionPoint.transform.parent = transform;
      newinteractionPoint.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
      newinteractionPoint.GetComponent<InteractionPoint>().HandInteraction = true;
      HandInteractionPoint = newinteractionPoint.transform;

#if UNITY_EDITOR
      Selection.activeTransform = HandInteractionPoint.transform;
#endif
   }
}