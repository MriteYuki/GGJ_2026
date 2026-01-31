using UnityEngine;

namespace GGJ2026.Gameplay.Condition
{
    /// <summary>
    /// 条件基类
    /// </summary>
    public class ConditionBase
    {
        [Header("比较对象")]
        [SerializeField] protected Feature compareFeature;

        [SerializeField]
        protected bool enablePosition;

        [SerializeField]
        protected bool enableRotation;

        [SerializeField]
        protected bool enableScale;

        [Header("比较设置")]
        [Header("检测位置-相对位置")]
        [SerializeField] protected Vector2 checkPosition;

        [Header("检测位置-相对半径")]
        [SerializeField] protected float checkRadius;

        [Header("检测旋转-相对容差角度")]
        [Tooltip("设置旋转角度的容差范围 X:最小值 Y:最大值")]
        [SerializeField] protected RotationType checkRotation;

        [Header("检测比例-容差比例")]
        [Tooltip("设置放缩比例的容差范围 X:最小值 Y:最大值")]
        [SerializeField] protected ScaleType checkScale;

        /// <summary>
        /// 待比较特征
        /// </summary>
        public Feature CompareFeature
        {
            get => compareFeature;
            set => compareFeature = value;
        }

        /// <summary>
        /// 设置比较参数
        /// </summary>
        public void Set(bool enablePosition, bool enableRotation, bool enableScale,
                        Vector2 checkPosition, float checkRadius, RotationType checkRotation, ScaleType checkScale)
        {
            this.enablePosition = enablePosition;
            this.enableRotation = enableRotation;
            this.enableScale = enableScale;
            this.checkPosition = checkPosition;
            this.checkRadius = checkRadius;
            this.checkRotation = checkRotation;
            this.checkScale = checkScale;
        }

        /// <summary>
        /// 检查条件是否满足
        /// </summary>
        /// <returns>是否满足条件</returns>
        public virtual bool Check() => true;

        /// <summary>
        /// 检测位置是否在相对半径范围内
        /// </summary>
        /// <returns>是否在范围内</returns>
        protected bool CheckPositionRadius()
        {
            if (!enablePosition)
                return true;

            if (compareFeature == null)
                return false;

            float distance = Vector3.Distance(checkPosition, compareFeature.Position);
            return distance <= checkRadius;
        }

        /// <summary>
        /// 检测旋转角度是否在容差范围内
        /// </summary>
        /// <returns>是否在范围内</returns>
        protected bool CheckRotation()
        {
            if (!enableRotation)
                return true;

            if (compareFeature == null)
                return false;

            return checkRotation == compareFeature.Rotation;
        }

        /// <summary>
        /// 检测缩放比例是否在容差范围内
        /// </summary>
        /// <returns>是否在范围内</returns>
        protected bool CheckScale()
        {
            if (!enableScale)
                return true;

            if (compareFeature == null)
                return false;

            return compareFeature.Scale == checkScale;
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

            if (!CheckRotation())
            {
                result = false;
            }

            if (!CheckScale())
            {
                result = false;
            }

            return result;
        }
    }
}
