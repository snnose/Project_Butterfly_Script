using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DirectingEventSystem
{
    [CustomEditor(typeof(DirectingEventZone))]
    public class DirectingEventEditor : Editor
    {
        private Dictionary<UnityEngine.Object, Editor> editorCache = new Dictionary<UnityEngine.Object, Editor>();
        SerializedProperty directingEventProperty;

        private void OnEnable()
        {
            directingEventProperty = serializedObject.FindProperty("directingEventList");
        }

        private void OnDisable()
        {
            foreach (var editor in editorCache.Values)
            {
                if (editor != null)
                    DestroyImmediate(editor);
            }

            editorCache.Clear();
        }

        public override void OnInspectorGUI()
        {
            // 기본 인스펙터 그리기
            DrawDefaultInspector();

            DrawEventOption();
            DrawAddEventButton();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawEventOption()
        {
            for (int i = 0; i < directingEventProperty.arraySize; i++)
            {
                SerializedProperty element = directingEventProperty.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginVertical(GUI.skin.box);

                // ScriptableObject를 할당하는 필드를 그린다.
                EditorGUILayout.PropertyField(element, new GUIContent($"Event {i}"));

                if (element.objectReferenceValue != null)
                {
                    // "Edit Details" 라는 이름의 펼치기/접기 UI를 만든다.
                    // isExpanded는 각 리스트 요소가 가진 숨겨진 속성으로, 펼침 상태를 기억한다.
                    element.isExpanded = EditorGUILayout.Foldout(element.isExpanded, "Edit Details", true);

                    // 펼쳐진 상태라면
                    if (element.isExpanded)
                    {
                        if (!editorCache.ContainsKey(element.objectReferenceValue))
                        {
                            editorCache[element.objectReferenceValue] = Editor.CreateEditor(element.objectReferenceValue);
                        }

                        var editor = editorCache[element.objectReferenceValue];

                        EditorGUI.indentLevel++;
                        editor.OnInspectorGUI();
                        EditorGUI.indentLevel--;
                    }
                }

                EditorGUILayout.EndVertical();
            }
        }

        /// <summary>
        /// "Add New Directing Event" 버튼을 그리고, 클릭 시 메뉴를 띄운다.
        /// </summary>
        private void DrawAddEventButton()
        {
            EditorGUILayout.Space();

            var button = GUILayout.Button("Add New Directing Event");

            if (button)
            {
                GenericMenu menu = new GenericMenu();

                // Reflection을 사용해 DirectingEvent를 상속받는 모든 생성 가능한(추상이 아닌) 클래스 타입을 찾는다.
                var eventTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(type => type.IsSubclassOf(typeof(DirectingEvent)) && !type.IsAbstract);

                // 찾은 각 타입에 대해 메뉴 항목 추가
                foreach (var type in eventTypes)
                {
                    // 메뉴에 표시될 이름 설정
                    string menuPath = ObjectNames.NicifyVariableName(type.Name);
                    menu.AddItem(new GUIContent(menuPath), false, () => CreateAndAddEvent(type));
                }

                // 메뉴를 현재 마우스 위치에 표시
                menu.ShowAsContext();
            }
        }

        private void CreateAndAddEvent(Type eventType)
        {
            // ScriptableObject 인스턴스 생성
            var newEventInstance = ScriptableObject.CreateInstance(eventType);
            newEventInstance.name = "New " + eventType.Name;

            // 에셋을 저장할 경로를 지정하고, 폴더가 없으면 생성
            string folderPath = GetCurrentProjectFolderPath();
            string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/{newEventInstance.name}.asset");

            AssetDatabase.CreateAsset(newEventInstance, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            int newIndex = directingEventProperty.arraySize;
            directingEventProperty.InsertArrayElementAtIndex(newIndex);
            SerializedProperty newElement = directingEventProperty.GetArrayElementAtIndex(newIndex);
            newElement.objectReferenceValue = AssetDatabase.LoadAssetAtPath<DirectingEvent>(assetPath);
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Unity 프로젝트 창에서 현재 선택된 폴더의 경로를 반환합니다.
        /// </summary>
        /// <returns>선택된 폴더의 경로. 유효 폴더가 아니면 "Assets"를 반환합니다.</returns>
        private string GetCurrentProjectFolderPath()
        {
            string path = "Assets"; // 기본 경로는 Assets 폴더

            foreach (var obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);

                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    // 선택된 것이 파일이라면, 해당 파일이 속한 디렉토리 경로를 사용
                    path = Path.GetDirectoryName(path);
                }
                break;
            }

            return path;
        }
    }
}