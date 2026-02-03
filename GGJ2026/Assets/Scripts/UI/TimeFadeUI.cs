using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedFadeUI : MonoBehaviour
{
    public float showTime = 2f;
    public float fadeTime = 0.3f;

    CanvasGroup group;
    Coroutine routine;

    void Awake()
    {
        group = GetComponent<CanvasGroup>();
        group.alpha = 0;
        gameObject.SetActive(false);
    }

    public void Show()
    {
        if (routine != null)
            StopCoroutine(routine);

        gameObject.SetActive(true);
        routine = StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        group.alpha = 1;
        yield return new WaitForSeconds(showTime);

        float t = 0;
        while (t < fadeTime)
        {
            t += Time.deltaTime;
            group.alpha = Mathf.Lerp(1, 0, t / fadeTime);
            yield return null;
        }

        group.alpha = 0;
        gameObject.SetActive(false);
    }
}
