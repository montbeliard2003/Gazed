using UnityEngine;
using System.Collections.Generic; // 必须引用，用于支持 List

[RequireComponent(typeof(AudioSource))]
public class FlickeringLightsGroup : MonoBehaviour
{
    private AudioSource _audioSource;

    [Header("灯光组")]
    public List<Light> lightsToFlicker = new List<Light>(); // 在 Inspector 里把那两个灯拖进来

    [Header("灯光设置")]
    public float minIntensity = 0.1f;
    public float maxIntensity = 1.0f;
    public float flickerSpeed = 0.08f;

    [Header("音效设置")]
    public AudioClip electricBuzz;
    [Range(0, 1)] public float sfxThreshold = 0.4f;
    public float volumeRandomness = 0.2f;

    private float _lastTime;

    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.spatialize = true;
        _audioSource.spatialBlend = 1.0f;

        // 如果你忘了拖入灯光，脚本会自动尝试在子物体中寻找
        if (lightsToFlicker.Count == 0)
        {
            lightsToFlicker.AddRange(GetComponentsInChildren<Light>());
        }
    }

    void Update()
    {
        // --- 核心修改：如果进入了结局状态，强行关闭声音并拦截闪烁 ---
        if (GameState.IsEnding)
        {
            if (_audioSource.isPlaying)
            {
                _audioSource.Stop(); // 确保正在响的电流声立刻掐断
            }
            return; // 直接返回，不再执行下面的闪烁和声音播放逻辑
        }
        // --------------------------------------------------------

        if (Time.time - _lastTime > flickerSpeed)
        {
            // 统一计算一个随机强度，保证所有灯同步
            float newIntensity = Random.Range(minIntensity, maxIntensity);

            // --- 触发音效 ---
            if (newIntensity < sfxThreshold && !_audioSource.isPlaying && electricBuzz != null)
            {
                _audioSource.pitch = Random.Range(0.8f, 1.2f);
                _audioSource.volume = Random.Range(0.5f - volumeRandomness, 0.5f + volumeRandomness);
                _audioSource.PlayOneShot(electricBuzz);
            }

            // --- 同时更新所有灯光的强度 ---
            foreach (Light l in lightsToFlicker)
            {
                if (l != null)
                {
                    l.intensity = newIntensity;
                }
            }

            _lastTime = Time.time;
        }
    }
}