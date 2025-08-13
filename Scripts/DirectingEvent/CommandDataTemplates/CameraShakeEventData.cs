using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DirectingEventSystem;

[CreateAssetMenu(fileName = "CameraShakeEventData", menuName = "DirectingEvent/Event Data/Camera Shake")]
public class CameraShakeEventData : ScriptableObject
{
    public CameraShakeOption cameraShakeOption;
}
