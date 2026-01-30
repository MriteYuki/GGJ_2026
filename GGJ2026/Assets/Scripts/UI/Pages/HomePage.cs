using UnityEngine;
using UnityEngine.UI;

namespace GGJ2026.UI
{
    /// <summary>
    /// 首页 - 游戏主界面
    /// </summary>
    public class HomePage : BasePage
    {
        [Header("首页组件")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private Text titleText;
        [SerializeField] private Text versionText;

        [Header("首页设置")]
        [SerializeField] private string gameTitle = "GGJ 2026";
        [SerializeField] private string gameVersion = "1.0.0";

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void InitializeComponents()
        {
            base.InitializeComponents();

            // 设置页面名称
            pageName = "Home";

            // 初始化UI元素
            if (titleText != null)
                titleText.text = gameTitle;

            if (versionText != null)
                versionText.text = $"Version: {gameVersion}";

            // 绑定按钮事件
            if (startButton != null)
                startButton.onClick.AddListener(OnStartButtonClick);

            if (settingsButton != null)
                settingsButton.onClick.AddListener(OnSettingsButtonClick);

            if (quitButton != null)
                quitButton.onClick.AddListener(OnQuitButtonClick);
        }

        /// <summary>
        /// 页面显示时调用
        /// </summary>
        public override void OnShow(object data = null)
        {
            base.OnShow(data);

            // 首页不能返回
            canGoBack = false;

            Debug.Log($"首页显示，传递数据: {data}");
        }

        /// <summary>
        /// 开始游戏按钮点击
        /// </summary>
        private void OnStartButtonClick()
        {
            Debug.Log("开始游戏");

            // 可以在这里添加开始游戏的逻辑
            // 例如：加载游戏场景、显示游戏页面等
            ShowPage("GamePage", new { level = 1, difficulty = "Normal" });
        }

        /// <summary>
        /// 设置按钮点击
        /// </summary>
        private void OnSettingsButtonClick()
        {
            Debug.Log("打开设置");
            ShowPage("SettingsPage");
        }

        /// <summary>
        /// 退出按钮点击
        /// </summary>
        private void OnQuitButtonClick()
        {
            Debug.Log("退出游戏");

            // 退出游戏逻辑
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }

        /// <summary>
        /// 页面更新
        /// </summary>
        public override void OnUpdate()
        {
            // 首页特定的更新逻辑
            // 例如：检查网络状态、更新时间显示等
        }

        /// <summary>
        /// 页面隐藏时调用
        /// </summary>
        public override void OnHide()
        {
            base.OnHide();
            Debug.Log("首页隐藏");
        }

        /// <summary>
        /// 销毁时清理
        /// </summary>
        protected void OnDestroy()
        {
            // 清理按钮事件
            if (startButton != null)
                startButton.onClick.RemoveAllListeners();

            if (settingsButton != null)
                settingsButton.onClick.RemoveAllListeners();

            if (quitButton != null)
                quitButton.onClick.RemoveAllListeners();
        }
    }
}
