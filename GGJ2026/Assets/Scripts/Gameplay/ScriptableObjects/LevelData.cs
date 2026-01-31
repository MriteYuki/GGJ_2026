using System.Collections.Generic;
using GGJ2026.Gameplay.Condition;
using UnityEngine;
using GGJ2026;

namespace GGJ2026.Gameplay.ScriptableObjects
{
    /// <summary>
    /// 关卡数据配置
    /// </summary>
    [CreateAssetMenu(fileName = "New LevelData", menuName = "GGJ2026/Level Data")]
    public class LevelData : ScriptableObject
    {
        [Header("关卡条件配置")]
        [SerializeField] private List<SerializableCondition> serializableConditions = new();

        /// <summary>
        /// 序列化条件列表
        /// </summary>
        public List<SerializableCondition> SerializableConditions
        {
            get => serializableConditions;
            set => serializableConditions = value;
        }

        /// <summary>
        /// 获取条件数量
        /// </summary>
        public int ConditionCount => serializableConditions.Count;

        /// <summary>
        /// 添加序列化条件
        /// </summary>
        public void AddSerializableCondition(SerializableCondition condition)
        {
            if (condition != null && !serializableConditions.Contains(condition))
            {
                serializableConditions.Add(condition);
            }
        }

        /// <summary>
        /// 移除序列化条件
        /// </summary>
        public void RemoveSerializableCondition(SerializableCondition condition)
        {
            if (condition != null)
            {
                serializableConditions.Remove(condition);
            }
        }

        /// <summary>
        /// 清空所有条件
        /// </summary>
        public void ClearConditions()
        {
            serializableConditions.Clear();
        }

        /// <summary>
        /// 检查所有条件是否满足
        /// </summary>
        public bool CheckAllConditions(List<Feature> availableFeatures = null)
        {
            if (serializableConditions == null || serializableConditions.Count == 0)
                return true;

            foreach (var serializableCondition in serializableConditions)
            {
                if (serializableCondition == null)
                {
                    Debug.LogWarning("LevelData中存在空条件，跳过检查");
                    continue;
                }

                try
                {
                    // 转换为运行时条件对象
                    var condition = serializableCondition.ToCondition();

                    // 如果提供了可用特征列表，则设置到条件中
                    if (availableFeatures != null)
                    {
                        SetConditionFeatures(condition, availableFeatures);
                    }

                    if (!condition.Check())
                    {
                        return false;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"条件检查失败: {ex.Message}");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 为条件设置特征列表
        /// </summary>
        private void SetConditionFeatures(ConditionBase condition, List<Feature> availableFeatures)
        {
            if (condition is FeatureByIdCondition idCondition)
            {
                // 检查属性是否存在，避免运行时错误
                var property = typeof(FeatureByIdCondition).GetProperty("FaceFeatures");
                if (property != null && property.CanWrite)
                {
                    property.SetValue(idCondition, availableFeatures);
                }
                else
                {
                    Debug.LogWarning("FeatureByIdCondition.FaceFeatures属性不存在或不可写");
                }
            }
            else if (condition is FeatureByTypeCondition typeCondition)
            {
                var property = typeof(FeatureByTypeCondition).GetProperty("FaceFeatures");
                if (property != null && property.CanWrite)
                {
                    property.SetValue(typeCondition, availableFeatures);
                }
                else
                {
                    Debug.LogWarning("FeatureByTypeCondition.FaceFeatures属性不存在或不可写");
                }
            }
        }

        /// <summary>
        /// 检查是否有空条件
        /// </summary>
        public bool HasNullConditions()
        {
            foreach (var condition in serializableConditions)
            {
                if (condition == null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 清理空条件
        /// </summary>
        public void RemoveNullConditions()
        {
            serializableConditions.RemoveAll(condition => condition == null);
        }

        /// <summary>
        /// 从运行时条件列表创建序列化数据
        /// </summary>
        public void ImportFromRuntimeConditions(List<ConditionBase> runtimeConditions)
        {
            serializableConditions.Clear();
            foreach (var condition in runtimeConditions)
            {
                if (condition != null)
                {
                    serializableConditions.Add(SerializableCondition.FromCondition(condition));
                }
            }
        }

        /// <summary>
        /// 转换为运行时条件列表
        /// </summary>
        public List<ConditionBase> ConvertToRuntimeConditions()
        {
            var runtimeConditions = new List<ConditionBase>();
            foreach (var serializableCondition in serializableConditions)
            {
                if (serializableCondition != null)
                {
                    runtimeConditions.Add(serializableCondition.ToCondition());
                }
            }
            return runtimeConditions;
        }

        /// <summary>
        /// 获取条件描述信息
        /// </summary>
        public string GetConditionDescription()
        {
            if (serializableConditions.Count == 0)
                return "无任何条件";

            var description = $"关卡包含 {serializableConditions.Count} 个条件：\n";

            foreach (var condition in serializableConditions)
            {
                if (condition != null)
                {
                    description += $"- {condition}\n";
                }
            }

            return description;
        }
    }
}
