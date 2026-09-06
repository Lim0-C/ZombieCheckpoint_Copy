using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public enum ExitDir { Left, Right, Down }

public class NPC : MonoBehaviour, IPointerClickHandler
{
    [Header("Type")]
    public NPCType type;

    [Header("Zombie Common")]
    public int tapsToKill = 3;
    private int _tapCount = 0;

    [Header("Sticky Zombie")]
    public bool isSticky = false;        // 철퍼덕 달라붙는 좀비 여부
    public float stickyDelay = 0.8f;     // 등장 후 붙기까지 대기
    public float stickyScale = 1.6f;     // 붙은 후 크기
    public float stickyTimeLimit = 2.5f; // 붙은 후 제한시간(0=무제한)
    private bool _sticking = false;
    private float _stickyTimer = 0f;
    private RectTransform _stickyLayer;  // GameUIManager에서 주입

    [Header("Enter & Exit")]
    public float enterDuration = 0.35f;
    public Vector2 startOffset = new Vector2(0, -600f);
    public float slideDistance = 900f;
    public float slideDuration = 0.35f;

    [Header("FX")]
    public float shakeStrength = 24f;
    public float shakeDuration = 0.25f;
    public float killPopScale = 1.25f;
    public float killFxDuration = 0.35f;
    public float killFallDistance = 300f;

    public event Action<NPC> OnZombieKilled;       // 처치 완료
    public event Action<NPC> OnStickyAttach;       // 달라붙는 순간
    public event Action<NPC> OnStickyReleased;     // 떼어냄(사망 포함)
    public event Action<NPC> OnStickyTimeout;      // 붙은 채 제한시간 초과

