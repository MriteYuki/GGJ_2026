using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ2026.UI
{
    /// <summary>
    /// UI事件系统 - 实现发布订阅模式
    /// </summary>
    public class UIEventSystem : MonoBehaviour
    {
        private static UIEventSystem instance;
        private Dictionary<string, List<Action<object>>> eventHandlers;
        
        public static UIEventSystem Instance
        {
            get
            {
                if (instance == null)
                {
                    var go = new GameObject("UIEventSystem");
                    instance = go.AddComponent<UIEventSystem>();
                }
                return instance;
            }
        }

        public static bool IsExist => instance != null;
        
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                eventHandlers = new Dictionary<string, List<Action<object>>>();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// 订阅事件
        /// </summary>
        public void Subscribe(string eventName, Action<object> handler)
        {
            if (!eventHandlers.ContainsKey(eventName))
            {
                eventHandlers[eventName] = new List<Action<object>>();
            }
            
            if (!eventHandlers[eventName].Contains(handler))
            {
                eventHandlers[eventName].Add(handler);
                Debug.Log($"事件订阅: {eventName}");
            }
        }
        
        /// <summary>
        /// 取消订阅
        /// </summary>
        public void Unsubscribe(string eventName, Action<object> handler)
        {
            if (eventHandlers.ContainsKey(eventName))
            {
                eventHandlers[eventName].Remove(handler);
                Debug.Log($"事件取消订阅: {eventName}");
            }
        }
        
        /// <summary>
        /// 发布事件
        /// </summary>
        public void Publish(string eventName, object data = null)
        {
            if (eventHandlers.ContainsKey(eventName))
            {
                // 使用标记列表来跟踪需要执行的处理程序
                var handlersToExecute = new List<Action<object>>();
                
                // 首先收集所有需要执行的处理程序
                foreach (var handler in eventHandlers[eventName])
                {
                    handlersToExecute.Add(handler);
                }
                
                // 然后执行收集到的处理程序
                foreach (var handler in handlersToExecute)
                {
                    // 检查处理程序是否仍然在订阅列表中（可能在其他处理程序中已被取消）
                    if (eventHandlers.ContainsKey(eventName) && 
                        eventHandlers[eventName].Contains(handler))
                    {
                        try
                        {
                            handler?.Invoke(data);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"事件处理错误 {eventName}: {e.Message}");
                        }
                    }
                }
                
                Debug.Log($"事件发布: {eventName}");
            }
        }
        
        /// <summary>
        /// 清空所有事件订阅
        /// </summary>
        public void ClearAll()
        {
            eventHandlers.Clear();
            Debug.Log("所有事件订阅已清空");
        }
        
        /// <summary>
        /// 检查是否有订阅者
        /// </summary>
        public bool HasSubscribers(string eventName)
        {
            return eventHandlers.ContainsKey(eventName) && eventHandlers[eventName].Count > 0;
        }
    }
    
    /// <summary>
    /// 常用UI事件定义
    /// </summary>
    public static class UIEventTypes
    {
        public const string DESC_SHOW = "desc_show";
        public const string DESC_HIDE = "desc_hide";
        public const string BUTTON_CLICK = "button_click";
        public const string PAGE_CHANGE = "page_change";
        public const string SETTINGS_CHANGED = "settings_changed";
        public const string GAME_STATE_CHANGED = "game_state_changed";
        public const string UI_ANIMATION_COMPLETE = "ui_animation_complete";
    }
}