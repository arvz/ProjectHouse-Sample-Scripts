using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[InlineEditor, Serializable]
public abstract class GameData : SerializedScriptableObject
{
    [BoxGroup("Basic Info"), OdinSerialize, LabelWidth(100), PropertyOrder(-100)]
    public virtual string Name { get; protected set; }

    [BoxGroup("Basic Info"), OdinSerialize, LabelWidth(100), MultiLineProperty]
    public string Description { get; private set; }
}

public abstract class GameDataWithIcon : GameData
{
   [BoxGroup("Basic Info"), AssetsOnly, OdinSerialize, LabelWidth(100)]
   [PreviewField(75, ObjectFieldAlignment.Left, Height = 100f)]
   public Sprite Icon { get; private set; }
}
