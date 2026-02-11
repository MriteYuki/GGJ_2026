using GGJ2026.Gameplay.ScriptableObjects;
using UnityEditor;

namespace GGJ2026.Gameplay.Condition
{
    [CustomEditor(typeof(LevelData))]
    [CanEditMultipleObjects]
    public class LevelDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // 更新序列化对象
            serializedObject.Update();

            // 找到你的列表属性
            SerializedProperty listProp = serializedObject.FindProperty("serializableConditions");

            // 关键点：第二个参数传 true。
            // 这会让 Unity 自动调用你刚才写的 PropertyDrawer.GetPropertyHeight()，
            // 并根据其返回的总高度自动撑开父级窗口。
            EditorGUILayout.PropertyField(listProp, true);

            SerializedProperty timeConfigsProp = serializedObject.FindProperty("timeConfigs");
            EditorGUILayout.PropertyField(timeConfigsProp, true);

            // 应用修改
            serializedObject.ApplyModifiedProperties();
        }
    }
}

