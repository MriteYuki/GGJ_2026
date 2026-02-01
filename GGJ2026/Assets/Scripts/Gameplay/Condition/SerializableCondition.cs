using System;
using UnityEngine;

namespace GGJ2026.Gameplay.Condition
{
    /// <summary>
    /// 可序列化的条件数据
    /// </summary>
    [System.Serializable]
    public class SerializableCondition
    {
        [Header("条件类型")]
        [SerializeField] private ConditionType conditionType;

        [Header("ID查找条件")]
        [SerializeField] private string targetFeatureId;

        [Header("类型查找条件")]
        [SerializeField] private FeatureType targetFeatureType;
        [SerializeField] private bool enablePosition = true;
        [SerializeField] private bool enableRotation = true;
        [SerializeField] private bool enableScale = true;
        [SerializeField] private Vector2 checkPosition;
        [SerializeField] private float checkRadius;
        [SerializeField] private RotationType checkRotation;
        [SerializeField] private ScaleType checkScale;

        /// <summary>
        /// 条件类型
        /// </summary>
        public ConditionType Type
        {
            get => conditionType;
            set => conditionType = value;
        }

        /// <summary>
        /// 目标特征ID
        /// </summary>
        public string TargetFeatureId
        {
            get => targetFeatureId;
            set => targetFeatureId = value;
        }

        /// <summary>
        /// 目标特征类型
        /// </summary>
        public FeatureType TargetFeatureType
        {
            get => targetFeatureType;
            set => targetFeatureType = value;
        }

        public LogicAndOr LogicAndOr
        {
            get => conditionType switch
            {
                ConditionType.FeatureById or ConditionType.FeatureByType => LogicAndOr.Or,
                ConditionType.FeatureExcludeId or ConditionType.FeatureExcludeType => LogicAndOr.And,
                _ => LogicAndOr.None
            };
        }

        /// <summary>
        /// 转换为具体的条件对象
        /// </summary>
        public ConditionBase ToCondition(Feature feature)
        {
            ConditionBase condition = conditionType
            switch
            {
                ConditionType.FeatureById =>
                        new FeatureByIdCondition(targetFeatureId),
                ConditionType.FeatureByType =>
                        new FeatureByTypeCondition(targetFeatureType),
                ConditionType.FeatureExcludeId =>
                        new FeatureExcludeIdCondition(targetFeatureId),
                ConditionType.FeatureExcludeType =>
                        new FeatureExcludeTypeCondition(targetFeatureType),
                _ => new ConditionBase(),
            };

            condition.CompareFeature = feature;
            condition.Set(enablePosition, enableRotation, enableScale,
                checkPosition, checkRadius, checkRotation, checkScale);

            return condition;
        }

        /// <summary>
        /// 从条件对象创建序列化数据
        /// </summary>
        public static SerializableCondition FromCondition(ConditionBase condition)
        {
            var serializable = new SerializableCondition();

            if (condition is FeatureByIdCondition idCondition)
            {
                serializable.conditionType = ConditionType.FeatureById;
            }
            else if (condition is FeatureByTypeCondition typeCondition)
            {
                serializable.conditionType = ConditionType.FeatureByType;
            }
            else if (condition is FeatureExcludeIdCondition excludeIdCondition)
            {
                serializable.conditionType = ConditionType.FeatureExcludeId;
            }
            else if (condition is FeatureExcludeTypeCondition excludeTypeCondition)
            {
                serializable.conditionType = ConditionType.FeatureExcludeType;
            }
            return serializable;
        }

        public override string ToString()
        {
            return $"SerializableCondition {{Type: {conditionType}, TargetId: {targetFeatureId}, TargetType: {targetFeatureType}" +
            $"Position: {checkPosition}, Radius: {checkRadius}, Rotation: {checkRotation}, Scale: {checkScale}}}";
        }
    }
}
