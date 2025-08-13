using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DirectingEventSystem;

[CreateAssetMenu(fileName = "ObjectScaleChangeEventData", menuName = "DirectingEvent/Event Data/Object Scale Change")]
public class ObjectScaleChangeEventData : ScriptableObject
{
    public ObjectScaleChangeOption objectScaleChangeOption;
}
