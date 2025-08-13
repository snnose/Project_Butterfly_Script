using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DirectingEventSystem;

[CreateAssetMenu(fileName = "ObjectMoveEventData", menuName = "DirectingEvent/Event Data/Object Move")]

public class ObjectMoveEventData : ScriptableObject
{
    public ObjectMoveOption objectMoveOption;
}
