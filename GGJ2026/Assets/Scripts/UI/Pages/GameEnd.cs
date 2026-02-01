using GGJ2026.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEnd : MonoBehaviour
{
    [SerializeField] private Button m_gameEnd;
    private void Start()
    {
        m_gameEnd.onClick.AddListener(OnGameEnd);
    }

    private void OnDestroy()
    {
        m_gameEnd.onClick.RemoveListener(OnGameEnd);
    }

    private void OnGameEnd()
    {
        GameManager.Instance.LoadMainMenu();
    }
}
