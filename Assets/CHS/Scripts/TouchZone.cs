using UnityEngine;
using UnityEngine.EventSystems;

public class TouchZone : MonoBehaviour, IPointerDownHandler
{
    public bool isLeft; // LeftZone = true, RightZone = false

    public void OnPointerDown(PointerEventData eventData)
    {
        GameUIManager.Instance.TryClassifyByZone(isLeft);
    }
}