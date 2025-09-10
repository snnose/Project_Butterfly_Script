using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(OptionSOData))]
public class OptionDataImporter : GenericDataImporter<OptionSOData, Option, OptionDTO>
{
    
}
