using UnityEngine;
using UnityEngine.UI;

namespace GGJ2026.UI
{
    /// <summary>
    /// 设置页面 - 游戏设置界面
    /// </summary>
    public class SettingsPage : BasePage
    {
        [Header("设置组件")]
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider soundVolumeSlider;
        [SerializeField] private Toggle fullscreenToggle;
        [SerializeField] private Dropdown resolutionDropdown;
        [SerializeField] private Dropdown qualityDropdown;
        [SerializeField] private Button backButton;
        [SerializeField] private Button applyButton;
        [SerializeField] private Button resetButton;

        [Header("设置默认值")]
        [SerializeField] private float defaultMusicVolume = 0.8f;
        [SerializeField] private float defaultSoundVolume = 0.8f;
        [SerializeField] private bool defaultFullscreen = true;

        private bool settingsChanged = false;

        /// <summary>
        /// 初始化
        /// </summary>
        protected override void InitializeComponents()
        {
            base.InitializeComponents();

            // 设置页面名称
            pageName = "Settings";

            // 绑定按钮事件
            if (backButton != null)
                backButton.onClick.AddListener(OnBackButtonClick);

            if (applyButton != null)
                applyButton.onClick.AddListener(OnApplyButtonClick);

            if (resetButton != null)
                resetButton.onClick.AddListener(OnResetButtonClick);

            // 绑定设置变化事件
            if (musicVolumeSlider != null)
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

            if (soundVolumeSlider != null)
                soundVolumeSlider.onValueChanged.AddListener(OnSoundVolumeChanged);

            if (fullscreenToggle != null)
                fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);

            if (resolutionDropdown != null)
                resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);

