using System;
using UnityEngine;

[Serializable]
public abstract class InteractRequirement
{
   public abstract bool IsRequirementMet();
}

public abstract class InteractRequirementKeyStore<T> : InteractRequirement where T : KeyStoreData
{
   public T KeyStoreData;
}


public class InteractRequirementItem : InteractRequirement
{
   public ItemData ItemData;

   public override bool IsRequirementMet()
   {
      return ItemsManager.Instance.OwnedItems.Contains(ItemData);
   }
}

public class InteractRequirementKeyStoreBool : InteractRequirementKeyStore<BoolKeyStoreData>
{
   public bool RequiredBoolValue;

   public override bool IsRequirementMet()
   {
      if (KeyStoreData == null) return true;
      return KeyStoreData.BoolValue == RequiredBoolValue;
   }
}

public class InteractRequirementKeyStoreFloat : InteractRequirementKeyStore<FloatKeyStoreData>
{
   public int RequiredFloatValue;

   public override bool IsRequirementMet()
   {
      if (KeyStoreData == null) return true;
      return Math.Abs(KeyStoreData.FloatValue - RequiredFloatValue) <= 0.0001f;
   }
}

public class InteractRequirementKeyStoreString : InteractRequirementKeyStore<StringKeyStoreData>
{
   public string RequiredStringValue;

   public override bool IsRequirementMet()
   {
      if (KeyStoreData == null) return true;
      return KeyStoreData.StringValue.Equals(RequiredStringValue);
   }
}