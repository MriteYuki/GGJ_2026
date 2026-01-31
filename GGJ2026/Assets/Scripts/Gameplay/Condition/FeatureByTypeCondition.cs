using System.Collections.Generic;
using UnityEngine;

namespace GGJ2026.Gameplay.Condition
{
    /// <summary>
    /// 按类型查找特征的条件
    /// </summary>
    [System.Serializable]
    public class FeatureByTypeCondition : ConditionBase
    {
        private FeatureType featureType;

        public FeatureByTypeCondition(FeatureType featureType)
        {
            this.featureType = featureType;
        }

        /// <summary>
        /// 检查条件是否满足
        /// </summary>
        public override bool Check()
        {
            // 比较目标Type的特征
            if (compareFeature.Type != featureType)
            {
                return false;
            }

            return CheckAllParameters();
        }
    }
}
