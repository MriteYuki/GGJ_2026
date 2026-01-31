using GGJ2026.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStart : MonoBehaviour
{
    [SerializeField] private Button m_gameStart;
    private void Start()
    {
        m_gameStart.onClick.AddListener(OnGameStart);
    }

    private void OnDestroy()
    {
        m_gameStart.onClick.RemoveListener(OnGameStart);
    }

    private void OnGameStart()
    {
        GameManager.Instance.LoadNextScene();
    }
}
