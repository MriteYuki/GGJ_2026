#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using GGJ2026.Gameplay;

[CustomEditor(typeof(SceneFlowConfig))]
public class SceneFlowConfigEditor : Editor
{
    private SceneFlowConfig config;
    private bool showDebugInfo = false;
    private Vector2 scrollPosition;

    void OnEnable()
    {
        config = target as SceneFlowConfig;
        
        // 安全检查
        if (config == null)
        {
            Debug.LogError("SceneFlowConfigEditor: 目标对象为null");
            return;
        }
    }

    void OnDisable()
    {
        // 清理资源，防止空引用异常
        config = null;
    }

    public override void OnInspectorGUI()
    {
        // 安全检查
        if (config == null || serializedObject == null)
        {
            EditorGUILayout.HelpBox("配置对象为空或序列化对象不可用", MessageType.Error);
            return;
        }
        
        serializedObject.Update();
        
        EditorGUILayout.Space();
        
        // 基础设置
        EditorGUILayout.LabelField("场景流程配置", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("startScene"), new GUIContent("起始场景"));
        
        EditorGUILayout.Space();
        
        // 场景流程链配置
        EditorGUILayout.LabelField("流程链定义", EditorStyles.boldLabel);
        
        SerializedProperty chains = serializedObject.FindProperty("sceneFlowChains");
        EditorGUILayout.PropertyField(chains, new GUIContent("流程链列表"), true);
        
        EditorGUILayout.Space();
        
        // 验证按钮
        if (GUILayout.Button("验证配置"))
        {
            if (config.ValidateConfig())
            {
                EditorUtility.DisplayDialog("验证成功", "场景流程配置验证通过！", "确定");
            }
            else
            {
                EditorUtility.DisplayDialog("验证失败", "请检查控制台中的错误信息", "确定");
            }
        }
        
        EditorGUILayout.Space();
        
        // 流程可视化预览
        if (config.sceneFlowChains.Count > 0)
        {
            EditorGUILayout.LabelField("流程预览", EditorStyles.boldLabel);
            
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
            
            foreach (var chain in config.sceneFlowChains)
            {
                EditorGUILayout.BeginVertical("box");
                
                // 流程链标题
                EditorGUILayout.LabelField($"流程链: {chain.chainName}", EditorStyles.boldLabel);
                
                // 场景序列可视化
                EditorGUILayout.BeginHorizontal();
                
                for (int i = 0; i < chain.sceneSequence.Count; i++)
                {
                    string sceneName = chain.sceneSequence[i];
                    
                    // 绘制场景节点
                    GUIStyle nodeStyle = new GUIStyle(EditorStyles.miniButton);
                    nodeStyle.normal.textColor = i == 0 ? Color.green : (i == chain.sceneSequence.Count - 1 ? Color.red : Color.white);
                    
                    if (GUILayout.Button(sceneName, nodeStyle, GUILayout.Width(80)))
                    {
                        // 点击场景名称可以快速定位到场景资源
                        Selection.activeObject = AssetDatabase.LoadAssetAtPath<SceneAsset>($"Assets/Scenes/{sceneName}.unity");
                    }
                    
                    // 绘制箭头（除了最后一个场景）
                    if (i < chain.sceneSequence.Count - 1)
                    {
                        EditorGUILayout.LabelField("→", GUILayout.Width(20));
                    }
                }
                
                EditorGUILayout.EndHorizontal();
                
                // 场景详细信息
                EditorGUILayout.LabelField($"首关: {chain.FirstLevel}", EditorStyles.miniLabel);
                EditorGUILayout.LabelField($"最终关: {chain.FinalLevel}", EditorStyles.miniLabel);
                EditorGUILayout.LabelField($"总关卡数: {chain.TotalScenes}", EditorStyles.miniLabel);
                
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }
            
            EditorGUILayout.EndScrollView();
        }
        else
        {
            EditorGUILayout.HelpBox("请至少添加一个流程链", MessageType.Info);
        }
        
        EditorGUILayout.Space();
        
        // 调试信息
        showDebugInfo = EditorGUILayout.Foldout(showDebugInfo, "调试信息");
        if (showDebugInfo)
        {
            EditorGUILayout.BeginVertical("box");
            
            EditorGUILayout.LabelField("配置信息:", EditorStyles.miniBoldLabel);
            EditorGUILayout.LabelField($"起始场景: {config.startScene}");
            EditorGUILayout.LabelField($"流程链数量: {config.sceneFlowChains.Count}");
            
            if (config.sceneFlowChains.Count > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("各流程链信息:", EditorStyles.miniBoldLabel);
                
                foreach (var chain in config.sceneFlowChains)
                {
                    EditorGUILayout.LabelField($"{chain.chainName}: {chain.sceneSequence.Count} 个场景");
                }
            }
            
            EditorGUILayout.EndVertical();
        }
        
        // 快速操作按钮
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("添加新流程链"))
        {
            AddNewChain();
        }
        
        if (GUILayout.Button("清空所有流程链"))
        {
            if (EditorUtility.DisplayDialog("确认清空", "确定要清空所有流程链吗？", "确定", "取消"))
            {
                config.sceneFlowChains.Clear();
                EditorUtility.SetDirty(config);
            }
        }
        
        EditorGUILayout.EndHorizontal();
        
        serializedObject.ApplyModifiedProperties();
    }
    
    private void AddNewChain()
    {
        var newChain = new SceneFlowChain
        {
            chainName = $"流程链{config.sceneFlowChains.Count + 1}"
        };
        
        config.sceneFlowChains.Add(newChain);
        EditorUtility.SetDirty(config);
    }
}
#endif