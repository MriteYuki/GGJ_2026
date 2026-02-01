using TMPro;
using UnityEngine;

namespace GGJ2026
{
    /// <summary>
    /// 面部特征类型枚举
    /// </summary>
    public enum FeatureType
    {
        Eye,
        Ear,
        Mouth,
        Nose,
        Face
    }

    /// <summary>
    /// 旋转方向类型（八个方向）
    /// </summary>
    public enum RotationType
    {
        Up,      // 0° - 北
        UpRight,  // -45° - 东北
        Right,       // -90° - 东
        DownRight,  // -135° - 东南
        Down,      // -180° - 南
        DownLeft,  // -225° - 西南
        Left,       // -270° - 西
        UpLeft   // -315° - 西北
    }

    /// <summary>
    /// 比例类型
    /// </summary>
    public enum ScaleType
    {
        Small,
        Medium,
        Large
    }

    /// <summary>
    /// 面部特征数据类
    /// </summary>
    [System.Serializable]
    public class Feature : MonoBehaviour
    {
        [Header("基础信息")]
        [SerializeField] private FeatureType featureType;
        [SerializeField] private string featureID;

        [SerializeField] private RotationType rotationType = RotationType.Up;

        [SerializeField] private ScaleType scaleType = ScaleType.Medium;

        [SerializeField] private string name;

        [SerializeField, TextArea(3, 10)] private string description;

        public string Name => name;

        public string Description => description;

        /// <summary>
        /// 面部特征类型
        /// </summary>
        public FeatureType Type
        {
            get => featureType;
            set => featureType = value;
        }

        /// <summary>
        /// 面部特征的唯一标识
        /// </summary>
        public string ID
        {
            get => featureID;
            set => featureID = value;
        }

        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 Position
        {
            get => transform.localPosition;
        }

        /// <summary>
        /// 比例
        /// </summary>
        public ScaleType Scale
        {
            get => scaleType;
            set => scaleType = value;
        }

        /// <summary>
        /// 旋转
        /// </summary>
        public RotationType Rotation
        {
            get => rotationType;
            set => rotationType = value;
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Feature()
        {
            featureID = string.Empty;
        }

        /// <summary>
        /// 带参数的构造函数
        /// </summary>
        /// <param name="type">特征类型</param>
        /// <param name="id">唯一标识（可选，不提供则自动生成）</param>
        public Feature(FeatureType type, string id = null)
        {
            featureType = type;
            featureID = id ?? string.Empty;
        }

        public override string ToString()
        {
            return $"Feature {{Type: {featureType}, ID: {featureID}, Position: {Position}, Scale: {Scale}, Rotation: {Rotation}}}";
        }

#if UNITY_EDITOR

        public void OnValidate()
        {
            var angles = rotationType switch
            {
                RotationType.Up => Vector3.zero,

                RotationType.UpRight => new Vector3(0, 0, -45),

                RotationType.Right => new Vector3(0, 0, -90),

                RotationType.DownRight => new Vector3(0, 0, -135),

                RotationType.Down => new Vector3(0, 0, -180),

                RotationType.DownLeft => new Vector3(0, 0, -225),

                RotationType.Left => new Vector3(0, 0, -270),

                RotationType.UpLeft => new Vector3(0, 0, -315),

                _ => Vector3.zero
            };

            transform.eulerAngles = angles;

            var scale = scaleType switch
            {

                ScaleType.Small => Vector3.one * 0.7f,
                ScaleType.Medium => Vector3.one,
                ScaleType.Large => Vector3.one * 1.4f,
                _ => Vector3.one
            };

            transform.localScale = scale;
        }
#endif
    }
}
