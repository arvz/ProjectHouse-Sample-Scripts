using Enums;
using UnityEngine;

public class CharacterAnimationStateSMB : SceneLinkedSMB<CharacterInteractions>
{
   [SerializeField] CharacterAnimationState _state;

   //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
   public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
   {
      if (Application.isPlaying == false) return;

      if (_smbLinkers == null)
      {
         Messaging.SendMessage(MessageType.ReinitializeSLSMB);
      }

      if (_smbLinkers == null)
      {
         return;
      }

      foreach (var characterInteraction in _smbLinkers)
      {
         characterInteraction.SetState(_state);
      }
   }
}