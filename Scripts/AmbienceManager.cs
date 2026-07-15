using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AmbienceManager : MonoBehaviour
{
    // 单例：让其他脚本（如 OpeningManager, EndingManager）能轻松找到它
    public static AmbienceManager Instance { get; private set; }

    private AudioSource _audioSource;

    [Header("音量设置")]
    [Range(0f, 1f)] public float maxVolume = 0.3f; // 环境音最终的音量
    public float fadeInDuration = 3.0f;           // 淡入持续时间（秒）
    public float fadeOutDuration = 3.0f;          // --- 新增：淡出持续时间（秒） ---

    private Coroutine _fadeCoroutine;             // --- 新增：用于记录当前的淡入淡出协程，防止冲突 ---

    void Awake()
    {
        // 初始化单例
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); return; }

        _audioSource = GetComponent<AudioSource>();

        // 核心配置：确保它是 2D 且初始无声
        _audioSource.playOnAwake = false;
        _audioSource.loop = true;
        _audioSource.spatialBlend = 0f;
        _audioSource.volume = 0f;
    }

    // 供 OpeningManager 调用：开始播放并淡入
    public void StartAmbience()
    {
        if (_audioSource.clip == null)
        {
            Debug.LogWarning("AmbienceManager: 没挂环境音剪辑！");
            return;
        }

        // 如果之前有正在进行的淡入淡出，先停掉它
        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);

        _audioSource.Play();
        _fadeCoroutine = StartCoroutine(FadeIn(fadeInDuration, maxVolume));
    }

    // --- 新增：供 EndingManager 调用，触发平滑淡出 ---
    public void StopAmbience()
    {
        if (_audioSource != null && _audioSource.isPlaying)
        {
            // 如果之前有正在进行的淡入淡出，先停掉它
            if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);

            _fadeCoroutine = StartCoroutine(FadeOut(fadeOutDuration));
        }
    }

    // 内部协程：处理音量从 0 到 maxVolume 的平滑过渡
    private IEnumerator FadeIn(float duration, float targetVol)
    {
        float timer = 0f;
        float startVol = _audioSource.volume; // 从当前音量开始过渡，更安全

        while (timer < duration)
        {
            timer += Time.deltaTime;
            _audioSource.volume = Mathf.Lerp(startVol, targetVol, timer / duration);
            yield return null;
        }
        _audioSource.volume = targetVol;
        _fadeCoroutine = null;
    }

    // --- 新增：内部协程，处理音量平滑淡出直至关闭 ---
    private IEnumerator FadeOut(float duration)
    {
        float timer = 0f;
        float startVol = _audioSource.volume;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            _audioSource.volume = Mathf.Lerp(startVol, 0f, timer / duration);
            yield return null;
        }

        _audioSource.volume = 0f;
        _audioSource.Stop();
        _fadeCoroutine = null;
    }
}