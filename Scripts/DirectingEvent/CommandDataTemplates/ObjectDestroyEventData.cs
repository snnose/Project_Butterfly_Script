using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DirectingEventSystem;

[CreateAssetMenu(fileName = "ObjectDestroyEventData", menuName = "DirectingEvent/Event Data/Object Destroy")]

public class ObjectDestroyEventData : ScriptableObject
{
    public ObjectDestroyOption objectDestroyOption;
}
