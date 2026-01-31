using System.Collections.Generic;
using GGJ2026.Gameplay.Condition;
using GGJ2026.Gameplay.ScriptableObjects;
using UnityEngine;

namespace GGJ2026.Gameplay
{
    /// <summary>
    /// 关卡管理器，负责管理关卡数据和条件检查
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        [Header("关卡配置")]
        [SerializeField] private LevelData levelData;

        [Header("调试设置")]
        [SerializeField] private bool enableDebugLog = true;

        [Header("关卡自动开始")]
        [SerializeField] private bool autoMarkStartState = true;

        // 关卡开始时的特征状态记录
        private readonly Dictionary<Feature, FeatureState> initialFeatureStates = new();
        private bool hasMarkedStartState = false;

        /// <summary>
        /// 当前关卡数据
        /// </summary>
        public LevelData CurrentLevelData
        {
            get => levelData;
            set => levelData = value;
        }

        /// <summary>
        /// 获取场景中所有带有Feature组件的GameObject
        /// </summary>
        public List<Feature> GetAllFeaturesInScene()
        {
            var features = new List<Feature>();
            var featureComponents = FindObjectsByType<Feature>(FindObjectsSortMode.None);

            foreach (var feature in featureComponents)
            {
                if (feature != null && feature.gameObject.activeInHierarchy)
                {
                    features.Add(feature);
                }
            }

            if (enableDebugLog)
            {
                Debug.Log($"在场景中找到 {features.Count} 个Feature组件");
                foreach (var feature in features)
                {
                    Debug.Log($"Feature: {feature.Type} (ID: {feature.ID})");
                }
            }

            return features;
        }

        /// <summary>
        /// 检查所有关卡条件是否满足
        /// </summary>
        public bool CheckAllConditions()
        {
            if (levelData == null)
            {
                Debug.LogError("LevelData未设置，无法检查条件");
                return false;
            }

            if (levelData.SerializableConditions == null || levelData.SerializableConditions.Count == 0)
            {
                if (enableDebugLog)
                {
                    Debug.Log("关卡无任何条件，默认通过");
                }
                return true;
            }

            // 获取场景中所有特征
            var allFeatures = GetAllFeaturesInScene();

            if (allFeatures.Count == 0)
            {
                Debug.LogWarning("场景中未找到任何Feature组件");
                return false;
            }

            // 检查每个条件
            for (int i = 0; i < levelData.SerializableConditions.Count; i++)
            {
                var serializableCondition = levelData.SerializableConditions[i];
                bool isSatisfied = CheckSingleCondition(serializableCondition, allFeatures);

                if (!isSatisfied)
                {
                    if (enableDebugLog)
                    {
                        Debug.Log($"第 {i} 个条件检查失败, 中断检查: {serializableCondition}");
                    }

                    return false;
                }
                else
                {
                    if (enableDebugLog)
                    {
                        Debug.Log($"第 {i} 个条件检查通过: {serializableCondition}");
                    }
                }
            }

            if (enableDebugLog)
            {
                Debug.Log("所有关卡条件检查通过！");
            }
            return true;
        }

        /// <summary>
        /// 检查单个条件是否满足
        /// </summary>
        private bool CheckSingleCondition(SerializableCondition serializableCondition, List<Feature> allFeatures)
        {
            if (serializableCondition == null)
                return false;

            foreach (var feature in allFeatures)
            {
                var condition = serializableCondition.ToCondition(feature);
                if (condition == null)
                    continue;

                if (condition.Check())
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 获取关卡条件状态报告
        /// </summary>
        public string GetConditionStatusReport()
        {
            if (levelData == null)
            {
                return "关卡数据未设置";
            }

            var allFeatures = GetAllFeaturesInScene();
            var report = $"关卡条件状态报告 (总条件数: {levelData.SerializableConditions.Count})\n";
            report += $"场景中特征数量: {allFeatures.Count}\n\n";

            for (int i = 0; i < levelData.SerializableConditions.Count; i++)
            {
                var condition = levelData.SerializableConditions[i];
                if (condition == null)
                {
                    report += $"条件 {i + 1}: 空条件\n";
                    continue;
                }

                bool isSatisfied = CheckSingleCondition(condition, allFeatures);

                report += $"条件 {i + 1}: {condition.Type} - {(isSatisfied ? "✓ 满足" : "✗ 未满足")}\n";
                report += $"  目标: {condition.TargetFeatureId} (类型: {condition.TargetFeatureType})\n";
            }

            return report;
        }

        /// <summary>
        /// 特征状态记录结构
        /// </summary>
        [System.Serializable]
        private struct FeatureState
        {
            public Vector3 position;
            public Quaternion rotation;
            public Vector3 scale;

            public FeatureState(Feature feature)
            {
                position = feature.transform.localPosition;
                rotation = feature.transform.localRotation;
                scale = feature.transform.localScale;
            }

            public void ApplyTo(Feature feature)
            {
                feature.transform.localPosition = position;
                feature.transform.localRotation = rotation;
                feature.transform.localScale = scale;
            }
        }

        /// <summary>
        /// 标记关卡开始状态
        /// </summary>
        public void MarkLevelStartState()
        {
            var allFeatures = GetAllFeaturesInScene();
            initialFeatureStates.Clear();

            foreach (var feature in allFeatures)
            {
                if (feature != null)
                {
                    initialFeatureStates[feature] = new FeatureState(feature);
                }
            }

            hasMarkedStartState = true;

            if (enableDebugLog)
            {
                Debug.Log($"标记了 {initialFeatureStates.Count} 个Feature的初始状态");
            }
        }

        /// <summary>
        /// 重置关卡状态
        /// </summary>
        public void ResetLevel()
        {
            if (!hasMarkedStartState)
            {
                if (enableDebugLog)
                {
                    Debug.LogWarning("未标记关卡开始状态，无法重置");
                }
                return;
            }

            int resetCount = 0;
            foreach (var kvp in initialFeatureStates)
            {
                var feature = kvp.Key;
                var state = kvp.Value;

                if (feature != null && feature.gameObject.activeInHierarchy)
                {
                    state.ApplyTo(feature);
                    resetCount++;
                }
            }

            if (enableDebugLog)
            {
                Debug.Log($"重置了 {resetCount} 个Feature的状态");
            }
        }

        /// <summary>
        /// 快速检查关卡是否通过（用于按钮事件等）
        /// </summary>
        public void QuickCheck()
        {
            bool passed = CheckAllConditions();
            if (passed)
            {
                Debug.Log("<color=#FF0000>关卡通过！</color>");
                // 触发关卡通过事件
                GameManager.Instance.LoadNextScene();
            }
            else
            {
                Debug.Log("<color=#FF0000>关卡未通过，请检查条件</color>");
                ResetLevel();
            }
        }

        void Start()
        {
            if (enableDebugLog)
            {
                Debug.Log("LevelManager初始化完成");
                if (levelData != null)
                {
                    Debug.Log($"加载关卡数据: {levelData.name}");
                    Debug.Log(GetConditionStatusReport());
                }
            }

            // 自动标记关卡开始状态
            if (autoMarkStartState)
            {
                MarkLevelStartState();
            }
        }

        void Update()
        {
            // 可以在这里添加实时条件检查逻辑
            // 例如：if (enableRealTimeCheck) { CheckAllConditions(); }
        }
    }
}
