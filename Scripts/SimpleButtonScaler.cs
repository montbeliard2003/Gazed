using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleButtonScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // 当鼠标进入按钮区域时触发（放大到 1.2 倍）
    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = Vector3.one * 1.2f;
    }

    // 当鼠标离开按钮区域时触发（恢复原大小）
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
    }
}