    private RectTransform _rt;
    private CanvasGroup _cg;
    private Vector2 _centerPos;
    private bool _isAnimating = false;
    private bool _alive = true;

    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
        _cg = GetComponent<CanvasGroup>();
        if (_cg == null) _cg = gameObject.AddComponent<CanvasGroup>();
        _cg.alpha = 1f;
    }

    public void AssignStickyLayer(RectTransform layer) => _stickyLayer = layer;

    public void SetupAtCenter(RectTransform lane)
    {
        _rt.SetParent(lane, false);
        _rt.anchorMin = _rt.anchorMax = new Vector2(0.5f, 0.5f);
        _rt.pivot = new Vector2(0.5f, 0.5f);

        _centerPos = Vector2.zero;
        _rt.anchoredPosition = _centerPos + startOffset;
        transform.localScale = Vector3.one;
        _cg.alpha = 1f;

        _alive = true;
        _sticking = false;
        _stickyTimer = 0f;

        StopAllCoroutines();
        StartCoroutine(Co_EnterThenMaybeStick());
    }

    private IEnumerator Co_EnterThenMaybeStick()
    {
        // Enter
        _isAnimating = true;
        float t = 0f;
        Vector2 from = _rt.anchoredPosition;
        while (t < enterDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / enterDuration);
            float e = 1f - Mathf.Pow(1f - k, 3f);
            _rt.anchoredPosition = Vector2.LerpUnclamped(from, _centerPos, e);
            yield return null;
        }
        _rt.anchoredPosition = _centerPos;
        _isAnimating = false;

        // Sticky 좀비면 잠시 후 철퍼덕!
        if (type == NPCType.Zombie && isSticky && _stickyLayer != null)
        {
            yield return new WaitForSeconds(stickyDelay);
            if (_alive) yield return StartCoroutine(Co_StickToScreen());
        }
    }

    private IEnumerator Co_StickToScreen()
    {
        // 부모를 StickyLayer로 이동(화면 최상단 레이어 중앙)
        _rt.SetParent(_stickyLayer, false);
        _rt.anchorMin = _rt.anchorMax = new Vector2(0.5f, 0.5f);
        _rt.pivot = new Vector2(0.5f, 0.5f);

        // 철퍼덕: 약간 튀듯이 커지며 중앙 고정
        _isAnimating = true;
        _sticking = true;
        OnStickyAttach?.Invoke(this);

        float dur = 0.18f;
        float t = 0f;
        Vector3 s0 = transform.localScale;
        Vector3 s1 = Vector3.one * stickyScale;

        while (t < dur)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / dur);
            float e = 1f - Mathf.Pow(1f - k, 2f);
            transform.localScale = Vector3.LerpUnclamped(s0, s1, e);
            yield return null;
        }
        transform.localScale = s1;
        _isAnimating = false;

        // 제한시간 카운트(있다면)
        _stickyTimer = 0f;
        while (_alive && _sticking && (stickyTimeLimit <= 0f || _stickyTimer < stickyTimeLimit))
        {
            _stickyTimer += Time.deltaTime;
            yield return null;
        }
        // 시간 초과
        if (_alive && _sticking && stickyTimeLimit > 0f)
        {
            _sticking = false;
            OnStickyTimeout?.Invoke(this);
            // 패널티 느낌으로 아래로 미끄러짐
            yield return StartCoroutine(Co_SlideOut(ExitDir.Down));
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_alive || _isAnimating) return;
        if (type != NPCType.Zombie) return;

        _tapCount++;
        StopCoroutine(nameof(Co_Boop));
        StartCoroutine(Co_Boop());

        if (_tapCount >= tapsToKill)
        {
            StopAllCoroutines();
            StartCoroutine(Co_KillZombie());
        }
    }

    private IEnumerator Co_Boop()
    {
        float dur = 0.12f;
        Vector3 baseScale = transform.localScale;
        Vector3 big = baseScale * 1.08f;
        float t = 0f;
        while (t < dur) { t += Time.deltaTime; transform.localScale = Vector3.Lerp(baseScale, big, t/dur); yield return null; }
        t = 0f;
        while (t < dur) { t += Time.deltaTime; transform.localScale = Vector3.Lerp(big, baseScale, t/dur); yield return null; }
        transform.localScale = baseScale;
    }

    public IEnumerator Co_SlideOut(ExitDir dir)
    {
        _isAnimating = true;
        Vector2 start = _rt.anchoredPosition;
        Vector2 end = start + (dir == ExitDir.Left ? Vector2.left :
                               dir == ExitDir.Right ? Vector2.right : Vector2.down) * slideDistance;

        float t = 0f;
        while (t < slideDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / slideDuration);
            float e = k * k;
            _rt.anchoredPosition = Vector2.LerpUnclamped(start, end, e);
            _cg.alpha = 1f - (k * 0.6f);
            yield return null;
        }
        _rt.anchoredPosition = end;
        _cg.alpha = 0.4f;

        _isAnimating = false;
        _alive = false;
        if (_sticking) { _sticking = false; OnStickyReleased?.Invoke(this); }
    }

    public IEnumerator Co_ShakeWrong()
    {
        _isAnimating = true;
        Vector2 basePos = Vector2.zero;
        float t = 0f;
        while (t < shakeDuration)
        {
            t += Time.deltaTime;
            float k = t / shakeDuration;
            float damper = 1f - k;
            float offset = Mathf.Sin(k * 50f) * shakeStrength * damper;
            _rt.anchoredPosition = basePos + new Vector2(offset, 0f);
            yield return null;
        }
        _rt.anchoredPosition = basePos;
        _isAnimating = false;
    }

    private IEnumerator Co_KillZombie()
    {
        _isAnimating = true;
        _alive = false;

        float t = 0f;
        Vector3 s0 = transform.localScale;
        Vector3 s1 = Vector3.one * killPopScale;
        Vector2 p0 = _rt.anchoredPosition;
        Vector2 p1 = p0 + Vector2.down * killFallDistance;

        while (t < killFxDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / killFxDuration);
            float e = 1f - Mathf.Pow(1f - k, 2f);
            transform.localScale = Vector3.LerpUnclamped(s0, s1, e);
            _cg.alpha = 1f - k;
            _rt.anchoredPosition = Vector2.LerpUnclamped(p0, p1, e);
            yield return null;
        }
        _cg.alpha = 0f;
        transform.localScale = s1;
        _rt.anchoredPosition = p1;

        if (_sticking) { _sticking = false; OnStickyReleased?.Invoke(this); }
        OnZombieKilled?.Invoke(this);
        _isAnimating = false;
    }
}
