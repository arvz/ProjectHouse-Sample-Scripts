using System.Collections.Generic;
using UnityEngine;

public class CharacterSounds : MonoBehaviour, IAnimationEventReceiver
{
   [SerializeField] List<AudioClip> _audioClips;
   Animator _animator;

   void Awake()
   {
      _animator = GetComponentInChildren<Animator>();
   }

   public void LinkSMB()
   {
      SceneLinkedSMB<IAnimationEventReceiver>.Initialise(_animator, this);
   }

   public void AnimationEventInvoke(AnimationEventType animationEvent)
   {
      if (animationEvent == AnimationEventType.Footstep)
      {
         var randomClip = _audioClips.GetRandom();
         AudioManager.Instance.PlaySFX(randomClip);
      }
   }
}