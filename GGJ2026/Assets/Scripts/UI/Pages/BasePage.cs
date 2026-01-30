using UnityEngine;

namespace GGJ2026.UI
{
    /// <summary>
    /// 基础页面 - 提供常用UI功能的基类
    /// </summary>
    [RequireComponent(typeof(CanvasGroup), typeof(RectTransform))]
    public class BasePage : UIPage
    {
        [Header("基础页面设置")]
        [SerializeField] protected CanvasGroup canvasGroup;
        [SerializeField] protected RectTransform contentPanel;

        [Header("过渡动画设置")]
        [SerializeField] protected AnimationType showAnimationType = AnimationType.Fade;
        [SerializeField] protected AnimationType hideAnimationType = AnimationType.Fade;
        [SerializeField] protected float slideDistance = 200f;
        [SerializeField] protected float scaleFactor = 0.8f;

        protected Vector2 originalPosition;
        protected Vector3 originalScale;

        public enum AnimationType
        {
            Fade,
            SlideFromLeft,
            SlideFromRight,
            SlideFromTop,
            SlideFromBottom,
            Scale
        }

        protected void Awake()
        {
            InitializeComponents();
        }

        /// <summary>
        /// 初始化组件
        /// </summary>
        protected virtual void InitializeComponents()
        {
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();

            if (contentPanel == null)
                contentPanel = GetComponent<RectTransform>();

            // 保存原始位置和缩放
            if (contentPanel != null)
            {
                originalPosition = contentPanel.anchoredPosition;
                originalScale = contentPanel.localScale;
            }
        }

        /// <summary>
        /// 播放显示动画
        /// </summary>
        protected override void PlayShowAnimation(System.Action onComplete = null)
        {
            switch (showAnimationType)
            {
                case AnimationType.Fade:
                    PlayFadeAnimation(0f, 1f, showAnimationDuration, onComplete);
                    break;
                case AnimationType.SlideFromLeft:
                    PlaySlideAnimation(new Vector2(-slideDistance, 0), originalPosition, showAnimationDuration, onComplete);
                    break;
                case AnimationType.SlideFromRight:
                    PlaySlideAnimation(new Vector2(slideDistance, 0), originalPosition, showAnimationDuration, onComplete);
                    break;
                case AnimationType.SlideFromTop:
                    PlaySlideAnimation(new Vector2(0, slideDistance), originalPosition, showAnimationDuration, onComplete);
                    break;
                case AnimationType.SlideFromBottom:
                    PlaySlideAnimation(new Vector2(0, -slideDistance), originalPosition, showAnimationDuration, onComplete);
                    break;
                case AnimationType.Scale:
                    PlayScaleAnimation(Vector3.one * scaleFactor, originalScale, showAnimationDuration, onComplete);
                    break;
                default:
                    base.PlayShowAnimation(onComplete);
                    break;
            }
        }

        /// <summary>
        /// 播放隐藏动画
        /// </summary>
        protected override void PlayHideAnimation(System.Action onComplete = null)
        {
            switch (hideAnimationType)
            {
                case AnimationType.Fade:
                    PlayFadeAnimation(1f, 0f, hideAnimationDuration, onComplete);
                    break;
                case AnimationType.SlideFromLeft:
                    PlaySlideAnimation(originalPosition, new Vector2(-slideDistance, 0), hideAnimationDuration, onComplete);
                    break;
                case AnimationType.SlideFromRight:
                    PlaySlideAnimation(originalPosition, new Vector2(slideDistance, 0), hideAnimationDuration, onComplete);
                    break;
                case AnimationType.SlideFromTop:
                    PlaySlideAnimation(originalPosition, new Vector2(0, slideDistance), hideAnimationDuration, onComplete);
                    break;
                case AnimationType.SlideFromBottom:
                    PlaySlideAnimation(originalPosition, new Vector2(0, -slideDistance), hideAnimationDuration, onComplete);
                    break;
                case AnimationType.Scale:
                    PlayScaleAnimation(originalScale, Vector3.one * scaleFactor, hideAnimationDuration, onComplete);
                    break;
                default:
                    base.PlayHideAnimation(onComplete);
                    break;
            }
        }

        /// <summary>
        /// 播放淡入淡出动画
        /// </summary>
        protected virtual void PlayFadeAnimation(float fromAlpha, float toAlpha, float duration, System.Action onComplete = null)
        {
            if (canvasGroup != null)
            {
                StartCoroutine(FadeAnimationCoroutine(canvasGroup, fromAlpha, toAlpha, duration, onComplete));
            }
            else
            {
                onComplete?.Invoke();
            }
        }
        
        /// <summary>
        /// 淡入淡出动画协程
        /// </summary>
        private System.Collections.IEnumerator FadeAnimationCoroutine(CanvasGroup canvasGroup, float fromAlpha, float toAlpha, float duration, System.Action onComplete = null)
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
        /// 播放滑动动画
        /// </summary>
        protected virtual void PlaySlideAnimation(Vector2 fromPosition, Vector2 toPosition, float duration, System.Action onComplete = null)
        {
            if (contentPanel != null)
            {
                StartCoroutine(SlideAnimation(fromPosition, toPosition, duration, onComplete));
            }
            else
            {
                onComplete?.Invoke();
            }
        }

        /// <summary>
        /// 播放缩放动画
        /// </summary>
        protected virtual void PlayScaleAnimation(Vector3 fromScale, Vector3 toScale, float duration, System.Action onComplete = null)
        {
            if (contentPanel != null)
            {
                StartCoroutine(ScaleAnimation(fromScale, toScale, duration, onComplete));
            }
            else
            {
                onComplete?.Invoke();
            }
        }

        /// <summary>
        /// 滑动动画协程
        /// </summary>
        private System.Collections.IEnumerator SlideAnimation(Vector2 fromPosition, Vector2 toPosition, float duration, System.Action onComplete = null)
        {
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                contentPanel.anchoredPosition = Vector2.Lerp(fromPosition, toPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            contentPanel.anchoredPosition = toPosition;
            onComplete?.Invoke();
        }

        /// <summary>
        /// 缩放动画协程
        /// </summary>
        private System.Collections.IEnumerator ScaleAnimation(Vector3 fromScale, Vector3 toScale, float duration, System.Action onComplete = null)
        {
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                contentPanel.localScale = Vector3.Lerp(fromScale, toScale, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            contentPanel.localScale = toScale;
            onComplete?.Invoke();
        }

        /// <summary>
        /// 重置动画状态
        /// </summary>
        protected virtual void ResetAnimationState()
        {
            if (canvasGroup != null)
                canvasGroup.alpha = 1f;

            if (contentPanel != null)
            {
                contentPanel.anchoredPosition = originalPosition;
                contentPanel.localScale = originalScale;
            }
        }

        /// <summary>
        /// 页面显示时调用
        /// </summary>
        public override void OnShow(object data = null)
        {
            ResetAnimationState();
            base.OnShow(data);
        }
    }
}
