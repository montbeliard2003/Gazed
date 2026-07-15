using UnityEngine;

public class ScanHighlighter : MonoBehaviour
{
    public Material scanMaterial;

    private Renderer[] _renderers;
    private Material[][] _originalMats;

    private MaterialPropertyBlock _mpb;

    private bool _scanning;

    void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>();
        _originalMats = new Material[_renderers.Length][];
        for (int i = 0; i < _renderers.Length; i++)
        {
            _originalMats[i] = _renderers[i].sharedMaterials;
        }

        _mpb = new MaterialPropertyBlock();
    }

    public void BeginScan()
    {
        if (_scanning) return;
        if (scanMaterial == null) return;

        _scanning = true;

        // 换成扫描材质（单材质版本最简单稳定）
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].sharedMaterial = scanMaterial;
        }
    }

    public void SetScanProgress(float progress01)
    {
        if (!_scanning) return;

        progress01 = Mathf.Clamp01(progress01);

        for (int i = 0; i < _renderers.Length; i++)
        {
            var r = _renderers[i];

            // 传 bounds Y 范围（世界坐标）
            var b = r.bounds;

            r.GetPropertyBlock(_mpb);
            _mpb.SetFloat("_ScanProgress", progress01);
            _mpb.SetFloat("_BoundsMinY", b.min.y);
            _mpb.SetFloat("_BoundsMaxY", b.max.y);
            r.SetPropertyBlock(_mpb);
        }
    }

    public void EndScan()
    {
        if (!_scanning) return;

        _scanning = false;

        // 恢复原材质
        for (int i = 0; i < _renderers.Length; i++)
        {
            _renderers[i].sharedMaterials = _originalMats[i];
            _renderers[i].SetPropertyBlock(null);
        }
    }
}