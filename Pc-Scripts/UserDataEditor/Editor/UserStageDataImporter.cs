using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UserStageSOData))]
public class UserStageDataImporter : GenericDataImporter<UserStageSOData, UserStage, UserStageDTO>
{
    
}
