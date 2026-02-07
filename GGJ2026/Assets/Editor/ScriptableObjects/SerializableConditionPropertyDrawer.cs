using GGJ2026.Gameplay.Condition;
using UnityEditor;
using UnityEngine;

namespace GGJ2026.Gameplay.Condition
{
    /// <summary>
    /// SerializableCondition的PropertyDrawer，用于在列表中显示
    /// </summary>
    [CustomPropertyDrawer(typeof(SerializableCondition))]
    public class SerializableConditionPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // 获取序列化属性
            var conditionTypeProp = property.FindPropertyRelative("conditionType");
            var targetFeatureIdProp = property.FindPropertyRelative("targetFeatureId");
            var targetFeatureTypeProp = property.FindPropertyRelative("targetFeatureType");
            var enablePositionProp = property.FindPropertyRelative("enablePosition");
            var enableRotationProp = property.FindPropertyRelative("enableRotation");
            var enableScaleProp = property.FindPropertyRelative("enableScale");
            var checkPositionProp = property.FindPropertyRelative("checkPosition");
            var checkRotationListProp = property.FindPropertyRelative("checkRotationList");
            var checkScaleProp = property.FindPropertyRelative("checkScale");

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            // 开始绘制
            Rect currentRect = new Rect(position.x, position.y, position.width, lineHeight);

            // 折叠标签
            property.isExpanded = EditorGUI.Foldout(currentRect, property.isExpanded, label, true);

            if (property.isExpanded)
            {
                float yOffset = lineHeight + spacing;

                // 条件类型
                currentRect.y = position.y + yOffset;
                currentRect.height = EditorGUI.GetPropertyHeight(conditionTypeProp);
                EditorGUI.PropertyField(currentRect, conditionTypeProp);
                yOffset += currentRect.height + spacing;

                // 根据条件类型显示相应字段
                var conditionType = (ConditionType)conditionTypeProp.enumValueIndex;

                if (conditionType is ConditionType.FeatureById or ConditionType.FeatureExcludeId)
                {
                    currentRect.y = position.y + yOffset;
                    currentRect.height = EditorGUI.GetPropertyHeight(targetFeatureIdProp);
                    EditorGUI.PropertyField(currentRect, targetFeatureIdProp);
                    yOffset += currentRect.height + spacing;
                }
                else if (conditionType is ConditionType.FeatureByType or ConditionType.FeatureExcludeType)
                {
                    currentRect.y = position.y + yOffset;
                    currentRect.height = EditorGUI.GetPropertyHeight(targetFeatureTypeProp);
                    EditorGUI.PropertyField(currentRect, targetFeatureTypeProp);
                    yOffset += currentRect.height + spacing;
                }

                // 比较设置标题
                currentRect.y = position.y + yOffset;
                currentRect.height = lineHeight;
                EditorGUI.LabelField(currentRect, "比较设置", EditorStyles.boldLabel);
                yOffset += lineHeight + spacing;

                // 比较设置字段
                currentRect.y = position.y + yOffset;
                currentRect.height = EditorGUI.GetPropertyHeight(enablePositionProp);
                EditorGUI.PropertyField(currentRect, enablePositionProp);
                yOffset += currentRect.height + spacing;

                currentRect.y = position.y + yOffset;
                currentRect.height = EditorGUI.GetPropertyHeight(enableRotationProp);
                EditorGUI.PropertyField(currentRect, enableRotationProp);
                yOffset += currentRect.height + spacing;

                currentRect.y = position.y + yOffset;
                currentRect.height = EditorGUI.GetPropertyHeight(enableScaleProp);
                EditorGUI.PropertyField(currentRect, enableScaleProp);
                yOffset += currentRect.height + spacing;

                currentRect.y = position.y + yOffset;
                currentRect.height = EditorGUI.GetPropertyHeight(checkPositionProp);
                EditorGUI.PropertyField(currentRect, checkPositionProp);
                yOffset += currentRect.height + spacing;

                currentRect.y = position.y + yOffset;
                currentRect.height = EditorGUI.GetPropertyHeight(checkRotationListProp);
                EditorGUI.PropertyField(currentRect, checkRotationListProp);
                yOffset += currentRect.height + spacing;

                currentRect.y = position.y + yOffset;
                currentRect.height = EditorGUI.GetPropertyHeight(checkScaleProp);
                EditorGUI.PropertyField(currentRect, checkScaleProp);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
                return EditorGUIUtility.singleLineHeight;

            // 获取序列化属性
            var conditionTypeProp = property.FindPropertyRelative("conditionType");
            var targetFeatureIdProp = property.FindPropertyRelative("targetFeatureId");
            var targetFeatureTypeProp = property.FindPropertyRelative("targetFeatureType");
            var enablePositionProp = property.FindPropertyRelative("enablePosition");
            var enableRotationProp = property.FindPropertyRelative("enableRotation");
            var enableScaleProp = property.FindPropertyRelative("enableScale");
            var checkPositionProp = property.FindPropertyRelative("checkPosition");
            var checkRotationListProp = property.FindPropertyRelative("checkRotationList");
            var checkScaleProp = property.FindPropertyRelative("checkScale");

            var conditionType = (ConditionType)conditionTypeProp.enumValueIndex;

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            // 基础高度：折叠标签
            float height = lineHeight + spacing;

            // 条件类型
            height += EditorGUI.GetPropertyHeight(conditionTypeProp) + spacing;

            // 根据条件类型添加相应字段高度
            if (conditionType == ConditionType.FeatureById)
            {
                height += EditorGUI.GetPropertyHeight(targetFeatureIdProp) + spacing;
            }
            else if (conditionType == ConditionType.FeatureByType)
            {
                height += EditorGUI.GetPropertyHeight(targetFeatureTypeProp) + spacing;
            }

            // 添加比较设置字段高度
            height += lineHeight + spacing; // "比较设置"标题
            height += EditorGUI.GetPropertyHeight(enablePositionProp) + spacing;
            height += EditorGUI.GetPropertyHeight(enableRotationProp) + spacing;
            height += EditorGUI.GetPropertyHeight(enableScaleProp) + spacing;
            height += EditorGUI.GetPropertyHeight(enableScaleProp) + spacing;
            height += EditorGUI.GetPropertyHeight(checkPositionProp) + spacing;
            height += EditorGUI.GetPropertyHeight(checkRotationListProp) + spacing;
            height += EditorGUI.GetPropertyHeight(checkScaleProp) + spacing;

            return height;
        }
    }
}
