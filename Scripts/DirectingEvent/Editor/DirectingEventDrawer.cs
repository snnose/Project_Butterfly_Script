using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace DirectingEventSystem
{
    [CustomPropertyDrawer(typeof(DirectingEvent), true)]
    public class DirectingEventDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect currentRect = new Rect(position.x + 15f, position.y, position.width - 15f, EditorGUIUtility.singleLineHeight);

            // DirectingEventData 스크립터블 오브젝트를 찾는다.
            var templateProperty = property.FindPropertyRelative("dataTemplate");

            // 템플릿 필드가 존재하고, 무언가 할당 됐다면
            if (templateProperty != null && templateProperty.objectReferenceValue != null)
            {
                // 탬플릿 에셋의 이름을 가져와 라벨로 사용한다.
                label.text = templateProperty.objectReferenceValue.name;
            }

            // 1. Draw Option Property 
            float propertyHeight = EditorGUI.GetPropertyHeight(property, label, true);
            currentRect.height = propertyHeight;
            // 계산된 위치와 높이로 그린다.
            EditorGUI.PropertyField(currentRect, property, label, true);

            // 다음 UI를 그리기 위해 Y 위치 이동.
            currentRect.y += propertyHeight + EditorGUIUtility.standardVerticalSpacing;

            // 2. "Load from Template" 버튼을 추가한다.
            // templateProperty가 존재하고, 할당됐을 때만 버튼을 보여준다.
            AddLoadFromTemplateButton(currentRect, property, templateProperty);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float totalHeight = EditorGUI.GetPropertyHeight(property, label, true);

            // 만약 펼쳐져 있고, 템플릿이 할당됐다면 버튼의 높이와 간격을 추가한다.
            var templateProperty = property.FindPropertyRelative("dataTemplate");
            if (property.isExpanded && templateProperty != null && templateProperty.objectReferenceValue != null)
            {
                totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            return totalHeight;
        }

        private void AddLoadFromTemplateButton(Rect currentRect, SerializedProperty property, SerializedProperty templateProperty)
        {
            //SerializedProperty optionProperty =

            if (property.isExpanded && templateProperty != null && templateProperty.objectReferenceValue != null)
            {
                // 버튼의 Rect 계산.
                // 15f << 여백을 주기 위한 값.
                Rect buttonRect = new Rect(currentRect.x + 15f, currentRect.y, currentRect.width - 15f, EditorGUIUtility.singleLineHeight);

                if (GUI.Button(buttonRect, "Load Data from Template"))
                {
                    // SerializedObject를 사용해 템플릿의 데이터를 현재 이벤트로 복사한다.
                    SerializedObject templateObject = new SerializedObject(templateProperty.objectReferenceValue);

                    // 템플릿의 모든 프로퍼티 순회
                    SerializedProperty iterator = templateObject.GetIterator();
                    iterator.NextVisible(true);

                    while (iterator.NextVisible(false))
                    {
                        LoadFromTemplate(property, templateProperty.objectReferenceValue);
                    }
                }
            }
        }

        private void LoadFromTemplate(SerializedProperty destEventProperty, Object templateAsset)
        {
            // SerializedObject를 사용해 템플릿의 데이터를 현재 이벤트로 복사한다.
            SerializedObject templateObject = new SerializedObject(templateAsset);

            SerializedProperty destOptionProperty = destEventProperty.FindPropertyRelative("optionData");

            if (destOptionProperty == null)
            {
                Debug.LogError($"'{destEventProperty.displayName}'에서 optionData 필드를 찾을 수 없습니다!");
                return;
            }

            string targetTypeName = destOptionProperty.type;
            FieldInfo sourceField = templateAsset.GetType().GetFields()
                .FirstOrDefault(field => field.FieldType.Name == targetTypeName);

            if (sourceField == null)
            {
                Debug.LogError($"템플릿 에셋 '{templateAsset.name}'에서 '{targetTypeName}' 타입을 가진 필드를 찾을 수 없습니다!");
                return;
            }

            SerializedProperty sourceProperty = templateObject.FindProperty(sourceField.Name);
            Debug.LogWarning($"탬플릿 에셋 복사할 타입의 필드 이름 : {sourceField.Name}");

            // 위 모든 과정을 거쳤다면 복사 함수 호출
            CopySerializedProperty(destOptionProperty, sourceProperty);
            destEventProperty.serializedObject.ApplyModifiedProperties();

            // Attribute를 활용해 Option을 찾는 방식. 잘 작동하지 않아서 주석 처리.
            /*
            // 현재 프로퍼티의 실제 C# 객체를 가져온다.
            object destEventObject = destEventProperty.managedReferenceValue;
            if (destEventObject == null)
            {
                Debug.LogError("destEventObject의 값이 null 입니다. [SerializeReference]를 사용 중일 때 잘 작동합니다.");
                return;
            }

            // Reflection을 사용해 [DirectingEventOption] 어트리뷰트가 붙은 필드를 찾는다.
            FieldInfo destOptionField = templateAsset.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(field => field.IsDefined(typeof(DirectingEventOptionAttribute), false));

            if (destOptionField == null)
            {
                Debug.LogError($"'{destEventObject.GetType().Name}' 클래스에서 [DirectingEventOption] Attribute를 가진 필드를 찾을 수 없습니다!");
                return;
            }

            // 찾은 필드의 이름을 사용해 SerializedProperty를 가져온다.
            SerializedProperty destOptionProperty = destEventProperty.FindPropertyRelative(destOptionField.Name);
            string targetTypeName = destOptionField.FieldType.Name;
            */
        }

        private void CopySerializedProperty(SerializedProperty dest, SerializedProperty source)
        {
            // 커스텀 구조체(Generic 타입)의 경우 재귀 호출한다.
            if (source.hasChildren && source.propertyType == SerializedPropertyType.Generic)
            {
                // 자식 프로퍼티들을 순회하기 위한 이터레이터를 복제
                SerializedProperty sourceIterator = source.Copy();
                SerializedProperty endProperty = source.GetEndProperty();

                // 첫 번째 자식으로 이동한다.
                sourceIterator.NextVisible(true);

                // 구조체의 끝에 도달할 때까지 자식 프로퍼티를 순회한다.
                while (!SerializedProperty.EqualContents(sourceIterator, endProperty))
                {
                    SerializedProperty destProperty = dest.FindPropertyRelative(sourceIterator.name);

                    // 자식 프로퍼티를 인수로 주고 재귀
                    if (destProperty != null)
                    {
                        CopySerializedProperty(destProperty, sourceIterator);
                    }

                    if (!sourceIterator.NextVisible(false))
                        break;
                }
            }
            // 단일 속성은 바로 값을 복사한다.
            else
            {
                CopyValue(dest, source);
            }
        }

        private void CopyValue(SerializedProperty dest, SerializedProperty source)
        {
            switch (source.propertyType)
            {
                case SerializedPropertyType.Float:
                    dest.floatValue = source.floatValue;
                    break;
                case SerializedPropertyType.Integer:
                    dest.intValue = source.intValue;
                    break;
                case SerializedPropertyType.String:
                    dest.stringValue = source.stringValue;
                    break;
                case SerializedPropertyType.Boolean:
                    dest.boolValue = source.boolValue;
                    break;
                case SerializedPropertyType.Vector3:
                    dest.vector3Value = source.vector3Value;
                    break;
                case SerializedPropertyType.Vector2:
                    dest.vector2Value = source.vector2Value;
                    break;
                case SerializedPropertyType.Color:
                    dest.colorValue = source.colorValue;
                    break;
                case SerializedPropertyType.Enum:
                    dest.enumValueIndex = source.enumValueIndex;
                    break;
                default:
                    Debug.LogWarning($"Copying for type {source.propertyType} is notimplemented.");
                    break;
            }
        }
    }
}