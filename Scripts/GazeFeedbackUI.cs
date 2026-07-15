using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(AudioSource))] // 自动添加
public class GazeFeedbackUI : MonoBehaviour
{
    public Image crosshair;
    public Color normalColor = Color.white;
    public Color targetColor = Color.green;

    public GameObject readingRootText;
    public GameObject readingRootSlider;
    public Slider readingSlider;

    [Header("扫描音效设置")]
    public AudioClip scanningLoopSound; // 建议找一个可以循环的“滋滋”或“嘀嘀”声
    [Range(0f, 1f)] public float scanningVolume = 0.5f;
    private AudioSource _audioSource;

    private bool _isUIHidden = false;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = scanningLoopSound;
        _audioSource.loop = true; // 扫描音效通常需要循环
        _audioSource.playOnAwake = false;
        _audioSource.spatialBlend = 0f; // UI音效设为2D
    }

    void Update()
    {
        bool shouldHide = PauseMenuManager.IsPaused ||
                          GameState.IsOpening ||
                          GameState.IsEnding ||
                          (StoryUI.Instance != null && StoryUI.Instance.IsOpen);

        if (shouldHide)
        {
            if (!_isUIHidden)
            {
                SetUIVisible(false);
                StopScanningSound(); // 隐藏UI时必须停止声音
            }
        }
        else
        {
            if (_isUIHidden)
            {
                _isUIHidden = false;
                if (crosshair) crosshair.gameObject.SetActive(true);
            }
        }
    }

    public void SetIdle()
    {
        if (_isUIHidden || PauseMenuManager.IsPaused || GameState.IsOpening || (StoryUI.Instance != null && StoryUI.Instance.IsOpen))
        {
            StopScanningSound();
            return;
        }

        if (crosshair) crosshair.color = normalColor;
        if (readingRootText) readingRootText.SetActive(false);
        if (readingRootSlider) readingRootSlider.SetActive(false);
        if (readingSlider) readingSlider.value = 0f;

        // 停止扫描音效
        StopScanningSound();
    }

    public void SetAiming(float progress01)
    {
        if (_isUIHidden || PauseMenuManager.IsPaused || GameState.IsOpening || (StoryUI.Instance != null && StoryUI.Instance.IsOpen))
        {
            StopScanningSound();
            return;
        }

        if (crosshair) crosshair.color = targetColor;
        if (readingRootText) readingRootText.SetActive(true);
        if (readingRootSlider) readingRootSlider.SetActive(true);
        if (readingSlider) readingSlider.value = Mathf.Clamp01(progress01);

        // --- 核心逻辑：播放声音 ---
        if (!_audioSource.isPlaying && scanningLoopSound != null)
        {
            _audioSource.volume = scanningVolume;
            _audioSource.Play();
        }

    }

    private void StopScanningSound()
    {
        if (_audioSource.isPlaying)
        {
            _audioSource.Stop();
        }
    }

    public void SetUIVisible(bool visible)
    {
        _isUIHidden = !visible;
        if (crosshair) crosshair.gameObject.SetActive(visible);
        if (readingRootText) readingRootText.SetActive(visible);
        if (readingRootSlider) readingRootSlider.SetActive(visible);

        if (!visible)
        {
            if (readingSlider != null) readingSlider.value = 0f;
            StopScanningSound();
        }
    }
}