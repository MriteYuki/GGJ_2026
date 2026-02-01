using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGJ2026.UI;

public class ShowTips : MonoBehaviour
{
    private void Awake()
    {
        UIEventSystem.Instance.Subscribe(UIEventTypes.TIPS_SHOW, OnDescShow);
        UIEventSystem.Instance.Subscribe(UIEventTypes.TIPS_HIDE, OnDescHide);

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (UIEventSystem.IsExist)
        {
            UIEventSystem.Instance.Unsubscribe(UIEventTypes.TIPS_SHOW, OnDescShow);
            UIEventSystem.Instance.Unsubscribe(UIEventTypes.TIPS_HIDE, OnDescHide);
        }
    }

    private void OnDescShow(object data)
    {
        gameObject.SetActive(true);
    }

    private void OnDescHide(object data)
    {
        gameObject.SetActive(false);
    }
}
