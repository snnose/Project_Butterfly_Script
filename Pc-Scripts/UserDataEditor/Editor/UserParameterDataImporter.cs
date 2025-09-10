using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UserParameterSOData))]
public class UserParameterDataImporter : GenericDataImporter<UserParameterSOData, UserParameter, UserParameterDTO>
{
    
}
