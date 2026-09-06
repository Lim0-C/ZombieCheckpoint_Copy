using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }

    // ---------- References ----------
    [Header("Refs")]
    public SpawnManager spawnManager;
    public RectTransform stickyLayer;        // 철퍼덕 좀비가 붙는 전체화면 레이어
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI statusText;

    [Header("Gauge UI")]
    public Image gaugeFill;                  // Image(Type=Filled, Horizontal)
    [Range(0f, 1f)] public float startGauge = 1.0f; // ▶ 기본 풀게이지(초록)

    // ---------- Balance ----------
    [Header("Gauge Balance")]
    public float drainPerSecond    = 0.06f;  // 기본 초당 감소
    public float stickyExtraDrain  = 0.12f;  // 달라붙어 있는 동안 추가 감소
    public float gainOnCorrect     = 0.08f;  // 시민/의심자 정답
    public float lossOnWrong       = 0.12f;  // 분류 실패
    public float gainOnZombie      = 0.15f;  // 좀비 격파
    public float lossStickyTimeout = 0.25f;  // 붙은 채 시간초과

    // ---------- Gauge Color FX ----------
    [Header("Gauge Color FX")]
    [SerializeField] Color gaugeHigh = new Color(0.20f, 1f, 0.40f); // 초록
    [SerializeField] Color gaugeMid  = new Color(1f, 0.90f, 0.20f); // 노랑
    [SerializeField] Color gaugeLow  = new Color(1f, 0.25f, 0.20f); // 빨강
    [SerializeField] float blinkThreshold = 0.15f;  // 이 값 이하에서 깜빡임
    [SerializeField] float blinkSpeed = 6f;         // 깜빡임 속도
    [SerializeField] float startDrainFreezeSeconds = 1.0f; // ▶ 시작 직후 드레인 잠깐 멈춤

    // ---------- Game Over ----------
    [Header("Game Over UI")]
    public GameObject gameOverPanel;         // 비활성 시작
    public TextMeshProUGUI finalScoreText;
    public Button retryButton;

    [Header("Timings")]
    public float nextDelay = 0.3f;
    public float statusFade = 0.6f;

    // ---------- Runtime State ----------
    private int _score = 0;
    private float _gauge;
    private NPC _current;
    private bool _inputLocked = false;
    private bool _stickyActive = false;      // 화면에 좀비가 철퍼덕 붙어 있는가
    private float _startDrainFreezeTimer = 0f;

    // =========================================================

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (retryButton) retryButton.onClick.AddListener(Restart);

        // 패널은 꺼두고 시작
        SafeHideGameOverPanel();

        statusText.text = "";

        _score = 0;
        _gauge = Mathf.Clamp01(startGauge);
        _startDrainFreezeTimer = startDrainFreezeSeconds; // ▶ 시작 직후 초록 유지
        UpdateHUD();

        SpawnNext();
    }

    private void Update()
    {
        if (gameOverPanel.activeSelf) return;

        // 시간 경과에 따른 게이지 감소 (붙어있으면 추가 감소)
        float drain = 0f;
        if (_startDrainFreezeTimer > 0f)
        {
            _startDrainFreezeTimer -= Time.deltaTime;
        }
        else
        {
            drain = drainPerSecond + (_stickyActive ? stickyExtraDrain : 0f);
        }

        _gauge = Mathf.Max(0f, _gauge - drain * Time.deltaTime);
        UpdateGaugeUI();

        if (_gauge <= 0f)
        {
            StartCoroutine(Co_GameOver());
        }
    }

    // =========================================================
    // Spawning & Input
    // =========================================================

    private void SpawnNext()
    {
        _current = spawnManager.SpawnRandom();

        if (_current.type == NPCType.Zombie)
        {
            // 철퍼덕 좀비 대응: 콜백 연결 + 레이어 주입
            _current.AssignStickyLayer(stickyLayer);
            _current.OnZombieKilled    += OnZombieKilled;
            _current.OnStickyAttach    += OnStickyAttach;
            _current.OnStickyReleased  += OnStickyReleased;
            _current.OnStickyTimeout   += OnStickyTimeout;
        }
    }

    /// <summary>
    /// 좌/우 영역 터치 혹은 버튼 클릭에서 호출 (isLeftZone=true: 왼쪽/수용, false: 오른쪽/격리)
    /// </summary>
    public void TryClassifyByZone(bool isLeftZone)
    {
        if (_inputLocked || _current == null) return;

        // 달라붙어 있으면 먼저 떼어내야 함
        if (_stickyActive)
        {
            QuickStatus("먼저 달라붙은 좀비부터 떼어내!", new Color(1f, 0.85f, 0.2f));
            return;
        }

        // 좀비는 본체 클릭으로만 처리
        if (_current.type == NPCType.Zombie)
        {
            QuickStatus("좀비! 본체를 연타해!", new Color(1f, 0.9f, 0.2f));
            return;
        }

        bool correct =
            (_current.type == NPCType.Citizen && isLeftZone) ||
            (_current.type == NPCType.Suspect && !isLeftZone);

        if (correct) StartCoroutine(Co_OnCorrect(isLeftZone));
        else         StartCoroutine(Co_OnWrong());
    }

    // 버튼용 편의 함수 (Inspector에서 Button OnClick에 연결)
    public void OnClickAccept()    { TryClassifyByZone(true);  }   // 시민=왼쪽
    public void OnClickIsolation() { TryClassifyByZone(false); }   // 의심자=오른쪽

    // =========================================================
    // Sticky Zombie Callbacks
    // =========================================================

    private void OnStickyAttach(NPC z)
    {
        if (_current != z) return;
        _stickyActive = true;
        QuickStatus("철퍼덕! 연타로 떼어내!", new Color(1f, 0.75f, 0.25f));
    }

    private void OnStickyReleased(NPC z)
    {
        if (_current != z) return;
        _stickyActive = false;
    }

    private void OnStickyTimeout(NPC z)
    {
        if (_current != z) return;
        _stickyActive = false;

        _gauge = Mathf.Clamp01(_gauge - lossStickyTimeout);
        QuickStatus("시간초과! 게이지 크게 감소", new Color(1f, 0.45f, 0.25f));
        UpdateHUD();

        StartCoroutine(Co_ProceedAfterAnim());
    }

    private void OnZombieKilled(NPC z)
    {
        if (_current != z) return;
        _stickyActive = false;

        _score += 3;
        _gauge = Mathf.Clamp01(_gauge + gainOnZombie);
        QuickStatus("좀비 격파! +게이지", new Color(0.2f, 1f, 0.35f));
        UpdateHUD();

        StartCoroutine(Co_ProceedAfterAnim());
    }

    // =========================================================
    // Outcomes
    // =========================================================

    private IEnumerator Co_OnCorrect(bool isLeftZone)
    {
        _inputLocked = true;

        _score += 1;
        _gauge = Mathf.Clamp01(_gauge + gainOnCorrect);
        QuickStatus("분류 성공! +게이지", new Color(0.3f, 0.9f, 1f));
        UpdateHUD();

        // 좌/우로 슬라이드아웃
        if (_current != null)
        {
            yield return _current.StartCoroutine(
                _current.Co_SlideOut(isLeftZone ? ExitDir.Left : ExitDir.Right)
            );
        }

        yield return new WaitForSeconds(nextDelay);
        ProceedSpawn();
    }

    private IEnumerator Co_OnWrong()
    {
        _inputLocked = true;

        _gauge = Mathf.Clamp01(_gauge - lossOnWrong);
        QuickStatus("분류 실패! -게이지", Color.red);
        UpdateHUD();

        if (_current != null)
            yield return _current.StartCoroutine(_current.Co_ShakeWrong());

        if (_gauge <= 0f)
        {
            yield return StartCoroutine(Co_GameOver());
            yield break;
        }

        yield return new WaitForSeconds(nextDelay);
        ProceedSpawn();
    }

    private IEnumerator Co_ProceedAfterAnim()
    {
        _inputLocked = true;
        UpdateHUD();
        yield return new WaitForSeconds(nextDelay);
        ProceedSpawn();
    }

    private void ProceedSpawn()
    {
        if (_current != null) Destroy(_current.gameObject);
        _current = null;

        _inputLocked = false;
        SpawnNext();
    }

    // =========================================================
    // UI & Game Over
    // =========================================================

    private void SafeHideGameOverPanel()
    {
        if (!gameOverPanel) return;

        var cg = gameOverPanel.GetComponent<CanvasGroup>();
        if (cg == null) cg = gameOverPanel.AddComponent<CanvasGroup>();
        cg.alpha = 0f; cg.interactable = false; cg.blocksRaycasts = false;

        // 패널은 비활성으로 시작
        gameOverPanel.SetActive(false);
    }

    private IEnumerator Co_GameOver()
    {
        if (gameOverPanel.activeSelf) yield break;

        _inputLocked = true;
        if (_current != null) Destroy(_current.gameObject);
        _current = null;

        yield return new WaitForSeconds(0.25f);

        // ▶ 항상 최상단/클릭가능 보정 + 활성화
        var panelRt = gameOverPanel.GetComponent<RectTransform>();
        panelRt.SetAsLastSibling(); // 같은 Canvas 내 최상단

        var cg = gameOverPanel.GetComponent<CanvasGroup>();
        if (cg == null) cg = gameOverPanel.AddComponent<CanvasGroup>();

        var cv = gameOverPanel.GetComponent<Canvas>(); // 별도 Canvas가 있다면 정렬 보정
        if (cv != null) { cv.overrideSorting = true; cv.sortingOrder = 5000; }

        if (finalScoreText) finalScoreText.text = $"Score : {_score}";

        gameOverPanel.SetActive(true);
        cg.alpha = 1f; cg.interactable = true; cg.blocksRaycasts = true;
    }

    public void Restart()
    {
        _score = 0;
        _gauge = Mathf.Clamp01(startGauge);
        _stickyActive = false;
        _startDrainFreezeTimer = startDrainFreezeSeconds;

        SafeHideGameOverPanel();

        statusText.text = "";
        UpdateHUD();

        if (_current != null) Destroy(_current.gameObject);
        _current = null;

        _inputLocked = false;
        SpawnNext();
    }

    private void UpdateHUD()
    {
        if (scoreText) scoreText.text = _score.ToString();
        UpdateGaugeUI();
    }

    private void UpdateGaugeUI()
    {
        if (gaugeFill == null) return;

        gaugeFill.fillAmount = _gauge;

        // 초록(High) ↔ 노랑(Mid) ↔ 빨강(Low) 그라데이션
        Color c;
        if (_gauge >= 0.5f)
        {
            float t = (_gauge - 0.5f) / 0.5f; // 0~1
            c = Color.Lerp(gaugeMid, gaugeHigh, Mathf.Clamp01(t));
        }
        else
        {
            float t = _gauge / 0.5f;          // 0~1
            c = Color.Lerp(gaugeLow, gaugeMid, Mathf.Clamp01(t));
        }

        // 위험 구간에서 가벼운 깜빡임 효과
        if (_gauge <= blinkThreshold)
        {
            float pulse = 0.5f + 0.5f * Mathf.Sin(Time.time * blinkSpeed);
            c = Color.Lerp(c, Color.white, 0.25f * pulse);
        }

        gaugeFill.color = c;
    }

    private void QuickStatus(string msg, Color c)
    {
        StopCoroutine(nameof(Co_Status));
        StartCoroutine(Co_Status(msg, c));
    }

    private IEnumerator Co_Status(string msg, Color c)
    {
        if (!statusText) yield break;

        statusText.text = msg;
        statusText.color = c;
        statusText.alpha = 1f;

        float t = 0f;
        while (t < statusFade)
        {
            t += Time.deltaTime;
            float k = t / statusFade;
            statusText.alpha = 1f - k;
            yield return null;
        }
        statusText.text = "";
    }

    // ---- 디버그: 강제 오픈/닫기 (원하면 버튼에 연결해서 테스트) ----
    public void Debug_ShowGameOver()
    {
        StopAllCoroutines();
        StartCoroutine(Co_GameOver());
    }
    public void Debug_HideGameOver()
    {
        SafeHideGameOverPanel();
    }
}
