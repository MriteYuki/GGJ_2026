using System;
using UnityEngine;
using UnityEngine.UI;

namespace GGJ2026.UI
{
    /// <summary>
    /// 数据绑定系统 - 实现简单的数据到UI的绑定
    /// </summary>
    public class UIDataBinding : MonoBehaviour
    {
        [System.Serializable]
        public class Binding
        {
            public string propertyName;
            public Component targetComponent;
            public string targetProperty;
        }
        
        [Header("数据绑定设置")]
        [SerializeField] private MonoBehaviour dataSource;
        [SerializeField] private Binding[] bindings;
        
        private void Start()
        {
            SetupBindings();
        }
        
        /// <summary>
        /// 设置数据绑定
        /// </summary>
        private void SetupBindings()
        {
            if (dataSource == null)
            {
                Debug.LogWarning("数据源未设置");
                return;
            }
            
            // 这里可以实现更复杂的数据绑定逻辑
            // 目前是简化版本，需要手动更新
        }
        
        /// <summary>
        /// 更新所有绑定
        /// </summary>
        public void UpdateBindings()
        {
            foreach (var binding in bindings)
            {
                if (binding.targetComponent != null)
                {
                    UpdateBinding(binding);
                }
            }
        }
        
        /// <summary>
        /// 更新单个绑定
        /// </summary>
        private void UpdateBinding(Binding binding)
        {
            try
            {
                var dataValue = GetPropertyValue(dataSource, binding.propertyName);
                SetPropertyValue(binding.targetComponent, binding.targetProperty, dataValue);
            }
            catch (Exception e)
            {
                Debug.LogError($"数据绑定错误: {e.Message}");
            }
        }
        
        /// <summary>
        /// 获取属性值
        /// </summary>
        private object GetPropertyValue(object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName);
            if (property != null)
            {
                return property.GetValue(obj);
            }
            
            var field = obj.GetType().GetField(propertyName);
            if (field != null)
            {
                return field.GetValue(obj);
            }
            
            throw new Exception($"属性或字段未找到: {propertyName}");
        }
        
        /// <summary>
        /// 设置属性值
        /// </summary>
        private void SetPropertyValue(Component component, string propertyName, object value)
        {
            // 处理常见UI组件
            if (component is Text textComponent && propertyName == "text")
            {
                textComponent.text = value?.ToString() ?? "";
            }
            else if (component is Slider slider && propertyName == "value")
            {
                if (value is float floatValue)
                {
                    slider.value = floatValue;
                }
            }
            else if (component is Image image && propertyName == "fillAmount")
            {
                if (value is float fillValue)
                {
                    image.fillAmount = fillValue;
                }
            }
            else
            {
                // 使用反射设置其他属性
                var property = component.GetType().GetProperty(propertyName);
                if (property != null && property.CanWrite)
                {
                    property.SetValue(component, value);
                }
            }
        }
        
        /// <summary>
        /// 设置数据源
        /// </summary>
        public void SetDataSource(MonoBehaviour newDataSource)
        {
            dataSource = newDataSource;
            UpdateBindings();
        }
        
        /// <summary>
        /// 添加绑定
        /// </summary>
        public void AddBinding(string propertyName, Component target, string targetProperty)
        {
            var newBinding = new Binding
            {
                propertyName = propertyName,
                targetComponent = target,
                targetProperty = targetProperty
            };
            
            Array.Resize(ref bindings, bindings.Length + 1);
            bindings[bindings.Length - 1] = newBinding;
        }
    }
    
    /// <summary>
    /// 数据绑定扩展方法
    /// </summary>
    public static class DataBindingExtensions
    {
        /// <summary>
        /// 绑定文本到属性
        /// </summary>
        public static void BindText(this Text text, MonoBehaviour source, string propertyName)
        {
            var binding = text.gameObject.AddComponent<UIDataBinding>();
            binding.AddBinding(propertyName, text, "text");
            binding.SetDataSource(source);
        }
        
        /// <summary>
        /// 绑定滑块到属性
        /// </summary>
        public static void BindSlider(this Slider slider, MonoBehaviour source, string propertyName)
        {
            var binding = slider.gameObject.AddComponent<UIDataBinding>();
            binding.AddBinding(propertyName, slider, "value");
            binding.SetDataSource(source);
        }
    }
}