using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DirectingEventSystem;

[CreateAssetMenu(fileName = "CameraRotateEventData", menuName = "DirectingEvent/Event Data/Camera Rotate")]
public class CameraRotateEventData : ScriptableObject
{
    public CameraRotateOption cameraRotateOption;
}
