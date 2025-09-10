using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogueSOData))]
public class DialogueDataImporter : GenericDataImporter<DialogueSOData, Dialogue, DialogueDTO>
{
    
}
