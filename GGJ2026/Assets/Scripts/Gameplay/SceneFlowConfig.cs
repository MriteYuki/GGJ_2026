using UnityEngine;
using System;
using System.Collections.Generic;

namespace GGJ2026.Gameplay
{
    /// <summary>
    /// 场景流程链 - 定义一组有序的场景序列
    /// </summary>
    [Serializable]
    public class SceneFlowChain
    {
        [Header("流程链信息")]
        [Tooltip("流程链名称，用于标识")]
        public string chainName = "新流程链";

        [Header("场景顺序")]
        [Tooltip("场景名称列表，按游戏流程顺序排列")]
        public List<string> sceneSequence = new List<string>();

        /// <summary>
        /// 获取流程链的第一个场景
        /// </summary>
        public string FirstLevel => sceneSequence.Count > 0 ? sceneSequence[0] : null;

        /// <summary>
        /// 获取流程链的最后一个场景（最终关卡）
        /// </summary>
        public string FinalLevel => sceneSequence.Count > 0 ? sceneSequence[sceneSequence.Count - 1] : null;

        /// <summary>
        /// 检查流程链是否包含指定场景
        /// </summary>
        public bool ContainsScene(string sceneName) => sceneSequence.Contains(sceneName);

        /// <summary>
        /// 获取当前场景的下一个场景
        /// </summary>
        public string GetNextScene(string currentScene)
        {
            int index = sceneSequence.IndexOf(currentScene);
            return (index >= 0 && index < sceneSequence.Count - 1) ? sceneSequence[index + 1] : null;
        }

        /// <summary>
        /// 检查是否为流程链的最后一个场景
        /// </summary>
        public bool IsLastScene(string sceneName)
        {
            return !string.IsNullOrEmpty(sceneName) && sceneSequence.Count > 0 && sceneSequence[sceneSequence.Count - 1] == sceneName;
        }

        /// <summary>
        /// 获取场景在流程链中的位置（从1开始）
        /// </summary>
        public int GetScenePosition(string sceneName)
        {
            int index = sceneSequence.IndexOf(sceneName);
            return index >= 0 ? index + 1 : -1;
        }

        /// <summary>
        /// 获取流程链的总场景数
        /// </summary>
        public int TotalScenes => sceneSequence.Count;
    }

    /// <summary>
    /// 场景流程配置 - 管理游戏的所有场景流程
    /// </summary>
    [CreateAssetMenu(fileName = "NewSceneFlowConfig", menuName = "GGJ2026/场景流程配置")]
    public class SceneFlowConfig : ScriptableObject
    {
        [Header("起始场景设置")]
        [Tooltip("游戏开始场景")]
        public string startScene = "GameStart";

        [Header("场景流程定义")]
        [Tooltip("每个首关场景对应的完整流程链")]
        public List<SceneFlowChain> sceneFlowChains = new List<SceneFlowChain>();


        /// <summary>
        /// 获取随机的首关流程链
        /// </summary>
        public SceneFlowChain GetRandomFirstLevelChain()
        {
            if (sceneFlowChains.Count == 0) return null;
            int randomIndex = UnityEngine.Random.Range(0, sceneFlowChains.Count);
            return sceneFlowChains[randomIndex];
        }

        /// <summary>
        /// 根据场景名称获取所属的流程链
        /// </summary>
        public SceneFlowChain GetChainByScene(string sceneName)
        {
            foreach (var chain in sceneFlowChains)
            {
                if (chain.ContainsScene(sceneName)) return chain;
            }
            return null;
        }

        /// <summary>
        /// 获取当前场景的下一个场景
        /// </summary>
        public string GetNextScene(string currentScene)
        {
            var chain = GetChainByScene(currentScene);
            return chain?.GetNextScene(currentScene);
        }

        /// <summary>
        /// 检查是否为最终关卡
        /// </summary>
        public bool IsFinalLevel(string sceneName)
        {
            var chain = GetChainByScene(sceneName);
            return chain?.IsLastScene(sceneName) ?? false;
        }

        /// <summary>
        /// 获取场景在流程中的位置信息
        /// </summary>
        public string GetScenePositionInfo(string sceneName)
        {
            var chain = GetChainByScene(sceneName);
            if (chain == null) return "未知流程";

            int position = chain.GetScenePosition(sceneName);
            return $"{chain.chainName} 第{position}关/共{chain.TotalScenes}关";
        }

        /// <summary>
        /// 验证场景配置的有效性
        /// </summary>
        public bool ValidateConfig()
        {
            if (string.IsNullOrEmpty(startScene))
            {
                Debug.LogError("起始场景未设置");
                return false;
            }

            if (sceneFlowChains.Count == 0)
            {
                Debug.LogError("未定义任何场景流程链");
                return false;
            }

            foreach (var chain in sceneFlowChains)
            {
                if (string.IsNullOrWhiteSpace(chain.chainName))
                {
                    Debug.LogError("存在未命名的流程链");
                    return false;
                }

                if (chain.sceneSequence.Count == 0)
                {
                    Debug.LogError($"流程链 '{chain.chainName}' 未定义任何场景");
                    return false;
                }

                // 检查场景重复
                var distinctScenes = new HashSet<string>();
                foreach (var scene in chain.sceneSequence)
                {
                    if (string.IsNullOrWhiteSpace(scene))
                    {
                        Debug.LogError($"流程链 '{chain.chainName}' 中存在空场景");
                        return false;
                    }

                    if (distinctScenes.Contains(scene))
                    {
                        Debug.LogError($"流程链 '{chain.chainName}' 中存在重复场景: {scene}");
                        return false;
                    }
                    distinctScenes.Add(scene);
                }
            }

            return true;
        }
    }
}
