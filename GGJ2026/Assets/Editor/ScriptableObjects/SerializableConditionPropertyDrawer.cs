using GGJ2026.Gameplay.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace GGJ2026.Gameplay.Condition
{
    [CustomPropertyDrawer(typeof(SerializableCondition))]
    public class SerializableConditionPropertyDrawer : PropertyDrawer
    {
        // 统一间距常量，方便后期微调
        private float VSpacing => EditorGUIUtility.standardVerticalSpacing;
        private float LHeight => EditorGUIUtility.singleLineHeight;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // 1. 绘制 Foldout（折叠标签）
            Rect foldoutRect = new Rect(position.x, position.y, position.width, LHeight);
            property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label, true);

            if (property.isExpanded)
            {
                // 增加缩进
                EditorGUI.indentLevel++;

                // 初始化第一个属性的绘制位置（跳过折叠标签的高度）
                // 这里的 position.width 是关键，它让控件随窗口宽度自适应
                Rect currentRect = new Rect(position.x, position.y + LHeight + VSpacing, position.width, 0);

                // --- 内部绘制逻辑 ---

                // 绘制属性并自动更新 Rect 的局部函数
                void DrawAuto(string propName)
                {
                    SerializedProperty p = property.FindPropertyRelative(propName);
                    if (p != null)
                    {
                        // 核心：这里的 true 会计算子属性（如列表）展开后的完整高度
                        currentRect.height = EditorGUI.GetPropertyHeight(p, true);
                        EditorGUI.PropertyField(currentRect, p, true);

                        // 绘制完后，将 y 坐标下移：当前高度 + 间距
                        currentRect.y += currentRect.height + VSpacing;
                    }
                }

                // 2. 绘制条件类型
                DrawAuto("conditionType");

                // 3. 根据枚举逻辑过滤显示
                var conditionTypeProp = property.FindPropertyRelative("conditionType");
                var conditionType = (ConditionType)conditionTypeProp.enumValueIndex;

                if (conditionType is ConditionType.FeatureById or ConditionType.FeatureExcludeId)
                    DrawAuto("targetFeatureId");
                else if (conditionType is ConditionType.FeatureByType or ConditionType.FeatureExcludeType)
                    DrawAuto("targetFeatureType");

                // 4. 绘制“比较设置”标题
                currentRect.height = LHeight;
                EditorGUI.LabelField(currentRect, "比较设置", EditorStyles.boldLabel);
                currentRect.y += LHeight + VSpacing;

                // 5. 顺序绘制剩余所有字段，完全自适应高度
                DrawAuto("enablePosition");
                DrawAuto("enableRotation");
                DrawAuto("enableScale");
                DrawAuto("checkPosition");
                DrawAuto("checkRotationList"); // 嵌套列表会根据 GetPropertyHeight 自动撑开
                DrawAuto("checkScaleList");

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // 如果没展开，只占一行高度
            if (!property.isExpanded)
                return LHeight;

            // 基础高度：折叠标签本身
            float totalHeight = LHeight + VSpacing;

            // 计算属性高度的局部函数
            float GetHeightAuto(string propName)
            {
                SerializedProperty p = property.FindPropertyRelative(propName);
                // 必须传 true，否则嵌套列表展开时高度不会被计入父级，导致重叠
                return (p != null) ? EditorGUI.GetPropertyHeight(p, true) + VSpacing : 0;
            }

            // 1. 固定显示的字段
            totalHeight += GetHeightAuto("conditionType");

            // 2. 逻辑过滤显示的字段
            var conditionTypeProp = property.FindPropertyRelative("conditionType");
            var conditionType = (ConditionType)conditionTypeProp.enumValueIndex;

            if (conditionType is ConditionType.FeatureById or ConditionType.FeatureExcludeId)
                totalHeight += GetHeightAuto("targetFeatureId");
            else if (conditionType is ConditionType.FeatureByType or ConditionType.FeatureExcludeType)
                totalHeight += GetHeightAuto("targetFeatureType");

            // 3. 比较设置部分
            totalHeight += LHeight + VSpacing; // 标题高度
            totalHeight += GetHeightAuto("enablePosition");
            totalHeight += GetHeightAuto("enableRotation");
            totalHeight += GetHeightAuto("enableScale");
            totalHeight += GetHeightAuto("checkPosition");
            totalHeight += GetHeightAuto("checkRotationList");
            totalHeight += GetHeightAuto("checkScaleList");

            return totalHeight;
        }
    }
}