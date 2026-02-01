using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 音频管理器 - 负责背景音乐和音效的播放与控制
/// </summary>
public class AudioManager : MonoBehaviour
{
    #region 单例模式实现

    private static AudioManager instance;

    /// <summary>
    /// 音频管理器单例实例
    /// </summary>
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();
                if (instance == null)
                {
                    GameObject audioManagerObject = new("AudioManager");
                    instance = audioManagerObject.AddComponent<AudioManager>();
                }
            }
            return instance;
        }
    }

    #endregion

    [Header("音频设置")]
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    [Header("音量控制")]
    [SerializeField] private float musicVolume = 0.7f;
    [SerializeField] private float sfxVolume = 0.8f;

    [Header("音频资源")]
    [SerializeField] private AudioClip mainMenuBGM;
    [SerializeField] private AudioClip gameBGM;
    [SerializeField] private AudioClip successBGM;
    [SerializeField] private AudioClip failBGM;

    [Header("调试设置")]
    [SerializeField] private bool debugMode = false;

    // 音效资源字典
    private Dictionary<string, AudioClip> sfxClips = new Dictionary<string, AudioClip>();

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

        InitializeAudioManager();
    }

    #region 初始化

    /// <summary>
    /// 初始化音频管理器
    /// </summary>
    private void InitializeAudioManager()
    {
        // 确保AudioSource组件存在
        if (musicAudioSource == null)
        {
            musicAudioSource = gameObject.AddComponent<AudioSource>();
        }

        if (sfxAudioSource == null)
        {
            sfxAudioSource = gameObject.AddComponent<AudioSource>();
        }

        // 配置音乐AudioSource
        musicAudioSource.loop = true;
        musicAudioSource.playOnAwake = false;
        musicAudioSource.volume = musicVolume;

        // 配置音效AudioSource
        sfxAudioSource.loop = false;
        sfxAudioSource.playOnAwake = false;
        sfxAudioSource.volume = sfxVolume;

        if (debugMode)
        {
            Debug.Log("AudioManager初始化完成");
        }

        var sfxClips = Resources.LoadAll<AudioClip>("Sound");
        foreach (var clip in sfxClips)
        {
            RegisterSFX(clip.name, clip);
        }
    }

    /// <summary>
    /// 注册音效资源
    /// </summary>
    public void RegisterSFX(string sfxName, AudioClip clip)
    {
        if (!string.IsNullOrEmpty(sfxName) && clip != null)
        {
            sfxClips[sfxName] = clip;
        }
    }

    #endregion

    #region 音乐控制

    /// <summary>
    /// 播放主菜单背景音乐
    /// </summary>
    public void PlayMainMenuMusic()
    {
        PlayMusic(mainMenuBGM);
    }

    /// <summary>
    /// 播放游戏背景音乐
    /// </summary>
    public void PlayGameMusic()
    {
        PlayMusic(gameBGM);
    }

    /// <summary>
    /// 播放成功背景音乐
    /// </summary>
    public void PlaySuccessMusic()
    {
        PlayMusic(successBGM);
    }

    /// <summary>
    /// 播放失败背景音乐
    /// </summary>
    public void PlayFailMusic()
    {
        PlayMusic(failBGM);
    }

    /// <summary>
    /// 播放指定的背景音乐
    /// </summary>
    public void PlayMusic(AudioClip clip)
    {
        if (clip == null)
        {
            if (debugMode)
            {
                Debug.LogWarning("尝试播放空的背景音乐");
            }
            return;
        }

        if (musicAudioSource.clip == clip && musicAudioSource.isPlaying)
        {
            return;
        }

        musicAudioSource.clip = clip;
        musicAudioSource.Play();

        if (debugMode)
        {
            Debug.Log($"播放背景音乐: {clip.name}");
        }
    }

    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public void PauseMusic()
    {
        if (musicAudioSource.isPlaying)
        {
            musicAudioSource.Pause();
        }
    }

    /// <summary>
    /// 恢复背景音乐
    /// </summary>
    public void ResumeMusic()
    {
        if (!musicAudioSource.isPlaying && musicAudioSource.clip != null)
        {
            musicAudioSource.UnPause();
        }
    }

    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopMusic()
    {
        musicAudioSource.Stop();
    }

    #endregion

    #region 音效控制

    /// <summary>
    /// 播放音效
    /// </summary>
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
        {
            if (debugMode)
            {
                Debug.LogWarning("尝试播放空的音效");
            }
            return;
        }

        sfxAudioSource.PlayOneShot(clip);

        if (debugMode)
        {
            Debug.Log($"播放音效: {clip.name}");
        }
    }

    /// <summary>
    /// 播放指定名称的音效
    /// </summary>
    public void PlaySFX(string sfxName)
    {
        if (sfxClips.TryGetValue(sfxName, out AudioClip clip))
        {
            PlaySFX(clip);
        }
        else
        {
            if (debugMode)
            {
                Debug.LogWarning($"未找到音效: {sfxName}");
            }
        }
    }

    /// <summary>
    /// 播放音效（可自定义音量）
    /// </summary>
    public void PlaySFX(AudioClip clip, float volumeScale)
    {
        if (clip == null)
        {
            if (debugMode)
            {
                Debug.LogWarning("尝试播放空的音效");
            }
            return;
        }

        sfxAudioSource.PlayOneShot(clip, volumeScale);

        if (debugMode)
        {
            Debug.Log($"播放音效: {clip.name} (音量: {volumeScale})");
        }
    }

    /// <summary>
    /// 播放音效（可自定义音量）
    /// </summary>
    public void PlaySFX(string sfxName, float volumeScale)
    {
        if (sfxClips.TryGetValue(sfxName, out AudioClip clip))
        {
            PlaySFX(clip, volumeScale);
        }
        else
        {
            if (debugMode)
            {
                Debug.LogWarning($"未找到音效: {sfxName}");
            }
        }
    }

    /// <summary>
    /// 停止当前音效
    /// </summary>
    public void StopSFX()
    {
        sfxAudioSource.Stop();
    }

    #endregion

    #region 音量控制

    /// <summary>
    /// 设置背景音乐音量
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        musicAudioSource.volume = musicVolume;
    }

    /// <summary>
    /// 设置音效音量
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        sfxAudioSource.volume = sfxVolume;
    }

    /// <summary>
    /// 设置整体音量
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        float clampedVolume = Mathf.Clamp01(volume);
        musicAudioSource.volume = clampedVolume * musicVolume;
        sfxAudioSource.volume = clampedVolume * sfxVolume;
    }

    #endregion

    #region 渐变效果

    /// <summary>
    /// 背景音乐淡入
    /// </summary>
    public IEnumerator FadeInMusic(float duration, float targetVolume = 1f)
    {
        float startVolume = 0f;
        musicAudioSource.volume = startVolume;
        musicAudioSource.Play();

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            musicAudioSource.volume = Mathf.Lerp(startVolume, targetVolume * musicVolume, elapsedTime / duration);
            yield return null;
        }

        musicAudioSource.volume = targetVolume * musicVolume;
    }

    /// <summary>
    /// 背景音乐淡出
    /// </summary>
    public IEnumerator FadeOutMusic(float duration)
    {
        float startVolume = musicAudioSource.volume;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            musicAudioSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / duration);
            yield return null;
        }

        musicAudioSource.Stop();
        musicAudioSource.volume = musicVolume;
    }

    /// <summary>
    /// 背景音乐交叉淡入淡出
    /// </summary>
    public IEnumerator CrossFadeMusic(AudioClip newClip, float fadeDuration)
    {
        if (newClip == null)
        {
            yield break;
        }

        float startVolume = musicAudioSource.volume;
        float elapsedTime = 0f;

        // 淡出当前音乐
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            musicAudioSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeDuration);
            yield return null;
        }

        musicAudioSource.Stop();
        musicAudioSource.clip = newClip;

        // 淡入新音乐
        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            musicAudioSource.volume = Mathf.Lerp(0f, musicVolume, elapsedTime / fadeDuration);
            yield return null;
        }

        musicAudioSource.volume = musicVolume;
        musicAudioSource.Play();
    }

    #endregion

    #region 公共属性

    /// <summary>
    /// 当前背景音乐音量
    /// </summary>
    public float MusicVolume => musicVolume;

    /// <summary>
    /// 当前音效音量
    /// </summary>
    public float SFXVolume => sfxVolume;

    /// <summary>
    /// 背景音乐是否正在播放
    /// </summary>
    public bool IsMusicPlaying => musicAudioSource.isPlaying;

    /// <summary>
    /// 当前播放的背景音乐
    /// </summary>
    public AudioClip CurrentMusicClip => musicAudioSource.clip;

    #endregion
}
