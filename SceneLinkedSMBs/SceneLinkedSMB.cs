﻿/*
 * Base code taken from https://forum.unity.com/threads/extending-statemachinebehaviours.488314/
 * slightly modified by Arvin G
 */
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Animations;

public class SceneLinkedSMB<TSMBLinker> : SealedSMB where TSMBLinker : ISMBLinker
{
   protected List<TSMBLinker> _smbLinkers;

   bool _firstFrameHappened;
   bool _lastFrameHappened;

   public static void Initialise(Animator animator, TSMBLinker monoBehaviour)
   {
      animator.keepAnimatorStateOnDisable = true;
      SceneLinkedSMB<TSMBLinker>[] sceneLinkedSMBs = animator.GetBehaviours<SceneLinkedSMB<TSMBLinker>>();

      for (int i = 0; i < sceneLinkedSMBs.Length; i++)
      {
         sceneLinkedSMBs[i].InternalInitialise(animator, monoBehaviour);
      }
   }

   protected void InternalInitialise(Animator animator, TSMBLinker animationReceiver)
   {
      if (_smbLinkers == null)
      {
         _smbLinkers = new List<TSMBLinker>();
      }

      _smbLinkers.Add(animationReceiver);
      OnStart(animator);
   }

   public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
   {
      _firstFrameHappened = false;

      OnSLStateEnter(animator, stateInfo, layerIndex);
      OnSLStateEnter(animator, stateInfo, layerIndex, controller);
   }

   public sealed override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
   {
      if (!animator.gameObject.activeSelf)
         return;

      if (animator.IsInTransition(layerIndex) && animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash == stateInfo.fullPathHash)
      {
         OnSLTransitionToStateUpdate(animator, stateInfo, layerIndex);
         OnSLTransitionToStateUpdate(animator, stateInfo, layerIndex, controller);
      }

      if (!animator.IsInTransition(layerIndex) && _firstFrameHappened)
      {
         OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);
         OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex, controller);
      }

      if (animator.IsInTransition(layerIndex) && !_lastFrameHappened && _firstFrameHappened)
      {
         _lastFrameHappened = true;

         OnSLStatePreExit(animator, stateInfo, layerIndex);
         OnSLStatePreExit(animator, stateInfo, layerIndex, controller);
      }

      if (!animator.IsInTransition(layerIndex) && !_firstFrameHappened)
      {
         _firstFrameHappened = true;

         OnSLStatePostEnter(animator, stateInfo, layerIndex);
         OnSLStatePostEnter(animator, stateInfo, layerIndex, controller);
      }

      if (animator.IsInTransition(layerIndex) && animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash == stateInfo.fullPathHash)
      {
         OnSLTransitionFromStateUpdate(animator, stateInfo, layerIndex);
         OnSLTransitionFromStateUpdate(animator, stateInfo, layerIndex, controller);
      }
   }

   public sealed override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
   {
      _lastFrameHappened = false;

      OnSLStateExit(animator, stateInfo, layerIndex);
      OnSLStateExit(animator, stateInfo, layerIndex, controller);
   }

   /// <summary>
   /// Called by a MonoBehaviour in the scene during its Start function.
   /// </summary>
   public virtual void OnStart(Animator animator) { }

   /// <summary>
   /// Called before Updates when execution of the state first starts (on transition to the state).
   /// </summary>
   public virtual void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

   /// <summary>
   /// Called after OnSLStateEnter every frame during transition to the state.
   /// </summary>
   public virtual void OnSLTransitionToStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

   /// <summary>
   /// Called on the first frame after the transition to the state has finished.
   /// </summary>
   public virtual void OnSLStatePostEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

   /// <summary>
   /// Called every frame after PostEnter when the state is not being transitioned to or from.
   /// </summary>
   public virtual void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

   /// <summary>
   /// Called on the first frame after the transition from the state has started.  Note that if the transition has a duration of less than a frame, this will not be called.
   /// </summary>
   public virtual void OnSLStatePreExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

   /// <summary>
   /// Called after OnSLStatePreExit every frame during transition to the state.
   /// </summary>
   public virtual void OnSLTransitionFromStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

   /// <summary>
   /// Called after Updates when execution of the state first finshes (after transition from the state).
   /// </summary>
   public virtual void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

   /// <summary>
   /// Called before Updates when execution of the state first starts (on transition to the state).
   /// </summary>
   public virtual void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller) { }

   /// <summary>
   /// Called after OnSLStateEnter every frame during transition to the state.
   /// </summary>
   public virtual void OnSLTransitionToStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller) { }

   /// <summary>
   /// Called on the first frame after the transition to the state has finished.
   /// </summary>
   public virtual void OnSLStatePostEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller) { }

   /// <summary>
   /// Called every frame when the state is not being transitioned to or from.
   /// </summary>
   public virtual void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller) { }

   /// <summary>
   /// Called on the first frame after the transition from the state has started.  Note that if the transition has a duration of less than a frame, this will not be called.
   /// </summary>
   public virtual void OnSLStatePreExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller) { }

   /// <summary>
   /// Called after OnSLStatePreExit every frame during transition to the state.
   /// </summary>
   public virtual void OnSLTransitionFromStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller) { }

   /// <summary>
   /// Called after Updates when execution of the state first finshes (after transition from the state).
   /// </summary>
   public virtual void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller) { }
}

//This class repalce normal StateMachineBehaviour. It add the possibility of having direct reference to the object
//the state is running on, avoiding the cost of retrienving it through a GetComponent every time.
//c.f. Documentation for more in depth explainations.
public abstract class SealedSMB : SerializedStateMachineBehaviour
{
   public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

   public sealed override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

   public sealed override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
}