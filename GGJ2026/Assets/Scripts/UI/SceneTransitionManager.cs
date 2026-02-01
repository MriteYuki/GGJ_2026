using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GGJ2026.Gameplay;

namespace GGJ2026.UI
{
    /// <summary>
    /// 场景过渡管理器 - 提供黑屏过场动画
    /// </summary>
    public class SceneTransitionManager : MonoBehaviour
    {
        [Header("过渡设置")]
        [SerializeField] private float fadeInDuration = 0.5f;
        [SerializeField] private float fadeOutDuration = 0.5f;
        [SerializeField] private float minDisplayDuration = 1.0f;

        [Header("UI组件")]
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private Image fadeImage;

        [Header("颜色设置")]
        [SerializeField] private Color fadeColor = Color.black;

        [Header("调试设置")]
        [SerializeField] private bool debugMode = false;

        private static SceneTransitionManager instance;
        private bool isTransitioning = false;
        private Coroutine currentTransitionCoroutine;
        private string targetSceneName;
        private float loadingProgress = 0f;

        #region 单例模式

        public static SceneTransitionManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<SceneTransitionManager>();
                    if (instance == null)
                    {
                        GameObject go = new("SceneTransitionManager");
                        instance = go.AddComponent<SceneTransitionManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return instance;
            }
        }

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        #endregion

        #region 初始化

        private void Initialize()
        {
            var canvas = CreateTransitionUI();
            // 确保UI在最顶层
            canvas.sortingOrder = 9999;
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;

            if (fadeImage != null)
            {
                fadeImage.color = fadeColor;
            }

            // 注册事件监听
            RegisterEventListeners();
        }

        private void RegisterEventListeners()
        {
            // 监听GameManager发布的场景转换事件
            UIEventSystem.Instance.Subscribe(GameManager.SceneTransitionEvents.TRANSITION_START, OnTransitionStart);
            UIEventSystem.Instance.Subscribe(GameManager.SceneTransitionEvents.TRANSITION_FADE_IN_START, OnFadeInStart);
            UIEventSystem.Instance.Subscribe(GameManager.SceneTransitionEvents.TRANSITION_LOADING_PROGRESS, OnLoadingProgress);
            UIEventSystem.Instance.Subscribe(GameManager.SceneTransitionEvents.TRANSITION_FADE_OUT_START, OnFadeOutStart);
            UIEventSystem.Instance.Subscribe(GameManager.SceneTransitionEvents.TRANSITION_ABORT, OnTransitionAbort);
        }

        private Canvas CreateTransitionUI()
        {
            // 创建Canvas
            GameObject canvasGO = new("TransitionCanvas");
            canvasGO.transform.SetParent(transform);

            Canvas canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 9999;

            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            canvasGO.AddComponent<GraphicRaycaster>();

            // 创建CanvasGroup
            fadeCanvasGroup = canvasGO.AddComponent<CanvasGroup>();
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;

            // 创建背景图片
            GameObject imageGO = new("FadeImage");
            imageGO.transform.SetParent(canvasGO.transform);

            fadeImage = imageGO.AddComponent<Image>();
            fadeImage.color = fadeColor;
            fadeImage.rectTransform.anchorMin = Vector2.zero;
            fadeImage.rectTransform.anchorMax = Vector2.one;
            fadeImage.rectTransform.offsetMin = Vector2.zero;
            fadeImage.rectTransform.offsetMax = Vector2.zero;

            return canvas;
        }

        #endregion

        #region 事件处理方法

        private void OnTransitionStart(object data)
        {
            if (data is TransitionData transitionData)
            {
                targetSceneName = transitionData.sceneName;
                loadingProgress = 0f;

                if (debugMode)
                {
                    Debug.Log($"收到场景转换开始事件: {targetSceneName}");
                }
            }
        }

        private void OnFadeInStart(object data)
        {
            if (isTransitioning)
            {
                return;
            }

            if (currentTransitionCoroutine != null)
            {
                StopCoroutine(currentTransitionCoroutine);
            }

            currentTransitionCoroutine = StartCoroutine(HandleFadeIn());
        }

