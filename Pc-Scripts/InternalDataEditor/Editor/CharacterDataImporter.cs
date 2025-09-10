using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CharacterSOData))]
public class CharacterDataImporter : GenericDataImporter<CharacterSOData, Character, CharacterDTO>
{
    
}
