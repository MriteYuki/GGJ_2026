using GGJ2026.Gameplay;
using GGJ2026.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;

    private void Awake()
    {
        UIEventSystem.Instance.Subscribe(UIEventTypes.DESC_SHOW, OnDescShow);
        UIEventSystem.Instance.Subscribe(UIEventTypes.DESC_HIDE, OnDescHide);

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (UIEventSystem.IsExist)
        {
            UIEventSystem.Instance.Unsubscribe(UIEventTypes.DESC_SHOW, OnDescShow);
            UIEventSystem.Instance.Unsubscribe(UIEventTypes.DESC_HIDE, OnDescHide);
        }
    }

    private void OnDescShow(object data)
    {
        if(data is ItemData item)
        {
            var scaleName = item.scaleType switch
            {
                GGJ2026.ScaleType.Large => "¡¾´ó¡¿",
                GGJ2026.ScaleType.Medium => "¡¾ÖÐ¡¿",
                GGJ2026.ScaleType.Small => "¡¾Ð¡¡¿",
                _ => "¡¾£¿£¿£¿¡¿"
            };

            if (nameText)
            {
                nameText.text = item.name + scaleName;
            }
            if (descriptionText)
            {
                descriptionText.text = item.description;
            }
        }

        gameObject.SetActive(true);
        Debug.Log("Show Desc");
    }

    private void OnDescHide(object data)
    {
        gameObject.SetActive(false);
        Debug.Log("Hide Desc");
    }
}
