using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonBoop : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public float pressedScale = 0.92f;
    public float lerp = 18f;
    Vector3 _target = Vector3.one;

    void Update() { transform.localScale = Vector3.Lerp(transform.localScale, _target, Time.unscaledDeltaTime * lerp); }
    public void OnPointerDown(PointerEventData e) { _target = Vector3.one * pressedScale; }
    public void OnPointerUp(PointerEventData e)   { _target = Vector3.one; }
    public void OnPointerExit(PointerEventData e) { _target = Vector3.one; }
}