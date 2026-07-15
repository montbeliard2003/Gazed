using UnityEngine;

public class Highlightable : MonoBehaviour
{
    [Header("Highlight Settings")]
    public Color highlightColor = new Color(1f, 0.6f, 0f, 1f);
    [Range(0f, 1f)]
    public float intensity = 0.35f; // 变亮强度（越大越亮）

    private Renderer[] _renderers;
    private MaterialPropertyBlock _mpb;

    // 记录原色（按 renderer 记录）
    private Color[] _baseColors;

    void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        _mpb = new MaterialPropertyBlock();

        _baseColors = new Color[_renderers.Length];

        for (int i = 0; i < _renderers.Length; i++)
        {
            var mat = _renderers[i].sharedMaterial;
            if (mat != null && mat.HasProperty("_Color"))
                _baseColors[i] = mat.color;
            else
                _baseColors[i] = Color.white;
        }
    }

    public void SetHighlighted(bool on)
    {
        for (int i = 0; i < _renderers.Length; i++)
        {
            var r = _renderers[i];
            r.GetPropertyBlock(_mpb);

            // 基础颜色 -> 高亮颜色（混合）
            Color baseCol = _baseColors[i];
            Color target = on ? Color.Lerp(baseCol, highlightColor, intensity) : baseCol;

            // Built-in 标准材质一般用 _Color
            _mpb.SetColor("_Color", target);

            r.SetPropertyBlock(_mpb);
        }
    }
}