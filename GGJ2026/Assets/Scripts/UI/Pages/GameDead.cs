using GGJ2026.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDead : MonoBehaviour
{
    [SerializeField] private Button m_gameReset;
    private void Start()
    {
        m_gameReset.onClick.AddListener(OnGameReset);
    }

    private void OnDestroy()
    {
        m_gameReset.onClick.RemoveListener(OnGameReset);
    }

    private void OnGameReset()
    {
        GameManager.Instance.ReloadCurrentLevelScene();
    }
}