        private void OnLoadingProgress(object data)
        {
            if (data is TransitionData transitionData)
            {
                loadingProgress = transitionData.progress;

                // 可以在这里更新加载进度条UI
                if (debugMode)
                {
                    Debug.Log($"加载进度: {loadingProgress:P0}");
                }
            }
        }

        private void OnFadeOutStart(object data)
        {
            if (currentTransitionCoroutine != null)
            {
                StopCoroutine(currentTransitionCoroutine);
            }

            currentTransitionCoroutine = StartCoroutine(HandleFadeOut());
        }

        private void OnTransitionAbort(object data)
        {
            AbortTransition();
        }

        #endregion

        /// <summary>
        /// 强制中断当前过渡
        /// </summary>
        public void AbortTransition()
        {
            if (currentTransitionCoroutine != null)
            {
                StopCoroutine(currentTransitionCoroutine);
                currentTransitionCoroutine = null;
            }

            isTransitioning = false;
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;

            if (debugMode)
            {
                Debug.Log("场景过渡已中断");
            }
        }

        #region 事件处理协程

        private IEnumerator HandleFadeIn()
        {
            isTransitioning = true;

            if (debugMode)
            {
                Debug.Log("开始淡入过渡效果");
            }

            yield return StartCoroutine(FadeIn());

            if (debugMode)
            {
                Debug.Log("淡入过渡完成");
            }

            currentTransitionCoroutine = null;
        }

        private IEnumerator HandleFadeOut()
        {
            if (debugMode)
            {
                Debug.Log("开始淡出过渡效果");
            }

            yield return StartCoroutine(FadeOut());

            isTransitioning = false;

            if (debugMode)
            {
                Debug.Log("淡出过渡完成");
            }

            currentTransitionCoroutine = null;
        }

        #endregion


        private IEnumerator FadeIn()
        {
            fadeCanvasGroup.blocksRaycasts = true;

            float elapsedTime = 0f;
            while (elapsedTime < fadeInDuration)
            {
                elapsedTime += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeInDuration);
                yield return null;
            }

            fadeCanvasGroup.alpha = 1f;
        }

        private IEnumerator FadeOut()
        {
            float elapsedTime = 0f;
            while (elapsedTime < fadeOutDuration)
            {
                elapsedTime += Time.deltaTime;
                fadeCanvasGroup.alpha = Mathf.Clamp01(1f - (elapsedTime / fadeOutDuration));
                yield return null;
            }

            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
        }

        #region 属性

        /// <summary>
        /// 是否正在执行场景过渡
        /// </summary>
        public bool IsTransitioning => isTransitioning;

        /// <summary>
        /// 淡入持续时间
        /// </summary>
        public float FadeInDuration
        {
            get => fadeInDuration;
            set => fadeInDuration = Mathf.Max(0.1f, value);
        }

        /// <summary>
        /// 淡出持续时间
        /// </summary>
        public float FadeOutDuration
        {
            get => fadeOutDuration;
            set => fadeOutDuration = Mathf.Max(0.1f, value);
        }

        /// <summary>
        /// 最短显示时间
        /// </summary>
        public float MinDisplayDuration
        {
            get => minDisplayDuration;
            set => minDisplayDuration = Mathf.Max(0f, value);
        }

        /// <summary>
        /// 过渡颜色
        /// </summary>
        public Color FadeColor
        {
            get => fadeColor;
            set
            {
                fadeColor = value;
                if (fadeImage != null)
                {
                    fadeImage.color = fadeColor;
                }
            }
        }

        #endregion

        #region 调试方法

        [ContextMenu("测试淡入淡出")]
        private void TestFade()
        {
            if (currentTransitionCoroutine != null)
            {
                StopCoroutine(currentTransitionCoroutine);
            }

            currentTransitionCoroutine = StartCoroutine(TestFadeCoroutine());
        }

        private IEnumerator TestFadeCoroutine()
        {
            Debug.Log("开始测试淡入淡出效果");

            yield return StartCoroutine(FadeIn());
            Debug.Log("淡入完成");

            yield return new WaitForSeconds(1f);

            yield return StartCoroutine(FadeOut());
            Debug.Log("淡出完成");

            currentTransitionCoroutine = null;
        }

        #endregion
    }
}
