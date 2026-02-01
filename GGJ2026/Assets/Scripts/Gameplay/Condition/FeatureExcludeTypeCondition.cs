using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGJ2026.Gameplay.Condition
{
    public class FeatureExcludeTypeCondition : ConditionBase
    {
        private FeatureType featureType;

        public FeatureExcludeTypeCondition(FeatureType featureType)
        {
            this.featureType = featureType;
        }

        /// <summary>
        /// 检查条件是否满足
        /// </summary>
        public override bool Check()
        {
            // 比较目标Type的特征
            if (compareFeature.Type == featureType)
            {
                return false;
            }

            return CheckAllParameters();
        }
    }
}
