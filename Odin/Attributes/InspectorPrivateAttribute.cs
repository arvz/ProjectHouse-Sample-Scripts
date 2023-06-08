using System;
using UnityEngine;
using Sirenix.OdinInspector;

[IncludeMyAttributes]
[BoxGroup("Read Only"), ShowInInspector, ReadOnly]
[PropertyOrder(999)]
public class InspectorPrivateAttribute : Attribute { }
