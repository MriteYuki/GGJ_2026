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

        [Header("变换属性")]
        [SerializeField] private Vector3 position;
        [SerializeField] private Vector3 scale;
        [SerializeField] private Quaternion rotation;

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
            get => position;
            set => position = value;
        }

        /// <summary>
        /// 比例
        /// </summary>
        public Vector3 Scale
        {
            get => scale;
            set => scale = value;
        }

        /// <summary>
        /// 旋转
        /// </summary>
        public Quaternion Rotation
        {
            get => rotation;
            set => rotation = value;
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public Feature()
        {
            featureID = System.Guid.NewGuid().ToString();
            position = Vector3.zero;
            scale = Vector3.one;
            rotation = Quaternion.identity;
        }

        /// <summary>
        /// 带参数的构造函数
        /// </summary>
        /// <param name="type">特征类型</param>
        /// <param name="id">唯一标识（可选，不提供则自动生成）</param>
        public Feature(FeatureType type, string id = null)
        {
            featureType = type;
            featureID = id ?? System.Guid.NewGuid().ToString();
            position = Vector3.zero;
            scale = Vector3.one;
            rotation = Quaternion.identity;
        }

        /// <summary>
        /// 完整参数的构造函数
        /// </summary>
        public Feature(FeatureType type, Vector3 position, Vector3 scale, Quaternion rotation, string id = null)
        {
            featureType = type;
            featureID = id ?? System.Guid.NewGuid().ToString();
            this.position = position;
            this.scale = scale;
            this.rotation = rotation;
        }

        /// <summary>
        /// 获取旋转的欧拉角度
        /// </summary>
        public Vector3 EulerAngles
        {
            get => rotation.eulerAngles;
            set => rotation = Quaternion.Euler(value);
        }

        /// <summary>
        /// 创建变换矩阵
        /// </summary>
        public Matrix4x4 GetTransformMatrix()
        {
            return Matrix4x4.TRS(position, rotation, scale);
        }

        /// <summary>
        /// 应用变换到Transform组件
        /// </summary>
        public void ApplyToTransform(Transform targetTransform)
        {
            if (targetTransform != null)
            {
                targetTransform.position = position;
                targetTransform.localScale = scale;
                targetTransform.rotation = rotation;
            }
        }

        /// <summary>
        /// 从Transform组件获取数据
        /// </summary>
        public void SetFromTransform(Transform sourceTransform)
        {
            if (sourceTransform != null)
            {
                position = sourceTransform.position;
                scale = sourceTransform.localScale;
                rotation = sourceTransform.rotation;
            }
        }

        /// <summary>
        /// 复制特征数据
        /// </summary>
        public Feature Copy()
        {
            return new Feature(featureType, position, scale, rotation, featureID + "_copy");
        }

        public override string ToString()
        {
            return $"Feature {{Type: {featureType}, ID: {featureID}, Position: {position}, Scale: {scale}, Rotation: {rotation.eulerAngles}}}";
        }
    }
}
