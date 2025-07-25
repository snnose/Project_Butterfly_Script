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
            // �⺻ �ν����� �׸���
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

                // ScriptableObject�� �Ҵ��ϴ� �ʵ带 �׸���.
                EditorGUILayout.PropertyField(element, new GUIContent($"Event {i}"));

                if (element.objectReferenceValue != null)
                {
                    // "Edit Details" ��� �̸��� ��ġ��/���� UI�� �����.
                    // isExpanded�� �� ����Ʈ ��Ұ� ���� ������ �Ӽ�����, ��ħ ���¸� ����Ѵ�.
                    element.isExpanded = EditorGUILayout.Foldout(element.isExpanded, "Edit Details", true);

                    // ������ ���¶��
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
        /// "Add New Directing Event" ��ư�� �׸���, Ŭ�� �� �޴��� ����.
        /// </summary>
        private void DrawAddEventButton()
        {
            EditorGUILayout.Space();

            var button = GUILayout.Button("Add New Directing Event");

            if (button)
            {
                GenericMenu menu = new GenericMenu();

                // Reflection�� ����� DirectingEvent�� ��ӹ޴� ��� ���� ������(�߻��� �ƴ�) Ŭ���� Ÿ���� ã�´�.
                var eventTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(type => type.IsSubclassOf(typeof(DirectingEvent)) && !type.IsAbstract);

                // ã�� �� Ÿ�Կ� ���� �޴� �׸� �߰�
                foreach (var type in eventTypes)
                {
                    // �޴��� ǥ�õ� �̸� ����
                    string menuPath = ObjectNames.NicifyVariableName(type.Name);
                    menu.AddItem(new GUIContent(menuPath), false, () => CreateAndAddEvent(type));
                }

                // �޴��� ���� ���콺 ��ġ�� ǥ��
                menu.ShowAsContext();
            }
        }

        private void CreateAndAddEvent(Type eventType)
        {
            // ScriptableObject �ν��Ͻ� ����
            var newEventInstance = ScriptableObject.CreateInstance(eventType);
            newEventInstance.name = "New " + eventType.Name;

            // ������ ������ ��θ� �����ϰ�, ������ ������ ����
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
        /// Unity ������Ʈ â���� ���� ���õ� ������ ��θ� ��ȯ�մϴ�.
        /// </summary>
        /// <returns>���õ� ������ ���. ��ȿ ������ �ƴϸ� "Assets"�� ��ȯ�մϴ�.</returns>
        private string GetCurrentProjectFolderPath()
        {
            string path = "Assets"; // �⺻ ��δ� Assets ����

            foreach (var obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
            {
                path = AssetDatabase.GetAssetPath(obj);

                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    // ���õ� ���� �����̶��, �ش� ������ ���� ���丮 ��θ� ���
                    path = Path.GetDirectoryName(path);
                }
                break;
            }

            return path;
        }
    }
}