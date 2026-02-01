namespace GGJ2026.Gameplay.Condition
{
    /// <summary>
    /// 条件类型枚举
    /// </summary>
    public enum ConditionType
    {
        FeatureById,
        FeatureByType,
        FeatureExcludeId,
        FeatureExcludeType,
    }

    /// <summary>
    /// 逻辑与或枚举
    /// </summary>
    public enum LogicAndOr
    {
        None,
        And,
        Or,
    }
}
