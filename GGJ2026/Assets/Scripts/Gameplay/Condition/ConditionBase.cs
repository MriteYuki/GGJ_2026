using GGJ2026.FaceComponent;
using UnityEngine;

namespace GGJ2026.Gameplay.Condition
{
    /// <summary>
    /// 条件基类
    /// </summary>
    public abstract class ConditionBase
    {
        [Header("比较对象")]
        [SerializeField] protected Feature targetFeature;
        [SerializeField] protected Feature compareFeature;

        [Header("比较设置")]

        [Header("检测位置-相对半径")]
        [SerializeField] protected float checkRadius;

        [Header("检测旋转-相对容差角度")]
        [Tooltip("设置旋转角度的容差范围 X:最小值 Y:最大值")]
        [SerializeField] protected Vector2 checkRotation;

        [Header("检测比例-容差比例")]
        [Tooltip("设置放缩比例的容差范围 X:最小值 Y:最大值")]
        [SerializeField] protected Vector2 checkScale;

        /// <summary>
        /// 目标特征
        /// </summary>
        public Feature TargetFeature
        {
            get => targetFeature;
            set => targetFeature = value;
        }

        /// <summary>
        /// 待比较特征
        /// </summary>
        public Feature CompareFeature
        {
            get => compareFeature;
            set => compareFeature = value;
        }

        /// <summary>
        /// 检查条件是否满足
        /// </summary>
        /// <returns>是否满足条件</returns>
        public abstract bool Check();

        /// <summary>
        /// 检测位置是否在相对半径范围内
        /// </summary>
        /// <returns>是否在范围内</returns>
        protected bool CheckPositionRadius()
        {
            if (targetFeature == null || compareFeature == null)
                return false;

            float distance = Vector3.Distance(targetFeature.Position, compareFeature.Position);
            return distance <= checkRadius;
        }

        /// <summary>
        /// 检测旋转角度是否在容差范围内
        /// </summary>
        /// <returns>是否在范围内</returns>
        protected bool CheckRotationTolerance()
        {
            if (targetFeature == null || compareFeature == null)
                return false;

            float angle = Quaternion.Angle(targetFeature.Rotation, compareFeature.Rotation);
            return angle >= checkRotation.x && angle <= checkRotation.y;
        }

        /// <summary>
        /// 检测缩放比例是否在容差范围内
        /// </summary>
        /// <returns>是否在范围内</returns>
        protected bool CheckScaleTolerance()
        {
            if (targetFeature == null || compareFeature == null)
                return false;

            Vector3 scaleDiff = targetFeature.Scale - compareFeature.Scale;
            float magnitude = scaleDiff.magnitude;
            return magnitude >= checkScale.x && magnitude <= checkScale.y;
        }

        /// <summary>
        /// 检测所有设置的参数
        /// </summary>
        /// <returns>是否所有检测都通过</returns>
        protected bool CheckAllParameters()
        {
            bool result = true;

            if (!CheckPositionRadius())
            {
                result = false;
            }

            if (!CheckRotationTolerance())
            {
                result = false;
            }

            if (!CheckScaleTolerance())
            {
                result = false;
            }

            return result;
        }
    }
}
