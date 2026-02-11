using System.Collections.Generic;
using GGJ2026.Gameplay.Condition;
using UnityEngine;
using GGJ2026;
using System;

namespace GGJ2026.Gameplay.ScriptableObjects
{
    /// <summary>
    /// 时间改变事件
    /// </summary>
    /// <param name="isValid">是否有效时间</param>
    /// <param name="curTimeText">当前时间</param>
    public delegate void TimeChangedHandler(bool isValid, string curTimeText);

    /// <summary>
    /// 关卡数据配置
    /// </summary>
    [CreateAssetMenu(fileName = "New LevelData", menuName = "GGJ2026/Level Data")]
    public class LevelData : ScriptableObject
    {
        [Header("关卡条件配置")]
        [SerializeField] private List<SerializableCondition> serializableConditions = new();

        [Header("时间配置(血量)")]
        [SerializeField] private List<string> timeConfigs = new();

        private int curTimeIndex = 0;

        public TimeChangedHandler OnTimeChanged;

        /// <summary>
        /// 前进时间
        /// </summary>
        public void MoveTimeForward()
        {
            curTimeIndex++;
            var isVaild = curTimeIndex < timeConfigs.Count;
            var curTimeText = isVaild? timeConfigs[curTimeIndex] : "Unknown";
            OnTimeChanged?.Invoke(isVaild, curTimeText);
        }

        /// <summary>
        /// 重置时间
        /// </summary>
        public void ResetTimeData()
        {
            curTimeIndex = 0;
            if(timeConfigs.Count <= 0)
            {
                OnTimeChanged?.Invoke(false, "Unknown");
                return;
            }

            OnTimeChanged?.Invoke(true, timeConfigs[curTimeIndex]);
        }

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
