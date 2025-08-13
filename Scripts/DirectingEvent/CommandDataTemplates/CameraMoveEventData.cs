using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DirectingEventSystem;

[CreateAssetMenu(fileName = "CameraMoveEventData", menuName = "DirectingEvent/Event Data/Camera Move")]
public class CameraMoveEventData : ScriptableObject
{
    public CameraMoveOption cameraMoveOption;
}
