using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIWindow : MonoBehaviour
{
    public void Open()
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(Animate(true));
    }
    public void Close()
    {
        StopAllCoroutines();
        StartCoroutine(Animate(false));
    }
    IEnumerator Animate(bool opening)
    {
        CanvasGroup group = GetComponent<CanvasGroup>();
        group.interactable = false;
        group.blocksRaycasts = true;
        float start = opening ? 0 : 1;
        float end = opening ? 1 : 0;
        float time = 0;
        while (time < 0.2f)
        {
            time += Time.unscaledDeltaTime;
            float value = Mathf.Lerp(start, end, time / 0.2f);
            group.alpha = value;
            transform.localScale = Vector3.one * Mathf.Lerp(0.95f, 1f, value);
            yield return null;
        }
        group.alpha = end;
        transform.localScale = Vector3.one;
        group.interactable = opening;
        if (!opening) gameObject.SetActive(false);
    }
}
