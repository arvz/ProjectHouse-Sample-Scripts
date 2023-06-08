/*
 * Calls IAnimationEventReceiver.AnimationEventInvoke(AnimationEventType) at a specific frame during animation
 * Use this instead of standard Unity animation events which has limitations, and make code difficult to trace and debug
 */
using UnityEngine;

public class AnimationEventSMB : SceneLinkedSMB<IAnimationEventReceiver>
{
   [SerializeField] int _triggerFrame;
   [SerializeField] AnimationEventType _animationEventType;

   [InspectorPrivate] bool _canTrigger;
   int _lastLoopCount;

   public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
   {
      base.OnSLStateEnter(animator, stateInfo, layerIndex);
      _canTrigger = true;
      _lastLoopCount = 0;
   }

   public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
   {
      int currentLoopCount = Mathf.FloorToInt(stateInfo.normalizedTime);
      if (currentLoopCount > _lastLoopCount)
      {
         _lastLoopCount = currentLoopCount;
         _canTrigger = true;
      }

      if (_canTrigger == false || _smbLinkers == null || _smbLinkers.Count < 1)
      {
         return;
      }

      float currentTime = stateInfo.normalizedTime % 1;
      int totalFrames = Mathf.RoundToInt(stateInfo.length * animator.GetCurrentAnimatorClipInfo(layerIndex)[0].clip.frameRate);
      int currentFrame = Mathf.RoundToInt(currentTime * totalFrames);

      if (currentFrame >= _triggerFrame)
      {
         foreach (var animationReceiver in _smbLinkers)
         {
            animationReceiver.AnimationEventInvoke(_animationEventType);
            _canTrigger = false;
         }
      }
   }
}