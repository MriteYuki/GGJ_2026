using System.Collections.Generic;
using UnityEngine;

namespace GGJ2026.Gameplay.Condition
{
    /// <summary>
    /// 按特定ID查找特征的条件
    /// </summary>
    [System.Serializable]
    public class FeatureByIdCondition : ConditionBase
    {
        [SerializeField] private string featureId;

        public FeatureByIdCondition(string featureId)
        {
            this.featureId = featureId;
        }

        /// <summary>
        /// 检查条件是否满足
        /// </summary>
        public override bool Check()
        {
            // 比较目标ID的特征
            if (compareFeature.ID != featureId)
            {
                return false;
            }

            return CheckAllParameters();
        }
    }
}
