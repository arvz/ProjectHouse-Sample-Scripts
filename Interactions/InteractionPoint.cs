using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class InteractionPoint : MonoBehaviour
{
   InteractHandler InteractHandler => _interactHandler != null ? _interactHandler : _interactHandler = GetComponentInParent<InteractHandler>();

   public bool HandInteraction;

   [InspectorPrivate] InteractHandler _interactHandler;

   void OnDrawGizmos()
   {
      if (HandInteraction)
      {
         Gizmos.DrawIcon(transform.position, "handIcon.png", true);
      }
      else
      {
         DrawInteractionPointGizmo();
      }
   }

   void DrawInteractionPointGizmo()
   {
      if (InteractHandler != null)
      {
         Debug.DrawLine(transform.position, InteractHandler.transform.position, Color.white);
      }

      DebugExtension.DrawCircle(transform.position, Vector3.up, Color.cyan, 0.1f);
      DebugExtension.DrawArrow(transform.position, transform.forward / 3f, Color.cyan);
   }
}