using System.Collections.Generic;
using System.Linq;
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

        [Header("检测旋转-相对容差角度")]
        [Tooltip("设置旋转角度的容差范围 X:最小值 Y:最大值")]
        [SerializeField] protected List<RotationType> checkRotationList;

        [Header("检测比例-容差比例")]
        [Tooltip("设置放缩比例的容差范围 X:最小值 Y:最大值")]
        [SerializeField] protected List<ScaleType> checkScaleList;

        [Header("检测位置-相对位置")]
        [SerializeField] protected PositionType checkPosition;

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
                        PositionType checkPosition, List<RotationType> checkRotationList, List<ScaleType> checkScaleList)
        {
            this.enablePosition = enablePosition;
            this.enableRotation = enableRotation;
            this.enableScale = enableScale;
            this.checkPosition = checkPosition;
            this.checkScaleList = checkScaleList;
            this.checkRotationList = checkRotationList;
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

            return checkPosition == compareFeature.Position;
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

            return checkRotationList.Any(checkRotation => checkRotation == compareFeature.Rotation);
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

            return checkScaleList.Any(checkScale => checkScale == compareFeature.Scale);
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
