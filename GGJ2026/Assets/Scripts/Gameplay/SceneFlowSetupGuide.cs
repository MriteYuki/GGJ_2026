using UnityEngine;

namespace GGJ2026.Gameplay
{
    /// <summary>
    /// 场景流程配置系统使用指南
    /// </summary>
    public class SceneFlowSetupGuide : MonoBehaviour
    {
        [Header("配置步骤")]
        [TextArea(3, 10)]
        public string setupInstructions = 
            "=== 场景流程配置系统使用指南 ===\n" +
            "\n" +
            "步骤1: 创建场景流程配置资产\n" +
            "1. 在Project窗口右键 → Create → GGJ2026 → 场景流程配置\n" +
            "2. 命名为 'SceneFlowConfig'\n" +
            "\n" +
            "步骤2: 配置场景流程\n" +
            "1. 设置起始场景名称 (如: 'GameStart')\n" +
            "2. 添加流程链，每个流程链代表一种游戏路线\n" +
            "3. 在每个流程链中按顺序添加场景名称\n" +
            "\n" +
            "步骤3: 配置GameManager\n" +
            "1. 将SceneFlowConfig资产拖拽到GameManager的Scene Flow Config字段\n" +
            "\n" +
            "步骤4: 使用示例\n" +
            "- GameManager.Instance.LoadStartGame() - 开始游戏流程\n" +
            "- GameManager.Instance.LoadNextScene() - 进入下一关\n" +
            "- GameManager.Instance.HasNextScene - 检查是否有下一关\n" +
            "- GameManager.Instance.IsFinalLevel - 检查是否为最终关卡\n";

        [Header("配置示例")]
        [TextArea(5, 15)]
        public string configExample = 
            "示例配置:\n" +
            "\n" +
            "起始场景: GameStart\n" +
            "\n" +
            "流程链1: 森林路线\n" +
            "- Level1_Forest\n" +
            "- Level2_Cave\n" +
            "- Level3_TreeTop\n" +
            "\n" +
            "流程链2: 沙漠路线\n" +
            "- Level1_Desert\n" +
            "- Level2_Oasis\n" +
            "- Level3_Pyramid\n" +
            "\n" +
            "流程链3: 海洋路线\n" +
            "- Level1_Beach\n" +
            "- Level2_Underwater\n" +
            "- Level3_Shipwreck\n" +
            "\n" +
            "工作流程:\n" +
            "1. 玩家在GameStart场景点击开始\n" +
            "2. 随机选择一条路线（如森林路线）\n" +
            "3. 按顺序加载: Level1_Forest → Level2_Cave → Level3_TreeTop\n" +
            "4. 到达最终关卡后显示结局\n";

        [Header("代码使用示例")]
        [TextArea(5, 10)]
        public string codeExample = 
            "// 在GameStart场景中\n" +
            "public class GameStartController : MonoBehaviour\n" +
            "{\n" +
            "    public void OnStartButtonClick()\n" +
            "    {\n" +
            "        // 开始游戏，随机选择首关流程链\n" +
            "        GameManager.Instance.LoadStartGame();\n" +
            "    }\n" +
            "}\n" +
            "\n" +
            "// 在关卡场景中\n" +
            "public class LevelController : MonoBehaviour\n" +
            "{\n" +
            "    void Update()\n" +
            "    {\n" +
            "        if (Input.GetKeyDown(KeyCode.N))\n" +
            "        {\n" +
            "            // 按N键进入下一关\n" +
            "            if (GameManager.Instance.HasNextScene)\n" +
            "            {\n" +
            "                GameManager.Instance.LoadNextScene();\n" +
            "            }\n" +
            "            else if (GameManager.Instance.IsFinalLevel)\n" +
            "            {\n" +
            "                // 最终关卡的特殊处理\n" +
            "                ShowEnding();\n" +
            "            }\n" +
            "        }\n" +
            "    }\n" +
            "}\n";

        [Header("调试功能")]
        [TextArea(3, 5)]
        public string debugInfo = 
            "调试快捷键:\n" +
            "- F1: 刷新UI信息\n" +
            "- F2: 在控制台显示流程信息\n" +
            "- F3: 强制切换到随机流程链\n" +
            "\n" +
            "在Inspector中右键SceneFlowConfig资产可以快速验证配置";

        void Start()
        {
            Debug.Log("=== 场景流程配置系统已就绪 ===");
            Debug.Log("请按照上方指南进行配置");
        }

        [ContextMenu("创建示例配置资产")]
        private void CreateExampleConfig()
        {
#if UNITY_EDITOR
            // 这里可以添加自动创建示例配置的代码
            Debug.Log("请在Project窗口右键 → Create → GGJ2026 → 场景流程配置");
#endif
        }

        [ContextMenu("验证当前配置")]
        private void ValidateCurrentConfig()
        {
            var gm = GameManager.Instance;
            if (gm != null)
            {
                Debug.Log("GameManager配置检查:");
                Debug.Log($"场景流程配置: {(gm.GetComponent<GameManager>().sceneFlowConfig != null ? "已设置" : "未设置")}");
            }
        }
    }
}