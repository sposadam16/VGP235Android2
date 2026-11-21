using UnityEngine;

public class SafeArea : MonoBehaviour
{
    Rect lastArea;
    void Update()
    {
        Rect area = Screen.safeArea;
        if (area == lastArea) return;
        lastArea = area;
        RectTransform rect = (RectTransform)transform;
        rect.anchorMin = new Vector2(area.xMin / Screen.width, area.yMin / Screen.height);
        rect.anchorMax = new Vector2(area.xMax / Screen.width, area.yMax / Screen.height);
        rect.offsetMin = rect.offsetMax = Vector2.zero;
    }
}
