using GGJ2026.Gameplay;
using GGJ2026.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class TimeInfo : MonoBehaviour
{

    [SerializeField] private TMP_Text timeText;

    private void Awake()
    {
        UIEventSystem.Instance.Subscribe(UIEventTypes.UI_TIME_UPDATE, OnTimeUpdate);
    }

    private void OnDestroy()
    {
        if (UIEventSystem.IsExist)
        {
            UIEventSystem.Instance.Unsubscribe(UIEventTypes.UI_TIME_UPDATE, OnTimeUpdate);
        }
    }

    private void OnTimeUpdate(object data)
    {
        if (data is string time)
        {
            if (timeText)
            {
                timeText.text = time;
            }

            Debug.Log($"Time Update: {time}");
        }
    }
}
