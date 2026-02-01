using UnityEngine;

namespace GGJ2026.Gameplay.Condition
{
    /// <summary>
    /// 按特定ID排除特征的条件
    /// </summary>
    public class FeatureExcludeIdCondition : ConditionBase
    {
        [SerializeField] private string featureId;

        public FeatureExcludeIdCondition(string featureId)
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
                return true;
            }
 ;
            return CheckPositionRadius() is false;
        }
    }
}
