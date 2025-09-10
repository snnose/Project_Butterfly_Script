using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BoardSOData))]
public class BoardDataImporter : GenericDataImporter<BoardSOData, Board, BoardDTO>
{
    
}
