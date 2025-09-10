using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ConstantSOData))]
public class ConstantDataImporter : GenericDataImporter<ConstantSOData, Constant, ConstantDTO>
{
    
}
