using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T_SO"> Scriptable Object의 타입 </typeparam>
/// <typeparam name="T_Data"> 실제 데이터 타입 </typeparam>
/// <typeparam name="T_DTO"> DTO 데이터 타입 </typeparam>
public class GenericDataImporter<T_SO, T_Data, T_DTO> : Editor
    where T_SO : BaseSOData<T_Data>                   // T_SO는 BaseSOData<T_Data>를 상쇽해야한다.
    where T_Data : class, IDataFromDto<T_DTO>, new()  // T_Data는 클래스여야 하고, IDataFromDto를 구현하며, new()로 생성 가능해야한다.
    where T_DTO : class, IDto                         // T_Dto는 클래스여야 하고, IDto를 구현해야한다.
{
    [System.Serializable]
    private class JsonWrapper
    {
        public List<T_DTO> items;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // 현재 보고있는 Inspecter에서 데이터를 가져온다.
        T_SO data = (T_SO)target;

        // 버튼 추가
        if (GUILayout.Button("Import from JSON"))
        {
            // JSON 파일이 위치한 프로젝트 내 경로를 지정한다.
            // 경로 : Assets/JSON 폴더
            string path = Path.Combine(Application.dataPath, "JSON");

            // 위 경로가 존재하지 않는다면 Assets 폴더에서 시작한다.
            if (!Directory.Exists(path))
            {
                path = Application.dataPath;
            }

            string jsonPath = EditorUtility.OpenFilePanel($"Select {typeof(T_Data).Name} JSON", path, "json");

            if (string.IsNullOrEmpty(jsonPath))
            {
                return;
            }

            // Undo 기능을 위해 변경사항을 기록
            Undo.RecordObject(data, $"Import {typeof(T_Data).Name} from JSON");

            string jsonString = File.ReadAllText(jsonPath);

            string wrappedJson = "{ \"items\": " + jsonString + "}";
            JsonWrapper wrapper = JsonUtility.FromJson<JsonWrapper>(wrappedJson);

            if (wrapper == null || wrapper.items == null || wrapper.items.Count == 0)
            {
                Debug.LogError($"Failed to parse {typeof(T_Data).Name} from JSON. the list is empty or the format is incorrect.");
                return;
            }

            // 파싱한 데이터로 리스트를 업데이트한다.
            data.dataList = new List<T_Data>();
            foreach (var dto in wrapper.items)
            {
                T_Data newItem = new T_Data();  // new() 제약 조건으로 인해 가능
                newItem.FromDto(dto);           // IDataFromDto 제약 조건으로 인해 가능
                data.dataList.Add(newItem);
            }

            EditorUtility.SetDirty(data);

            Debug.Log($"Successfully imported {data.dataList.Count} achievements into {data.name}");
        }
    }
}
