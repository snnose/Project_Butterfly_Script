using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace DirectingEventSystem
{
    [CustomEditor(typeof(DirectingEventZone))]
    public class DirectingEventZoneEditor : Editor
    {
        private Dictionary<object, Editor> editorCache = new Dictionary<object, Editor>();
        SerializedProperty directingEventProperty;
        private ReorderableList reorderableList;

        private void Awake()
        {
            
        }

        private void OnEnable()
        {
            directingEventProperty = serializedObject.FindProperty("directingEventList");

            reorderableList = new ReorderableList(serializedObject, directingEventProperty, true, true, true, true);

            // 리스트의 헤더를 그리는 방법을 정의한다.
            reorderableList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "연출 이벤트 순서");
            };

            // 리스트의 각 요소를 그리는 방법을 정의한다.
            reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                // 현재 인덱스에 해당하는 리스트 요소를 가져온다.
                SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);

                /*
                // Rect 설정     
                rect.y += 2;
                rect.height = EditorGUIUtility.singleLineHeight;

                // 리스트 내 드래그를 위해 왼쪽에 15픽셀 정도의 여백을 준다.
                // 여백의 오른쪽에 PropertyField를 그리도록 새 Rect 계산.
                float indent = 15f;
                Rect contentRect = new Rect(rect.x + indent, rect.y, rect.width - indent, rect.height);

                EditorGUI.PropertyField(contentRect, element, GUIContent.none, false);

                element.isExpanded = EditorGUI.Foldout(rect, element.isExpanded, GUIContent.none, true);

                if (element.isExpanded)
                {
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width, rect.height), element, true);
                }
                */
                // 요소의 제목을 정한다.
                // ex) DirectingCameraMove => CameraMove
                string title = "빈 이벤트";
                if (element.managedReferenceValue != null)
                {
                    title = element.managedReferenceValue.GetType().Name.Replace("Directing", "");
                }

                // 정한 제목으로 각 이벤트를 그려준다.
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUI.GetPropertyHeight(element, true)), element, new GUIContent(title), true);
            };

            // 각 요소의 높이를 계산하는 방법을 정의한다. (요소가 많아지면 자동으로 높이가 늘어나도록)
            reorderableList.elementHeightCallback = (int index) =>
            {
                SerializedProperty element = reorderableList.serializedProperty.GetArrayElementAtIndex(index);

                /*
                if (element.isExpanded)
                {
                    return EditorGUI.GetPropertyHeight(element, true) + EditorGUIUtility.standardVerticalSpacing;
                }
                else
                {
                    return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                }
                */
                return EditorGUI.GetPropertyHeight(element, true) + EditorGUIUtility.standardVerticalSpacing;
            };

            // '+' 버튼을 눌렀을 때 동작 정의
            reorderableList.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
            {
                ShowAddEventMenu();
            };
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
            //DrawDefaultInspector();

            DrawCustomInspector();
            DrawAddEventButton();

            //DrawEventOption();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawCustomInspector()
        {
            // Draw isActivated
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isActivated"));
            EditorGUILayout.Space();

            // Draw Progress
            EditorGUILayout.PropertyField(serializedObject.FindProperty("mainProgress"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("subProgress"));
            EditorGUILayout.Space();

            // Draw Collider
            EditorGUILayout.PropertyField(serializedObject.FindProperty("zoneCollider"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("zoneSize"));
            EditorGUILayout.Space();

            reorderableList.DoLayoutList();
            EditorGUILayout.Space();
        }

        /*
        // MonoBehaviour, Scriptable Object 대상으로 유효한 함수.
        // 리스트에 추가된 대상의 세부 옵션을 설정하기 위함.
        private void DrawEventOption()
        {
            for (int i = 0; i < directingEventProperty.arraySize; i++)
            {
                SerializedProperty element = directingEventProperty.GetArrayElementAtIndex(i);
                
                EditorGUILayout.BeginVertical(GUI.skin.box);

                // Scriptable Object를 할당하는 필드를 그린다.
                EditorGUILayout.PropertyField(element, new GUIContent($"Event {i}"));

                if (element.managedReferenceValue != null)
                {
                    // "Edit Details" 라는 이름의 펼치기/접기 UI를 만든다.
                    // isExpanded는 각 리스트 요소가 가진 숨겨진 속성으로, 펼침 상태를 기억한다.
                    element.isExpanded = EditorGUILayout.Foldout(element.isExpanded, "Edit Details", true);

                    // 펼쳐진 상태라면
                    if (element.isExpanded)
                    {
                        if (!editorCache.ContainsKey(element.managedReferenceValue))
                        {
                            editorCache[element.managedReferenceValue] = Editor.CreateEditor(element.objectReferenceValue);
                        }

                        var editor = editorCache[element.managedReferenceValue];

                        EditorGUI.indentLevel++;
                        editor.OnInspectorGUI();
                        EditorGUI.indentLevel--;
                    }
                }

                EditorGUILayout.EndVertical();
            }
        }
        */
        /// <summary>
        /// "Add New Directing Event" 버튼을 그리고, 클릭 시 메뉴를 띄운다.
        /// </summary>
        private void DrawAddEventButton()
        {
            EditorGUILayout.Space();

            var button = GUILayout.Button("Add New Directing Event");

            if (button)
            {
                ShowAddEventMenu();
            }
        }

        private void ShowAddEventMenu()
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
                menu.AddItem(new GUIContent(menuPath), false, () =>
                {
                    // 선택된 타입의 인스턴스 생성 (CameraMove, CameraRotate, ...)
                    DirectingEvent directingEvent = (DirectingEvent)Activator.CreateInstance(type);

                    int newIndex = directingEventProperty.arraySize;
                    directingEventProperty.InsertArrayElementAtIndex(newIndex);
                    SerializedProperty newElement = directingEventProperty.GetArrayElementAtIndex(newIndex);
                    newElement.managedReferenceValue = directingEvent;
                    serializedObject.ApplyModifiedProperties();
                });
            }

            // 메뉴를 현재 마우스 위치에 표시
            menu.ShowAsContext();
        }

        /// <summary>
        /// (사용 X) Unity 프로젝트 창에서 현재 선택된 폴더의 경로를 반환합니다.
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