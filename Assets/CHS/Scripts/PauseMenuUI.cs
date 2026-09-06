using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class PauseMenuUI : MonoBehaviour
{
    [Header("Refs")]
    public GameObject pausePanel;
    public Button pauseButton;    // 우상단 버튼
    public Button closeBtn;       // ✕ 버튼 (추가)
    public Button resumeBtn;
    public Button restartBtn;
    public Button exitBtn;
    public Slider volumeSlider;
    public TextMeshProUGUI titleText;

    [Header("Audio")]
    public AudioMixer masterMixer;
    public string exposedVolumeParam = "MasterVolume";

    [Header("Options")]
    [Range(0f, 1f)] public float defaultVolume = 0.8f;
    public bool pauseUsesTimescale = true;

    float _prevTimeScale = 1f;
    bool _paused = false;
    const string PREF_KEY = "MasterVolume";

    void Awake()
    {
        // 버튼 이벤트 연결
        if (pauseButton) pauseButton.onClick.AddListener(TogglePause);
        if (closeBtn) closeBtn.onClick.AddListener(Resume); // ✕ 버튼 → 닫기
        if (resumeBtn) resumeBtn.onClick.AddListener(Resume);
        if (restartBtn) restartBtn.onClick.AddListener(Restart);
        if (exitBtn) exitBtn.onClick.AddListener(ExitGame);

        // 볼륨 슬라이더 초기화
        if (volumeSlider)
        {
            volumeSlider.minValue = 0.0001f;
            volumeSlider.maxValue = 1f;
            volumeSlider.onValueChanged.AddListener(SetVolume);
        }

        pausePanel.SetActive(false);

        float v = PlayerPrefs.GetFloat(PREF_KEY, defaultVolume);
        if (volumeSlider) volumeSlider.value = v;
        ApplyVolumeToMixer(v);
    }

    public void TogglePause()
    {
        if (_paused) Resume();
        else Pause();
    }

    public void Pause()
    {
        if (_paused) return;
        _paused = true;

        var rt = pausePanel.GetComponent<RectTransform>();
        rt.SetAsLastSibling();
        pausePanel.SetActive(true);

        if (pauseUsesTimescale)
        {
            _prevTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }

        if (titleText) titleText.text = "PAUSED";
    }

    public void Resume()
    {
        if (!_paused) return;
        _paused = false;

        if (pauseUsesTimescale)
            Time.timeScale = _prevTimeScale;

        pausePanel.SetActive(false);
    }

    public void Restart()
    {
        var gm = FindObjectOfType<GameUIManager>();
        if (gm != null) gm.Restart();
        Resume();
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void SetVolume(float v)
    {
        ApplyVolumeToMixer(v);
        PlayerPrefs.SetFloat(PREF_KEY, v);
    }

    void ApplyVolumeToMixer(float v)
    {
        if (masterMixer == null) return;
        float dB = Mathf.Log10(Mathf.Clamp(v, 0.0001f, 1f)) * 20f;
        masterMixer.SetFloat(exposedVolumeParam, dB);
    }
}
