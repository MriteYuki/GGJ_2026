using GGJ2026.FaceComponent;
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
        /// <summary>
        /// 检查条件是否满足
        /// </summary>
        public override bool Check()
        {
            // 比较目标Type的特征
            if (compareFeature.Type != targetFeature.Type)
            {
                return false;
            }

            return CheckAllParameters();
        }
    }
}
