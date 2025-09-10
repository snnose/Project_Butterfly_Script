using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FlowerBouquetSOData))]
public class FlowerBouquetDataImporter : GenericDataImporter<FlowerBouquetSOData, Bouquet, BouquetDTO>
{
    
}
