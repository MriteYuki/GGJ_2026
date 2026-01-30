using UnityEngine;
using System.Collections.Generic;

namespace GGJ2026.UI.Test
{
    /// <summary>
    /// UI测试配置 - 存储测试场景和页面配置
    /// </summary>
    [CreateAssetMenu(fileName = "UITestConfig", menuName = "GGJ2026/UI/Test Config")]
    public class UITestConfig : ScriptableObject
    {
        [Header("测试场景配置")]
        [Tooltip("测试场景名称列表，确保场景文件存在于项目中")]
        public string[] testScenes = new string[]
        {
            "UITest_BasePages",
            "UITest_Animations", 
            "UITest_DataBinding",
            "UITest_EventSystem"
        };
        
        [Header("页面动画配置")]
        [Tooltip("各页面的动画配置，页面名称需与实际的页面类名一致")]
        public PageAnimationConfig[] pageAnimations = new PageAnimationConfig[]
        {
            new PageAnimationConfig { pageName = "HomePage", showAnimation = BasePage.AnimationType.Fade, hideAnimation = BasePage.AnimationType.Fade },
            new PageAnimationConfig { pageName = "SettingsPage", showAnimation = BasePage.AnimationType.SlideFromRight, hideAnimation = BasePage.AnimationType.SlideFromRight },
            new PageAnimationConfig { pageName = "GamePage", showAnimation = BasePage.AnimationType.Scale, hideAnimation = BasePage.AnimationType.Scale }
        };
        
        [Header("测试数据")]
        [Tooltip("测试数据集，用于不同测试场景的数据配置")]
        public TestData[] testDataSets = new TestData[]
        {
            new TestData { dataName = "基础测试", parameters = new Dictionary<string, object>() },
            new TestData { dataName = "高级测试", parameters = new Dictionary<string, object>() }
        };
        
        [Header("性能测试设置")]
        [Range(1, 1000)]
        [Tooltip("性能测试迭代次数，建议在10-1000之间")]
        public int performanceTestIterations = 100;
        
        [Range(0.01f, 1.0f)]
        [Tooltip("性能测试延迟时间（秒），避免过快的测试导致结果不准确")]
        public float performanceTestDelay = 0.1f;
        
        /// <summary>
        /// 获取指定页面的动画配置
        /// </summary>
        /// <param name="pageName">页面名称</param>
        /// <returns>动画配置，如果未找到返回null</returns>
        public PageAnimationConfig GetPageAnimationConfig(string pageName)
        {
            if (pageAnimations == null) return null;
            
            foreach (var config in pageAnimations)
            {
                if (config.pageName == pageName)
                {
                    return config;
                }
            }
            return null;
        }
        
        /// <summary>
        /// 验证配置的有效性
        /// </summary>
        /// <returns>验证结果</returns>
        public bool ValidateConfig()
        {
            if (testScenes == null || testScenes.Length == 0)
            {
                Debug.LogWarning("UITestConfig: 测试场景配置为空");
                return false;
            }
            
            if (pageAnimations == null || pageAnimations.Length == 0)
            {
                Debug.LogWarning("UITestConfig: 页面动画配置为空");
                return false;
            }
            
            if (performanceTestIterations < 1)
            {
                Debug.LogWarning("UITestConfig: 性能测试迭代次数必须大于0");
                return false;
            }
            
            if (performanceTestDelay <= 0)
            {
                Debug.LogWarning("UITestConfig: 性能测试延迟时间必须大于0");
                return false;
            }
            
            return true;
        }
    }
    
    /// <summary>
    /// 页面动画配置
    /// </summary>
    [System.Serializable]
    public class PageAnimationConfig
    {
        [Tooltip("页面名称，需与实际的页面类名一致")]
        public string pageName;
        
        [Tooltip("显示动画类型")]
        public BasePage.AnimationType showAnimation;
        
        [Tooltip("隐藏动画类型")]
        public BasePage.AnimationType hideAnimation;
        
        [Range(0.1f, 2.0f)]
        [Tooltip("动画时长（秒），建议在0.1-2.0秒之间")]
        public float animationDuration = 0.3f;
        
        /// <summary>
        /// 验证配置的有效性
        /// </summary>
        /// <returns>是否有效</returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(pageName) && animationDuration > 0;
        }
    }
    
    /// <summary>
    /// 测试数据
    /// </summary>
    [System.Serializable]
    public class TestData
    {
        [Tooltip("测试数据名称，用于标识不同的测试数据集")]
        public string dataName;
        
        [Tooltip("测试参数键值对，支持多种数据类型")]
        public Dictionary<string, object> parameters;
        
        /// <summary>
        /// 获取指定参数的值，如果不存在返回默认值
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="key">参数键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>参数值</returns>
        public T GetParameter<T>(string key, T defaultValue = default)
        {
            if (parameters != null && parameters.ContainsKey(key))
            {
                try
                {
                    return (T)parameters[key];
                }
                catch (System.Exception)
                {
                    Debug.LogWarning($"UITestConfig: 参数 {key} 类型转换失败，使用默认值");
                    return defaultValue;
                }
            }
            return defaultValue;
        }
        
        /// <summary>
        /// 设置参数值
        /// </summary>
        /// <param name="key">参数键</param>
        /// <param name="value">参数值</param>
        public void SetParameter(string key, object value)
        {
            if (parameters == null)
            {
                parameters = new Dictionary<string, object>();
            }
            parameters[key] = value;
        }
        
        /// <summary>
        /// 验证测试数据的有效性
        /// </summary>
        /// <returns>是否有效</returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(dataName);
        }
    }
}