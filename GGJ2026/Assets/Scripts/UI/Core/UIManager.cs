using System.Collections.Generic;
using UnityEngine;

namespace GGJ2026.UI
{
    /// <summary>
    /// UI管理器 - 负责页面管理和导航
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("UI设置")]
        [SerializeField] private Transform pageContainer;
        [SerializeField] private Canvas mainCanvas;

        private Dictionary<string, UIPage> pageRegistry = new Dictionary<string, UIPage>();
        private Stack<UIPage> pageStack = new Stack<UIPage>();
        private UIPage currentPage;

        public static UIManager Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Initialize()
        {
            // 注册所有页面
            RegisterAllPages();
        }

        /// <summary>
        /// 注册页面
        /// </summary>
        public void RegisterPage(string pageName, UIPage page)
        {
            if (!pageRegistry.ContainsKey(pageName))
            {
                pageRegistry[pageName] = page;
                page.gameObject.SetActive(false);
                Debug.Log($"页面注册成功: {pageName}");
            }
        }

        /// <summary>
        /// 显示页面
        /// </summary>
        public void ShowPage(string pageName, object data = null)
        {
            if (pageRegistry.TryGetValue(pageName, out UIPage page))
            {
                if (currentPage != null)
                {
                    // 先隐藏当前页面，动画完成后显示新页面
                    currentPage.OnHide();
                    StartCoroutine(WaitForHideAnimationThenShow(currentPage, page, data, pageName));
                }
                else
                {
                    // 没有当前页面，直接显示新页面
                    currentPage = page;
                    pageStack.Push(page);
                    page.gameObject.SetActive(true);
                    page.OnShow(data);
                    Debug.Log($"显示页面: {pageName}");
                }
            }
            else
            {
                Debug.LogWarning($"页面未找到: {pageName}");
            }
        }

        /// <summary>
        /// 返回上一页
        /// </summary>
        public void Back()
        {
            if (pageStack.Count > 1)
            {
                var current = pageStack.Pop();
                var previous = pageStack.Peek();

                // 隐藏当前页面，动画完成后显示上一页
                current.OnHide();
                StartCoroutine(WaitForHideAnimationThenBack(current, previous));
            }
        }

        /// <summary>
        /// 获取当前页面
        /// </summary>
        public UIPage GetCurrentPage()
        {
            return currentPage;
        }

        /// <summary>
        /// 注册所有页面（自动扫描）
        /// </summary>
        private void RegisterAllPages()
        {
            // 初始扫描已存在的页面
            var existingPages = GetComponentsInChildren<UIPage>(true);
            foreach (var page in existingPages)
            {
                RegisterPage(page.GetType().Name, page);
            }

            Debug.Log($"初始注册 {existingPages.Length} 个页面");
        }

        /// <summary>
        /// 动态加载并注册页面
        /// </summary>
        public void LoadAndRegisterPage(string pagePrefabPath, string pageName)
        {
            if (pageRegistry.ContainsKey(pageName))
            {
                Debug.LogWarning($"页面已存在: {pageName}");
                return;
            }

            // 从Resources加载页面预制体
            var pagePrefab = Resources.Load<GameObject>(pagePrefabPath);
            if (pagePrefab == null)
            {
                Debug.LogError($"页面预制体加载失败: {pagePrefabPath}");
                return;
            }

            // 实例化页面
            var pageObject = Instantiate(pagePrefab, pageContainer ?? transform);
            var page = pageObject.GetComponent<UIPage>();

            if (page != null)
            {
                RegisterPage(pageName, page);
                Debug.Log($"动态加载页面成功: {pageName}");
            }
            else
            {
                Debug.LogError($"页面组件未找到: {pagePrefabPath}");
                Destroy(pageObject);
            }
        }

        /// <summary>
        /// 动态创建并注册页面
        /// </summary>
        public void CreateAndRegisterPage<T>(string pageName) where T : UIPage
        {
            if (pageRegistry.ContainsKey(pageName))
            {
                Debug.LogWarning($"页面已存在: {pageName}");
                return;
            }

            // 创建空的GameObject并添加页面组件
            var pageObject = new GameObject(pageName);
            pageObject.transform.SetParent(pageContainer ?? transform, false);

            var page = pageObject.AddComponent<T>();
            RegisterPage(pageName, page);

            Debug.Log($"动态创建页面成功: {pageName}");
        }

        /// <summary>
        /// 卸载并销毁页面
        /// </summary>
        public void UnregisterPage(string pageName)
        {
            if (pageRegistry.TryGetValue(pageName, out UIPage page))
            {
                // 如果页面是当前页面，需要先隐藏
                if (currentPage == page)
                {
                    // 从页面栈中移除
                    var tempStack = new Stack<UIPage>();
                    while (pageStack.Count > 0 && pageStack.Peek() != page)
                    {
                        tempStack.Push(pageStack.Pop());
                    }

                    if (pageStack.Count > 0)
                    {
                        pageStack.Pop(); // 移除目标页面
                    }

                    // 恢复栈
                    while (tempStack.Count > 0)
                    {
                        pageStack.Push(tempStack.Pop());
                    }

                    // 更新当前页面
                    if (pageStack.Count > 0)
                    {
                        currentPage = pageStack.Peek();
                    }
                    else
                    {
                        currentPage = null;
                    }
                }

                // 从注册表中移除
                pageRegistry.Remove(pageName);

                // 销毁页面对象
                if (page != null && page.gameObject != null)
                {
                    Destroy(page.gameObject);
                }

                Debug.Log($"卸载页面成功: {pageName}");
            }
            else
            {
                Debug.LogWarning($"页面未找到: {pageName}");
            }
        }

        /// <summary>
        /// 设置UI渲染模式
        /// </summary>
        public void SetRenderMode(RenderMode mode)
        {
            if (mainCanvas != null)
            {
                mainCanvas.renderMode = mode;
            }
        }

        /// <summary>
        /// 清空页面栈（用于重置状态）
        /// </summary>
        public void ClearStack()
        {
            pageStack.Clear();
        }

        /// <summary>
        /// 等待隐藏动画完成后显示新页面
        /// </summary>
        private System.Collections.IEnumerator WaitForHideAnimationThenShow(UIPage oldPage, UIPage newPage, object data, string pageName)
        {
            // 等待一帧确保动画开始
            yield return null;

            // 使用页面实际的隐藏动画时长
            float animationDuration = oldPage != null ? oldPage.HideAnimationDuration : 0.3f;
            yield return new WaitForSeconds(animationDuration);

            // 检查页面是否仍然有效（防止在动画期间被销毁）
            if (oldPage != null && oldPage.gameObject != null)
            {
                oldPage.gameObject.SetActive(false);
            }

            // 更新状态管理
            currentPage = newPage;
            pageStack.Push(newPage);

            if (newPage != null && newPage.gameObject != null)
            {
                newPage.gameObject.SetActive(true);
                newPage.OnShow(data);
            }

            Debug.Log($"显示页面: {pageName}");
        }

        /// <summary>
        /// 等待隐藏动画完成后显示上一页
        /// </summary>
        private System.Collections.IEnumerator WaitForHideAnimationThenBack(UIPage current, UIPage previous)
        {
            // 等待一帧确保动画开始
            yield return null;

            // 使用当前页面的隐藏动画时长
            float animationDuration = current != null ? current.HideAnimationDuration : 0.3f;
            yield return new WaitForSeconds(animationDuration);

            // 检查页面是否仍然有效（防止在动画期间被销毁）
            if (current != null && current.gameObject != null)
            {
                current.gameObject.SetActive(false);
            }

            // 显示上一页
            if (previous != null && previous.gameObject != null)
            {
                previous.gameObject.SetActive(true);
                previous.OnShow(null);
            }

            currentPage = previous;
            Debug.Log("返回上一页");
        }
    }
}
