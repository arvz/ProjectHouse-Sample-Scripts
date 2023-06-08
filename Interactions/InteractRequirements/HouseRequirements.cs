using System.Collections.Generic;
using Sirenix.Serialization;

[System.Serializable]
public struct HouseRequirements
{
   [OdinSerialize] public List<InteractRequirement> Requirements { get; private set; }

   public bool IsRequirementsMet()
   {
      if (Requirements == null) return true;

      foreach (var requirement in Requirements)
      {
         if (requirement.IsRequirementMet() == false)
         {
            return false;
         }
      }

      return true;
   }
}
