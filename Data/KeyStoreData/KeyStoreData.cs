/*
 * a KeyStore is a key-value pair in the form of a scriptableobject.
 * It is used as part of the game's save data system, and is serializable to JSON for saving.
 * e.g. use case: KeyStore "$storyPhase" might have value "1" representing player's progress in the story.
 */
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public abstract class KeyStoreData : GameData
{
   [JsonProperty(Order = 0)]
   public override string Name => base.Name;

   [JsonProperty(Order = 1)]
   public abstract object Value { get; }

   void OnValidate()
   {
      Name = name;
   }
}

