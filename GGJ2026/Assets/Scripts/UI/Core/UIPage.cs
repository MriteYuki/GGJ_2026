using UnityEngine;

namespace GGJ2026.UI
{
    /// <summary>
    /// 页面基类 - 所有UI页面的基础
    /// </summary>
    public abstract class UIPage : MonoBehaviour
    {
        [Header("页面设置")]
        [SerializeField] protected string pageName;
        [SerializeField] protected bool canGoBack = true;
        
        [Header("动画设置")]
        [SerializeField] protected float showAnimationDuration = 0.3f;
        [SerializeField] protected float hideAnimationDuration = 0.3f;
        
        /// <summary>
        /// 页面名称
        /// </summary>
        public string PageName => string.IsNullOrEmpty(pageName) ? GetType().Name : pageName;
        
        /// <summary>
        /// 是否可以返回
        /// </summary>
        public bool CanGoBack => canGoBack;
        
        /// <summary>
        /// 显示动画时长
        /// </summary>
        public float ShowAnimationDuration => showAnimationDuration;
        
        /// <summary>
        /// 隐藏动画时长
        /// </summary>
        public float HideAnimationDuration => hideAnimationDuration;
        
        /// <summary>
        /// 页面显示时调用
        /// </summary>
        /// <param name="data">传递的数据</param>
        public virtual void OnShow(object data = null)
        {
            Debug.Log($"页面显示: {PageName}");
            
            // 激活所有子对象
            gameObject.SetActive(true);
            
            // 播放显示动画（如果有）
            PlayShowAnimation();
        }
        
        /// <summary>
        /// 页面隐藏时调用
        /// </summary>
        public virtual void OnHide()
        {
            Debug.Log($"页面隐藏: {PageName}");
            
            // 播放隐藏动画（如果有），动画完成后停用对象
            PlayHideAnimation(() =>
            {
                gameObject.SetActive(false);
            });
        }
        
        /// <summary>
        /// 页面更新时调用
        /// </summary>
        public virtual void OnUpdate()
        {
            // 子类可以重写此方法实现每帧更新
        }
        
        /// <summary>
        /// 播放显示动画
        /// </summary>
        protected virtual void PlayShowAnimation(System.Action onComplete = null)
        {
            // 默认实现：淡入效果（使用协程实现）
            var canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, showAnimationDuration, onComplete));
            }
            else
            {
                onComplete?.Invoke();
            }
        }
        
        /// <summary>
        /// 播放隐藏动画
        /// </summary>
        protected virtual void PlayHideAnimation(System.Action onComplete = null)
        {
            // 默认实现：淡出效果（使用协程实现）
            var canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0f, hideAnimationDuration, onComplete));
            }
            else
            {
                onComplete?.Invoke();
            }
        }
        
        /// <summary>
        /// 淡入淡出协程
        /// </summary>
        protected System.Collections.IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float fromAlpha, float toAlpha, float duration, System.Action onComplete = null)
        {
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                canvasGroup.alpha = Mathf.Lerp(fromAlpha, toAlpha, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            canvasGroup.alpha = toAlpha;
            onComplete?.Invoke();
        }
        
        /// <summary>
        /// 返回上一页
        /// </summary>
        public void GoBack()
        {
            if (CanGoBack && UIManager.Instance != null)
            {
                UIManager.Instance.Back();
            }
        }
        
        /// <summary>
        /// 显示其他页面
        /// </summary>
        protected void ShowPage(string pageName, object data = null)
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowPage(pageName, data);
            }
        }
        
        void Update()
        {
            OnUpdate();
        }
    }
}