using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AchievementSOData))]
public class AchievementDataImporter : Editor
{
    [System.Serializable]
    private class JsonWrapper
    {
        public List<AchievementsDTO> items;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        // 현재 인스펙터에서 보고 있는 AchievementSOData를 가져온다.
        AchievementSOData data = (AchievementSOData)target;

        // 버튼 추가
        if (GUILayout.Button("Import from JSON"))
        {
            // JSON 파일이 위치한 프로젝트 내 경로를 지정한다.
            // 경로 : Assets/JSON/Internal 폴더
            string path = Path.Combine(Application.dataPath, "JSON", "Internal");

            // 위 경로가 존재하지 않는다면 Assets 폴더에서 시작한다.
            if (!Directory.Exists(path))
            {
                path = Application.dataPath;
            }

            string jsonPath = EditorUtility.OpenFilePanel("Select Achievements JSON", path, "json");

            if (string.IsNullOrEmpty(jsonPath))
            {
                return;
            }

            // Undo 기능을 위해 변경사항을 기록
            Undo.RecordObject(data, "Import Achievements from JSON");

            string jsonString = File.ReadAllText(jsonPath);

            string wrappedJson = "{ \"items\": " + jsonString + "}";
            JsonWrapper wrapper = JsonUtility.FromJson<JsonWrapper>(wrappedJson);

            if (wrapper == null || wrapper.items == null || wrapper.items.Count == 0)
            {
                Debug.LogError("Failed to parse achievements from JSON. the list is empty or the format is incorrect.");
                return;
            }

            // 파싱한 데이터로 리스트를 업데이트한다.
            data.achievementDataList = new List<Achievements>();
            foreach (var dto in wrapper.items)
            {
                data.achievementDataList.Add(new Achievements(dto));
            }

            EditorUtility.SetDirty(data);

            Debug.Log($"Successfully imported {data.achievementDataList.Count} achievements into {data.name}");
        }
    }
}
