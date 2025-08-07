using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(TweenAction))]
public class TweenActionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float y = position.y;

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // 공통 필드 그리기
        SerializedProperty startPosProp = property.FindPropertyRelative("startPosition");
        SerializedProperty typeProp = property.FindPropertyRelative("animationType");
        SerializedProperty durationProp = property.FindPropertyRelative("duration");
        SerializedProperty delayProp = property.FindPropertyRelative("delay");
        SerializedProperty runWithPreviousProp = property.FindPropertyRelative("runWithPrevious");

        Rect startPosRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
        // 다음 줄로 이동
        y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        // 한 줄에 여러 필드를 배치하기 위한 Rect 계산
        Rect typeRect = new Rect(position.x, y, position.width * 0.5f - 5, EditorGUIUtility.singleLineHeight);
        Rect durationRect = new Rect(position.x + position.width * 0.5f, y, position.width * 0.5f, EditorGUIUtility.singleLineHeight);
        // 다음 줄로 이동
        y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        Rect delayRect = new Rect(position.x, y, position.width * 0.5f - 5, EditorGUIUtility.singleLineHeight);
        Rect runWithPreviousRect = new Rect(position.x + position.width * 0.5f, y, position.width * 0.5f, EditorGUIUtility.singleLineHeight);
        // 다음 줄로 이동
        y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        EditorGUI.PropertyField(startPosRect, startPosProp);
        EditorGUI.PropertyField(typeRect, typeProp, GUIContent.none);
        EditorGUI.PropertyField(durationRect, durationProp);
        EditorGUI.PropertyField(delayRect, delayProp);
        EditorGUI.PropertyField(runWithPreviousRect, runWithPreviousProp);

        // 선택된 애니메이션 타입에 따라 다른 필드를 표시
        AnimationType selectedType = (AnimationType)typeProp.enumValueIndex;

        // 구분선 추가
        y += EditorGUIUtility.standardVerticalSpacing * 2;
        Rect lineRect = new Rect(position.x, y, position.width, 1);
        EditorGUI.DrawRect(lineRect, new Color(0.5f, 0.5f, 0.5f, 1));
        y += EditorGUIUtility.standardVerticalSpacing * 3;

        switch (selectedType)
        {
            case AnimationType.Move:
                Rect posRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(posRect, property.FindPropertyRelative("targetPosition"));
                break;

            case AnimationType.Scale:
                Rect scaleRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(scaleRect, property.FindPropertyRelative("targetScale"));
                break;

            case AnimationType.Rotate:
                Rect rotateRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(rotateRect, property.FindPropertyRelative("targetRotation"));
                break;

            case AnimationType.Fade:
                Rect fadeRect = new Rect(position.x, y, position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(fadeRect, property.FindPropertyRelative("targetAlpha"));
                break;
        }

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    // 프로퍼티 전체 높이를 계산해서 반환하는 함수
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // 기본적으로 공통 필드가 차지하는 높이 (3줄)
        float totalHeight = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 3;

        // 구분선과 여백 높이 추가
        totalHeight += EditorGUIUtility.standardVerticalSpacing * 3 + 1;

        // 타입별 특정 필드가 차지하는 높이 (1줄)
        totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        return totalHeight;
    }
}
