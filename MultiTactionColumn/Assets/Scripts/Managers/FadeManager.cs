using UnityEngine;
using System.Collections;
using M1.Utilities;

public class FadeManager : SingletonBehaviour<FadeManager>
{

    public static void FadeIn(CanvasGroup c, float _time, float _alpha = 1f)
    {
        Instance.StartCoroutine(Instance.iFadeIn(c, _time, _alpha));
    }

    IEnumerator iFadeIn(CanvasGroup c, float _time, float _alpha = 1f)
    {
        c.gameObject.SetActive(true);
        disableCanvasGroup(c, true);

        while (c.alpha < _alpha)
        {
            c.alpha += Time.deltaTime / _time;
            yield return null;
        }

        c.alpha = _alpha;

        enableCanvasGroup(c, false);
    }

    public static void FadeOut(CanvasGroup c, float _time, bool _turnOffC = false, float _alpha = 0f)
    {
        Instance.StartCoroutine(Instance.iFadeOut(c, _time, _turnOffC, _alpha));
    }

    IEnumerator iFadeOut(CanvasGroup c, float _time, bool _turnOffC = false, float _alpha = 0f)
    {
        while (c.alpha > _alpha)
        {
            c.alpha -= Time.deltaTime / _time;
            yield return null;
        }

        c.alpha = _alpha;

        disableCanvasGroup(c, false);
        if (_turnOffC) c.gameObject.SetActive(false);
    }

    public static void EnableCanvasGroup(CanvasGroup c, bool setAlpha)
    {
        Instance.enableCanvasGroup(c, setAlpha);
    }

    private void enableCanvasGroup(CanvasGroup c, bool setAlpha)
    {
        c.interactable = true;
        c.blocksRaycasts = true;
        if (setAlpha) c.alpha = 1f;
    }

    public static void DisableCanvasGroup(CanvasGroup c, bool setAlpha)
    {
        Instance.disableCanvasGroup(c, setAlpha);
    }

    private void disableCanvasGroup(CanvasGroup c, bool setAlpha)
    {
        c.interactable = false;
        c.blocksRaycasts = false;
        if (setAlpha) c.alpha = 0f;
    }
}
