using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UserSOData))]
public class UserDataImporter : GenericDataImporter<UserSOData, User, UserDTO>
{
   
}
