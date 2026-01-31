using UnityEngine;
using UnityEngine.UI;

namespace GGJ2026.Gameplay
{
    /// <summary>
    /// 场景流程使用示例 - 演示如何在游戏中使用场景流程配置系统
    /// </summary>
    public class SceneFlowExample : MonoBehaviour
    {
        [Header("UI引用")]
        [SerializeField] private Text sceneInfoText;
        [SerializeField] private Text nextSceneText;
        [SerializeField] private Button nextSceneButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button mainMenuButton;

        void Start()
        {
            // 初始化UI事件
            if (nextSceneButton != null)
            {
                nextSceneButton.onClick.AddListener(OnNextSceneClick);
            }

            if (restartButton != null)
            {
                restartButton.onClick.AddListener(OnRestartClick);
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.onClick.AddListener(OnMainMenuClick);
            }

            // 更新UI显示
            UpdateUI();
        }

        void Update()
        {
            // 实时更新UI（可选）
            if (Input.GetKeyDown(KeyCode.F1))
            {
                UpdateUI();
            }
        }

        /// <summary>
        /// 更新UI显示
        /// </summary>
        private void UpdateUI()
        {
            if (sceneInfoText != null)
            {
                sceneInfoText.text = GetSceneInfoText();
            }

            if (nextSceneText != null)
            {
                nextSceneText.text = GetNextSceneInfoText();
            }

            // 更新按钮状态
            if (nextSceneButton != null)
            {
                nextSceneButton.interactable = GameManager.Instance.HasNextScene && !GameManager.Instance.IsTransitioning;
                nextSceneButton.GetComponentInChildren<Text>().text = GameManager.Instance.IsFinalLevel ? "通关游戏" : "下一关";
            }

            if (restartButton != null)
            {
                restartButton.interactable = !GameManager.Instance.IsTransitioning;
            }

            if (mainMenuButton != null)
            {
                mainMenuButton.interactable = !GameManager.Instance.IsTransitioning;
            }
        }

        /// <summary>
        /// 获取当前场景信息文本
        /// </summary>
        private string GetSceneInfoText()
        {
            var gm = GameManager.Instance;
            
            string info = $"当前场景: {gm.CurrentScene}\n";
            info += $"场景位置: {gm.CurrentScenePositionInfo}\n";
            info += $"流程链: {gm.CurrentChainName ?? "无"}\n";
            
            if (gm.IsFinalLevel)
            {
                info += "<color=red>这是最终关卡！</color>\n";
            }
            
            return info;
        }

        /// <summary>
        /// 获取下一个场景信息文本
        /// </summary>
        private string GetNextSceneInfoText()
        {
            var gm = GameManager.Instance;
            
            if (gm.HasNextScene)
            {
                return $"下一个场景: {gm.NextScene}";
            }
            else if (gm.IsFinalLevel)
            {
                return "<color=yellow>恭喜！这是最后一关</color>";
            }
            else
            {
                return "没有下一个场景";
            }
        }

        /// <summary>
        /// 点击下一关按钮
        /// </summary>
        private void OnNextSceneClick()
        {
            if (GameManager.Instance.HasNextScene)
            {
                GameManager.Instance.LoadNextScene();
            }
        }

        /// <summary>
        /// 点击重新开始按钮
        /// </summary>
        private void OnRestartClick()
        {
            GameManager.Instance.ReloadCurrentScene();
        }

        /// <summary>
        /// 点击主菜单按钮
        /// </summary>
        private void OnMainMenuClick()
        {
            GameManager.Instance.LoadMainMenu();
        }

        /// <summary>
        /// 调试信息 - 在控制台显示详细流程信息
        /// </summary>
        [ContextMenu("显示流程信息")]
        private void ShowFlowInfo()
        {
            var gm = GameManager.Instance;
            
            Debug.Log($"=== 场景流程信息 ===");
            Debug.Log($"当前场景: {gm.CurrentScene}");
            Debug.Log($"场景位置: {gm.CurrentScenePositionInfo}");
            Debug.Log($"流程链: {gm.CurrentChainName}");
            Debug.Log($"下一个场景: {gm.NextScene ?? "无"}");
            Debug.Log($"是否为最终关卡: {gm.IsFinalLevel}");
            Debug.Log($"总关卡数: {gm.CurrentChainTotalLevels}");
            Debug.Log($"当前关卡位置: {gm.CurrentScenePosition}");
        }

        /// <summary>
        /// 强制切换到指定流程链（调试用）
        /// </summary>
        [ContextMenu("切换到随机流程链")]
        private void SwitchToRandomChain()
        {
            var gm = GameManager.Instance;
            if (gm != null)
            {
                gm.LoadStartGame();
            }
        }
    }
}