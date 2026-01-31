using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GGJ2026.Gameplay
{

    [Serializable]
    public struct SceneTransition
    {
        public string sceneFrom;
        public string sceneTo;
    }

    /// <summary>
    /// 游戏管理器 - 负责场景切换和全局游戏状态管理
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region 单例模式实现

        private static GameManager instance;

        /// <summary>
        /// 游戏管理器单例实例
        /// </summary>
        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<GameManager>();
                    if (instance == null)
                    {
                        GameObject gameManagerObject = new("GameManager");
                        instance = gameManagerObject.AddComponent<GameManager>();
                    }
                }
                return instance;
            }
        }

        #endregion

        [Header("场景设置")]
        [SerializeField] private string startGameScene = "GameStart";

        [Header("场景流程配置")]
        public SceneFlowConfig sceneFlowConfig;

        [Header("场景切换设置")]
        [SerializeField] private float sceneTransitionDelay = 1.0f;
        [SerializeField] private bool enableLoadingScreen = true;

        [Header("调试设置")]
        [SerializeField] private bool debugMode = false;

        // 场景状态
        private string currentScene;
        private string previousScene;
        private bool isTransitioning = false;

        // 流程配置状态
        private SceneFlowChain currentFlowChain;

        // 场景加载进度事件
        public delegate void SceneLoadProgress(float progress);
        public event SceneLoadProgress OnSceneLoadProgress;

        public delegate void SceneTransitionComplete(string sceneName);
        public event SceneTransitionComplete OnSceneTransitionComplete;

        #region Unity生命周期

        void Awake()
        {
            // 单例模式实现
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }

            InitializeGameManager();
        }

        void Start()
        {
            currentScene = SceneManager.GetActiveScene().name;
            if (debugMode)
            {
                Debug.Log($"GameManager初始化完成，当前场景: {currentScene}");
            }

            LoadStartGame();
        }

        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        #endregion

        #region 公共方法 - 场景切换

        void Update()
        {
            //if (Input.GetKeyDown(KeyCode.N))
            //{
            //    LoadNextScene();
            //}
        }

        /// <summary>
        /// 获取当前场景的下一个场景
        /// </summary>
        public string GetNextScene(string currentScene)
        {
            return currentFlowChain?.GetNextScene(currentScene);
        }

        /// <summary>
        /// 检查是否为最终关卡
        /// </summary>
        public bool IsLastScene(string sceneName)
        {
            return currentFlowChain?.IsLastScene(sceneName) ?? false;
        }

        /// <summary>
        /// 获取场景在流程中的位置信息
        /// </summary>
        public string GetScenePositionInfo(string sceneName)
        {
            if (currentFlowChain == null) return "未知流程";

            int position = currentFlowChain.GetScenePosition(sceneName);
            return $"{currentFlowChain.chainName} 第{position}关/共{currentFlowChain.TotalScenes}关";
        }

        /// <summary>
        /// 加载指定名称的场景
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        public void LoadScene(string sceneName)
        {
            if (isTransitioning)
            {
                if (debugMode)
                {
                    Debug.LogWarning($"正在切换场景，忽略重复请求: {sceneName}");
                }
                return;
            }

            if (!SceneExists(sceneName))
            {
                Debug.LogError($"场景不存在: {sceneName}");
                return;
            }

            StartCoroutine(TransitionToScene(sceneName));
        }

        /// <summary>
        /// 重新加载当前场景
        /// </summary>
        public void ReloadCurrentScene()
        {
            LoadScene(currentScene);
        }

        /// <summary>
        /// 返回上一个场景
        /// </summary>
        public void LoadPreviousScene()
        {
            if (!string.IsNullOrEmpty(previousScene) && SceneExists(previousScene))
            {
                LoadScene(previousScene);
            }
            else
            {
                Debug.LogWarning("无法返回上一个场景，将加载主菜单");
                LoadMainMenu();
            }
        }

        public void LoadMainMenu()
        {
            LoadScene("GameStart");
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        public void QuitGame()
        {
            if (debugMode)
            {
                Debug.Log("退出游戏");
            }

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }

        #endregion

        #region 场景流程配置相关方法

        /// <summary>
        /// 开始游戏流程 - 随机选择首关并进入起始场景
        /// </summary>
        public void LoadStartGame()
        {
            if (sceneFlowConfig == null)
            {
                Debug.LogError("场景流程配置未设置！");
                return;
            }

            // 随机选择首关流程链
            currentFlowChain = sceneFlowConfig.GetRandomFirstLevelChain();
            if (currentFlowChain == null)
            {
                Debug.LogError("无法获取有效的流程链！");
                return;
            }

            if (debugMode)
            {
                Debug.Log($"随机选择流程链: {currentFlowChain.chainName}");
                Debug.Log($"首关场景: {currentFlowChain.FirstLevel}");
                Debug.Log($"最终关卡: {currentFlowChain.FinalLevel}");
            }

            if(currentScene != currentFlowChain.FirstLevel)
            {
                LoadScene(currentFlowChain.FirstLevel);
            }
        }

        /// <summary>
        /// 加载下一个场景（根据流程配置）
        /// </summary>
        public void LoadNextScene()
        {
            if (sceneFlowConfig == null)
            {
                Debug.LogError("场景流程配置未设置！");
                return;
            }

            string nextScene = GetNextScene(currentScene);
            if (!string.IsNullOrEmpty(nextScene))
            {
                LoadScene(nextScene);
            }
            else
            {
                // 流程结束，显示结局或回到主菜单
                if (IsFinalLevel)
                {
                    ShowGameEnding();
                }
                else
                {
                    LoadScene("GameStart");
                }
            }
        }

        /// <summary>
        /// 显示游戏结局（最终关卡完成）
        /// </summary>
        private void ShowGameEnding()
        {
            if (debugMode)
            {
                Debug.Log($"游戏流程完成！流程链: {currentFlowChain?.chainName}");
            }

            // 这里可以加载结局场景或显示结局UI
            // LoadScene("GameEnding");
        }

        #endregion

        #region 公共属性

        /// <summary>
        /// 当前场景名称
        /// </summary>
        public string CurrentScene => currentScene;

        /// <summary>
        /// 上一个场景名称
        /// </summary>
        public string PreviousScene => previousScene;

        /// <summary>
        /// 是否正在切换场景
        /// </summary>
        public bool IsTransitioning => isTransitioning;

        /// <summary>
        /// 下一个场景名称（基于流程配置）
        /// </summary>
        public string NextScene => GetNextScene(currentScene);

        /// <summary>
        /// 是否有下一个场景
        /// </summary>
        public bool HasNextScene => !string.IsNullOrEmpty(NextScene);

        /// <summary>
        /// 是否是最终关卡
        /// </summary>
        public bool IsFinalLevel => IsLastScene(currentScene);

        /// <summary>
        /// 当前流程链名称
        /// </summary>
        public string CurrentChainName => currentFlowChain?.chainName;

        /// <summary>
        /// 当前场景在流程中的位置信息
        /// </summary>
        public string CurrentScenePositionInfo => GetScenePositionInfo(currentScene) ?? "未知位置";

        /// <summary>
        /// 当前流程链的总关卡数
        /// </summary>
        public int CurrentChainTotalLevels => currentFlowChain?.TotalScenes ?? 0;

        /// <summary>
        /// 当前场景在流程链中的位置（从1开始）
        /// </summary>
        public int CurrentScenePosition => currentFlowChain?.GetScenePosition(currentScene) ?? -1;

        #endregion

        #region 私有实现方法

        private void InitializeGameManager()
        {
            // 确保GameManager在场景切换时不被销毁
            DontDestroyOnLoad(gameObject);

            // 初始化场景状态
            currentScene = SceneManager.GetActiveScene().name;
            previousScene = "";
            isTransitioning = false;
        }

        private IEnumerator TransitionToScene(string sceneName)
        {
            isTransitioning = true;

            if (debugMode)
            {
                Debug.Log($"开始切换场景: {currentScene} -> {sceneName}");
            }

            // 触发场景切换开始事件
            OnSceneTransitionComplete?.Invoke("start");

            // 等待场景切换延迟
            yield return new WaitForSeconds(sceneTransitionDelay);

            // 异步加载场景
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            // 禁用自动激活场景
            asyncLoad.allowSceneActivation = false;

            // 监听加载进度
            while (!asyncLoad.isDone)
            {
                float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                OnSceneLoadProgress?.Invoke(progress);

                if (asyncLoad.progress >= 0.9f)
                {
                    // 加载完成，等待激活
                    break;
                }

                yield return null;
            }

            // 等待额外延迟后激活场景
            yield return new WaitForSeconds(sceneTransitionDelay);

            // 激活新场景
            asyncLoad.allowSceneActivation = true;

            // 等待场景激活完成
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            isTransitioning = false;

            if (debugMode)
            {
                Debug.Log($"场景切换完成: {sceneName}");
            }

            // 触发场景切换完成事件
            OnSceneTransitionComplete?.Invoke(sceneName);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // 更新场景记录
            previousScene = currentScene;
            currentScene = scene.name;

            if (debugMode)
            {
                Debug.Log($"场景加载完成: {currentScene} (上一个场景: {previousScene})");
                if (currentFlowChain != null)
                {
                    Debug.Log($"当前流程链: {currentFlowChain.chainName}");
                    Debug.Log($"场景位置: {CurrentScenePositionInfo}");
                    Debug.Log($"下一个场景: {NextScene ?? "无"}");
                }
            }
        }

        private bool SceneExists(string sceneName)
        {
            // 检查场景是否存在于构建设置中
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneNameInBuild = System.IO.Path.GetFileNameWithoutExtension(scenePath);

                if (sceneNameInBuild == sceneName)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region 工具方法

        /// <summary>
        /// 检查是否为主菜单场景
        /// </summary>
        public bool IsMainMenuScene()
        {
            return currentScene == "";
        }

        /// <summary>
        /// 检查是否为关卡场景
        /// </summary>
        public bool IsLevelScene()
        {
            return currentScene.StartsWith("Level");
        }

        /// <summary>
        /// 获取当前关卡编号（如果不是关卡场景返回-1）
        /// </summary>
        public int GetCurrentLevelNumber()
        {
            if (!IsLevelScene()) return -1;

            string levelNumberStr = currentScene.Replace("Level", "");
            if (int.TryParse(levelNumberStr, out int levelNumber))
            {
                return levelNumber;
            }

            return -1;
        }

        #endregion
    }
}
