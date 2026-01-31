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
    /// 面部特征数据类
    /// </summary>
    [System.Serializable]
    public class Feature : MonoBehaviour
    {
        [Header("基础信息")]
        [SerializeField] private FeatureType featureType;
        [SerializeField] private string featureID;


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
        public Vector3 Scale
        {
            get => transform.localScale;
        }

        /// <summary>
        /// 旋转
        /// </summary>
        public Quaternion Rotation
        {
            get => transform.localRotation;
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

        /// <summary>
        /// 获取旋转的欧拉角度
        /// </summary>
        public Vector3 EulerAngles
        {
            get => Rotation.eulerAngles;
        }

        /// <summary>
        /// 创建变换矩阵
        /// </summary>
        public Matrix4x4 GetTransformMatrix()
        {
            return Matrix4x4.TRS(Position, Rotation, Scale);
        }

        /// <summary>
        /// 复制特征数据
        /// </summary>
        public Feature Copy()
        {
            return new(featureType, featureID);
        }

        public override string ToString()
        {
            return $"Feature {{Type: {featureType}, ID: {featureID}, Position: {Position}, Scale: {Scale}, Rotation: {Rotation.eulerAngles}}}";
        }
    }
}
