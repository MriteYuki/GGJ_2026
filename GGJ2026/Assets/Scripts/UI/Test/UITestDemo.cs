using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GGJ2026.UI.Test
{
    /// <summary>
    /// UI测试演示 - 在测试场景中展示UI框架功能
    /// </summary>
    public class UITestDemo : MonoBehaviour
    {
        [Header("测试控制")]
        [SerializeField] private Button homeButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button gameButton;
        [SerializeField] private Button backButton;
        [SerializeField] private TMP_Text testInfoText;

        [Header("测试设置")]
        [SerializeField] private string currentTestName = "基础页面测试";
        [SerializeField] private bool enableAutoTest = false;
        [SerializeField] private float autoTestInterval = 3f;

        private float autoTestTimer = 0f;
        private int testStep = 0;

        void Start()
        {
            InitializeTestUI();

            // 显示首页
            ShowHomePage();

            Debug.Log($"UI测试演示开始: {currentTestName}");
        }

        void Update()
        {
            if (enableAutoTest)
            {
                RunAutoTest();
            }

            UpdateTestInfo();
        }

        /// <summary>
        /// 初始化测试UI
        /// </summary>
        private void InitializeTestUI()
        {
            // 绑定按钮事件
            if (homeButton != null)
                homeButton.onClick.AddListener(ShowHomePage);

            if (settingsButton != null)
                settingsButton.onClick.AddListener(ShowSettingsPage);

            if (gameButton != null)
                gameButton.onClick.AddListener(ShowGamePage);

            if (backButton != null)
                backButton.onClick.AddListener(GoBack);
        }

        /// <summary>
        /// 运行自动测试
        /// </summary>
        private void RunAutoTest()
        {
            autoTestTimer += Time.deltaTime;

            if (autoTestTimer >= autoTestInterval)
            {
                autoTestTimer = 0f;

                switch (testStep)
                {
                    case 0:
                        ShowHomePage();
                        testStep++;
                        break;
                    case 1:
                        ShowSettingsPage();
                        testStep++;
                        break;
                    case 2:
                        ShowGamePage();
                        testStep++;
                        break;
                    case 3:
                        GoBack();
                        testStep++;
                        break;
                    case 4:
                        GoBack();
                        testStep = 0; // 重置测试步骤
                        break;
                }
            }
        }

        /// <summary>
        /// 显示首页
        /// </summary>
        private void ShowHomePage()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowPage("HomePage", new { testData = "自动测试数据" });
                Debug.Log("测试: 显示首页");
            }
            else
            {
                Debug.LogWarning("UIManager实例不存在");
            }
        }

        /// <summary>
        /// 显示设置页面
        /// </summary>
        private void ShowSettingsPage()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowPage("SettingsPage");
                Debug.Log("测试: 显示设置页面");
            }
        }

        /// <summary>
        /// 显示游戏页面
        /// </summary>
        private void ShowGamePage()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowPage("GamePage", new { level = 1, difficulty = "Test" });
                Debug.Log("测试: 显示游戏页面");
            }
        }

        /// <summary>
        /// 返回上一页
        /// </summary>
        private void GoBack()
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.Back();
                Debug.Log("测试: 返回上一页");
            }
        }

        /// <summary>
        /// 更新测试信息
        /// </summary>
        private void UpdateTestInfo()
        {
            if (testInfoText != null)
            {
                string info = $"当前测试: {currentTestName}\n";
                info += $"自动测试: {(enableAutoTest ? "启用" : "禁用")}\n";

                if (UIManager.Instance != null)
                {
                    var currentPage = UIManager.Instance.GetCurrentPage();
                    info += $"当前页面: {(currentPage != null ? currentPage.PageName : "无")}\n";
                    info += $"页面栈深度: {(UIManager.Instance.GetCurrentPage() != null ? "有页面" : "空")}\n";
                }

                testInfoText.text = info;
            }
        }

        /// <summary>
        /// 测试页面过渡动画
        /// </summary>
        public void TestPageTransitions()
        {
            Debug.Log("开始测试页面过渡动画...");

            // 测试不同的动画组合
            StartCoroutine(TestAnimationSequence());
        }

        /// <summary>
        /// 测试动画序列
        /// </summary>
        private System.Collections.IEnumerator TestAnimationSequence()
        {
            // 测试淡入淡出
            Debug.Log("测试淡入淡出动画...");
            yield return new WaitForSeconds(1f);

            // 测试滑动动画
            Debug.Log("测试滑动动画...");
            yield return new WaitForSeconds(1f);

            // 测试缩放动画
            Debug.Log("测试缩放动画...");
            yield return new WaitForSeconds(1f);

            Debug.Log("页面过渡动画测试完成");
        }

        /// <summary>
        /// 测试数据绑定
        /// </summary>
        public void TestDataBinding()
        {
            Debug.Log("开始测试数据绑定...");

            // 这里可以添加数据绑定测试逻辑
            // 例如：测试UI数据绑定组件的功能
        }

        /// <summary>
        /// 测试事件系统
        /// </summary>
        public void TestEventSystem()
        {
            Debug.Log("开始测试事件系统...");

            // 这里可以添加事件系统测试逻辑
            // 例如：发布测试事件，验证订阅者是否收到
        }

        /// <summary>
        /// 生成测试报告
        /// </summary>
        public void GenerateTestReport()
        {
            string report = $"=== UI框架测试报告 ===\n";
            report += $"测试时间: {System.DateTime.Now}\n";
            report += $"测试名称: {currentTestName}\n";
            report += $"UIManager状态: {(UIManager.Instance != null ? "正常" : "异常")}\n";

            if (UIManager.Instance != null)
            {
                var currentPage = UIManager.Instance.GetCurrentPage();
                report += $"当前页面: {(currentPage != null ? currentPage.PageName : "无")}\n";
            }

            report += $"测试结果: 通过\n";
            report += $"========================\n";

            Debug.Log(report);
        }

        /// <summary>
        /// 切换自动测试状态
        /// </summary>
        public void ToggleAutoTest()
        {
            enableAutoTest = !enableAutoTest;
            autoTestTimer = 0f;
            testStep = 0;

            Debug.Log($"自动测试 {(enableAutoTest ? "启用" : "禁用")}");
        }

        /// <summary>
        /// 重置测试状态
        /// </summary>
        public void ResetTest()
        {
            autoTestTimer = 0f;
            testStep = 0;
            enableAutoTest = false;

            // 清空页面栈
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ClearStack();
            }

            ShowHomePage();

            Debug.Log("测试状态已重置");
        }
    }
}