            if (qualityDropdown != null)
                qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
        }

        /// <summary>
        /// 页面显示时调用
        /// </summary>
        public override void OnShow(object data = null)
        {
            base.OnShow(data);

            // 设置页面可以返回
            canGoBack = true;

            // 加载设置
            LoadSettings();

            settingsChanged = false;
            UpdateApplyButtonState();

            Debug.Log($"设置页面显示，传递数据: {data}");
        }

        /// <summary>
        /// 加载设置
        /// </summary>
        private void LoadSettings()
        {
            // 从PlayerPrefs加载设置或使用默认值
            float musicVolume = PlayerPrefs.GetFloat("MusicVolume", defaultMusicVolume);
            float soundVolume = PlayerPrefs.GetFloat("SoundVolume", defaultSoundVolume);
            bool fullscreen = PlayerPrefs.GetInt("Fullscreen", defaultFullscreen ? 1 : 0) == 1;

            if (musicVolumeSlider != null)
                musicVolumeSlider.value = musicVolume;

            if (soundVolumeSlider != null)
                soundVolumeSlider.value = soundVolume;

            if (fullscreenToggle != null)
                fullscreenToggle.isOn = fullscreen;

            // 初始化分辨率下拉框
            InitializeResolutionDropdown();

            // 初始化画质下拉框
            InitializeQualityDropdown();
        }

        /// <summary>
        /// 初始化分辨率下拉框
        /// </summary>
        private void InitializeResolutionDropdown()
        {
            if (resolutionDropdown == null) return;

            resolutionDropdown.ClearOptions();

            // 获取所有支持的分辨率
            Resolution[] resolutions = Screen.resolutions;
            int currentResolutionIndex = 0;

            for (int i = 0; i < resolutions.Length; i++)
            {
                resolutionDropdown.options.Add(new Dropdown.OptionData($"{resolutions[i].width} x {resolutions[i].height}"));

                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }

            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        /// <summary>
        /// 初始化画质下拉框
        /// </summary>
        private void InitializeQualityDropdown()
        {
            if (qualityDropdown == null) return;

            qualityDropdown.ClearOptions();

            string[] qualityNames = QualitySettings.names;
            foreach (string qualityName in qualityNames)
            {
                qualityDropdown.options.Add(new Dropdown.OptionData(qualityName));
            }

            qualityDropdown.value = QualitySettings.GetQualityLevel();
            qualityDropdown.RefreshShownValue();
        }

        /// <summary>
        /// 音乐音量变化
        /// </summary>
        private void OnMusicVolumeChanged(float volume)
        {
            settingsChanged = true;
            UpdateApplyButtonState();

            // 实时应用音乐音量变化
            // AudioManager.Instance?.SetMusicVolume(volume);
        }

        /// <summary>
        /// 音效音量变化
        /// </summary>
        private void OnSoundVolumeChanged(float volume)
        {
            settingsChanged = true;
            UpdateApplyButtonState();

            // 实时应用音效音量变化
            // AudioManager.Instance?.SetSoundVolume(volume);
        }

        /// <summary>
        /// 全屏设置变化
        /// </summary>
        private void OnFullscreenChanged(bool isFullscreen)
        {
            settingsChanged = true;
            UpdateApplyButtonState();
        }

        /// <summary>
        /// 分辨率变化
        /// </summary>
        private void OnResolutionChanged(int index)
        {
            settingsChanged = true;
            UpdateApplyButtonState();
        }

        /// <summary>
        /// 画质变化
        /// </summary>
        private void OnQualityChanged(int index)
        {
            settingsChanged = true;
            UpdateApplyButtonState();
        }

        /// <summary>
        /// 返回按钮点击
        /// </summary>
        private void OnBackButtonClick()
        {
            if (settingsChanged)
            {
                // 提示用户保存设置
                Debug.Log("设置已更改，是否保存？");
                // 这里可以添加确认对话框
            }

            GoBack();
        }

        /// <summary>
        /// 应用按钮点击
        /// </summary>
        private void OnApplyButtonClick()
        {
            ApplySettings();
            settingsChanged = false;
            UpdateApplyButtonState();

            Debug.Log("设置已应用");
        }

        /// <summary>
        /// 重置按钮点击
        /// </summary>
        private void OnResetButtonClick()
        {
            // 重置为默认设置
            if (musicVolumeSlider != null)
                musicVolumeSlider.value = defaultMusicVolume;

            if (soundVolumeSlider != null)
                soundVolumeSlider.value = defaultSoundVolume;

            if (fullscreenToggle != null)
                fullscreenToggle.isOn = defaultFullscreen;

            settingsChanged = true;
            UpdateApplyButtonState();

            Debug.Log("设置已重置为默认值");
        }

        /// <summary>
        /// 应用设置
        /// </summary>
        private void ApplySettings()
        {
            // 保存设置到PlayerPrefs
            if (musicVolumeSlider != null)
                PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);

            if (soundVolumeSlider != null)
                PlayerPrefs.SetFloat("SoundVolume", soundVolumeSlider.value);

            if (fullscreenToggle != null)
                PlayerPrefs.SetInt("Fullscreen", fullscreenToggle.isOn ? 1 : 0);

            // 应用分辨率设置
            if (resolutionDropdown != null)
            {
                Resolution selectedResolution = Screen.resolutions[resolutionDropdown.value];
                Screen.SetResolution(selectedResolution.width, selectedResolution.height, fullscreenToggle?.isOn ?? true);
            }

            // 应用画质设置
            if (qualityDropdown != null)
            {
                QualitySettings.SetQualityLevel(qualityDropdown.value);
            }

            PlayerPrefs.Save();
        }

        /// <summary>
        /// 更新应用按钮状态
        /// </summary>
        private void UpdateApplyButtonState()
        {
            if (applyButton != null)
            {
                applyButton.interactable = settingsChanged;
            }
        }

        /// <summary>
        /// 页面隐藏时调用
        /// </summary>
        public override void OnHide()
        {
            base.OnHide();
            Debug.Log("设置页面隐藏");
        }

        /// <summary>
        /// 销毁时清理
        /// </summary>
        protected void OnDestroy()
        {
            // 清理事件绑定
            if (musicVolumeSlider != null)
                musicVolumeSlider.onValueChanged.RemoveAllListeners();

            if (soundVolumeSlider != null)
                soundVolumeSlider.onValueChanged.RemoveAllListeners();

            if (fullscreenToggle != null)
                fullscreenToggle.onValueChanged.RemoveAllListeners();

            if (backButton != null)
                backButton.onClick.RemoveAllListeners();

            if (applyButton != null)
                applyButton.onClick.RemoveAllListeners();

            if (resetButton != null)
                resetButton.onClick.RemoveAllListeners();
        }
    }
}
