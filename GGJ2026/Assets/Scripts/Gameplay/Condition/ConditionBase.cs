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

        [Header("比较设置")]
        [Header("检测位置-相对位置")]
        [SerializeField] protected Vector2 checkPosition;

        [Header("检测位置-相对半径")]
        [SerializeField] protected float checkRadius;

        [Header("检测旋转-相对容差角度")]
        [Tooltip("设置旋转角度的容差范围 X:最小值 Y:最大值")]
        [SerializeField] protected Vector2 checkRotation;

        [Header("检测比例-容差比例")]
        [Tooltip("设置放缩比例的容差范围 X:最小值 Y:最大值")]
        [SerializeField] protected Vector2 checkScale;

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
        public void Set(Vector2 checkPosition, float checkRadius, Vector2 checkRotation, Vector2 checkScale)
        {
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
            if (compareFeature == null)
                return false;

            float distance = Vector3.Distance(checkPosition, compareFeature.Position);
            return distance <= checkRadius;
        }

        /// <summary>
        /// 检测旋转角度是否在容差范围内
        /// </summary>
        /// <returns>是否在范围内</returns>
        protected bool CheckRotationTolerance()
        {
            if (compareFeature == null)
                return false;

            float angle = Quaternion.Angle(new Quaternion(0, 0, 0, 1), compareFeature.Rotation);
            return angle >= checkRotation.x && angle <= checkRotation.y;
        }

        /// <summary>
        /// 检测缩放比例是否在容差范围内
        /// </summary>
        /// <returns>是否在范围内</returns>
        protected bool CheckScaleTolerance()
        {
            if (compareFeature == null)
                return false;
            return compareFeature.Scale.x >= checkScale.x &&
                   compareFeature.Scale.x <= checkScale.y;
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
