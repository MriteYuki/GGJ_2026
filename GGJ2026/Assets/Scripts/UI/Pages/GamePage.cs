using UnityEngine;
using UnityEngine.UI;

namespace GGJ2026.UI
{
    /// <summary>
    /// 游戏页面 - 游戏进行界面
    /// </summary>
    public class GamePage : BasePage
    {
        [Header("游戏组件")]
        [SerializeField] private Text scoreText;
        [SerializeField] private Text levelText;
        [SerializeField] private Text timeText;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Slider healthBar;
        
        [Header("游戏设置")]
        [SerializeField] private int initialScore = 0;
        [SerializeField] private int initialLevel = 1;
        [SerializeField] private float gameTime = 60f;
        [SerializeField] private float maxHealth = 100f;
        
        private int currentScore;
        private int currentLevel;
        private float currentTime;
        private float currentHealth;
        private bool isPaused = false;
        
        /// <summary>
        /// 初始化
        /// </summary>
        protected override void InitializeComponents()
        {
            base.InitializeComponents();
            
            // 设置页面名称
            pageName = "Game";
            
            // 绑定按钮事件
            if (pauseButton != null)
                pauseButton.onClick.AddListener(OnPauseButtonClick);
                
            if (backButton != null)
                backButton.onClick.AddListener(OnBackButtonClick);
                
            // 初始化游戏状态
            ResetGameState();
        }
        
        /// <summary>
        /// 页面显示时调用
        /// </summary>
        public override void OnShow(object data = null)
        {
            base.OnShow(data);
            
            // 游戏页面可以返回
            canGoBack = true;
            
            // 处理传递的数据
            if (data != null)
            {
                // 可以解析传递的游戏数据
                // 例如：关卡信息、难度设置等
                Debug.Log($"游戏页面接收数据: {data}");
            }
            
            // 重置游戏状态
            ResetGameState();
            
            // 确保时间缩放正常
            Time.timeScale = 1f;
            
            Debug.Log("游戏页面显示");
        }
        
        /// <summary>
        /// 重置游戏状态
        /// </summary>
        private void ResetGameState()
        {
            currentScore = initialScore;
            currentLevel = initialLevel;
            currentTime = gameTime;
            currentHealth = maxHealth;
            isPaused = false;
            
            UpdateUI();
        }
        
        /// <summary>
        /// 页面更新
        /// </summary>
        public override void OnUpdate()
        {
            if (!isPaused)
            {
                // 游戏逻辑更新
                UpdateGameTime();
                UpdateUI();
            }
        }
        
        /// <summary>
        /// 更新游戏时间
        /// </summary>
        private void UpdateGameTime()
        {
            currentTime -= Time.deltaTime;
            
            if (currentTime <= 0)
            {
                currentTime = 0;
                // 游戏结束逻辑
                OnGameOver();
            }
        }
        
        /// <summary>
        /// 更新UI
        /// </summary>
        private void UpdateUI()
        {
            if (scoreText != null)
                scoreText.text = $"Score: {currentScore}";
                
            if (levelText != null)
                levelText.text = $"Level: {currentLevel}";
                
            if (timeText != null)
                timeText.text = $"Time: {Mathf.CeilToInt(currentTime)}";
                
            if (healthBar != null)
                healthBar.value = currentHealth / maxHealth;
                
            if (pauseButton != null)
            {
                Text buttonText = pauseButton.GetComponentInChildren<Text>();
                if (buttonText != null)
                    buttonText.text = isPaused ? "Resume" : "Pause";
            }
        }
        
        /// <summary>
        /// 增加分数
        /// </summary>
        public void AddScore(int points)
        {
            currentScore += points;
            UpdateUI();
        }
        
        /// <summary>
        /// 升级
        /// </summary>
        public void LevelUp()
        {
            currentLevel++;
            UpdateUI();
        }
        
        /// <summary>
        /// 受到伤害
        /// </summary>
        public void TakeDamage(float damage)
        {
            currentHealth = Mathf.Max(0, currentHealth - damage);
            
            if (currentHealth <= 0)
            {
                OnGameOver();
            }
            
            UpdateUI();
        }
        
        /// <summary>
        /// 暂停按钮点击
        /// </summary>
        private void OnPauseButtonClick()
        {
            if (pauseButton == null) return;
            
            isPaused = !isPaused;
            
            if (isPaused)
            {
                // 暂停游戏逻辑（使用局部时间缩放而不是全局时间缩放）
                // Time.timeScale = 0f; // 避免影响其他游戏系统
                Debug.Log("游戏暂停");
            }
            else
            {
                // 恢复游戏逻辑
                // Time.timeScale = 1f; // 避免影响其他游戏系统
                Debug.Log("游戏继续");
            }
            
            UpdateUI();
        }
        
        /// <summary>
        /// 返回按钮点击
        /// </summary>
        private void OnBackButtonClick()
        {
            if (backButton == null) return;
            
            // 确保时间缩放正常
            Time.timeScale = 1f;
            
            GoBack();
        }
        
        /// <summary>
        /// 游戏结束
        /// </summary>
        private void OnGameOver()
        {
            isPaused = true;
            
            // 显示游戏结束界面
            Debug.Log($"游戏结束! 最终得分: {currentScore}");
            
            // 可以在这里显示游戏结束页面或返回主菜单
            // ShowPage("GameOverPage", new { score = currentScore, level = currentLevel });
        }
        
        /// <summary>
        /// 页面隐藏时调用
        /// </summary>
        public override void OnHide()
        {
            // 确保时间缩放恢复正常
            Time.timeScale = 1f;
            
            base.OnHide();
            Debug.Log("游戏页面隐藏");
        }
        
        /// <summary>
        /// 销毁时清理
        /// </summary>
        protected void OnDestroy()
        {
            // 清理事件绑定
            if (pauseButton != null)
                pauseButton.onClick.RemoveAllListeners();
                
            if (backButton != null)
                backButton.onClick.RemoveAllListeners();
        }
    }
}