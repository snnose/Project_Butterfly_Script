using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DirectingEventSystem;

[CreateAssetMenu(fileName = "ObjectInstantiateEventData", menuName = "DirectingEvent/Event Data/Object Instantiate")]
public class ObjectInstantiateEventData : ScriptableObject
{
    public ObjectInstantiateOption objectInstantiateOption;
}
