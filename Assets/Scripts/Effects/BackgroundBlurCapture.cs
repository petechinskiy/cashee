using UnityEngine;

/// <summary>
/// Built-in RP background blur capture.
/// Attach to the main camera. Produces a blurred copy of the camera image into a global texture.
/// UI (Screen Space Overlay) can sample it in the same frame; SpriteRenderer may see it with 1-frame latency.
/// </summary>
[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public sealed class BackgroundBlurCapture : MonoBehaviour
{
    [Header("Performance / Quality")]
    [Tooltip("Downscale factor for the blur buffer. 2 = half res, 4 = quarter res.")]
    [Range(1, 8)] public int downsample = 2;

    [Tooltip("Blur iterations. 1-3 is usually enough for UI background blur.")]
    [Range(0, 8)] public int iterations = 2;

    [Tooltip("Base pixel offset (in downsampled pixels). Larger = blurrier.")]
    [Range(0f, 4f)] public float radius = 1.25f;

    [Header("Advanced")]
    [Tooltip("If enabled, uses bilinear filtering on the blur RTs (recommended).")]
    public bool bilinear = true;

    [Tooltip("Global shader texture name to expose blurred frame.")]
    public string globalTextureName = "_BackgroundBlurTex";

    [Tooltip("Global shader texel size name. (x=1/w, y=1/h, z=w, w=h)")]
    public string globalTexelSizeName = "_BackgroundBlurTex_TexelSize";

    [Header("Debug")]
    [Tooltip("If enabled, the camera will output the blurred texture to the screen (for debugging).")]
    public bool debugOutputBlurToScreen = false;

    Material _blurMat;
    int _globalTexId;
    int _globalTexelSizeId;
    RenderTexture _persistentBlur;
    int _lastW;
    int _lastH;

    void OnEnable()
    {
        _globalTexId = Shader.PropertyToID(globalTextureName);
        _globalTexelSizeId = Shader.PropertyToID(globalTexelSizeName);

        var shader = Shader.Find("Hidden/BackgroundBlur/Kawase");
        if (!shader)
        {
            Debug.LogError("BackgroundBlurCapture: shader 'Hidden/BackgroundBlur/Kawase' not found.");
            enabled = false;
            return;
        }

        _blurMat = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
    }

    void OnDisable()
    {
        if (_blurMat) DestroyImmediate(_blurMat);
        if (_persistentBlur)
        {
            _persistentBlur.Release();
            DestroyImmediate(_persistentBlur);
            _persistentBlur = null;
        }
        Shader.SetGlobalTexture(_globalTexId, null);
    }

    void OnValidate()
    {
        downsample = Mathf.Clamp(downsample, 1, 8);
        iterations = Mathf.Clamp(iterations, 0, 8);
        radius = Mathf.Max(0f, radius);
        if (!string.IsNullOrWhiteSpace(globalTextureName))
            _globalTexId = Shader.PropertyToID(globalTextureName);
        if (!string.IsNullOrWhiteSpace(globalTexelSizeName))
            _globalTexelSizeId = Shader.PropertyToID(globalTexelSizeName);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        int w = Mathf.Max(1, source.width / Mathf.Max(1, downsample));
        int h = Mathf.Max(1, source.height / Mathf.Max(1, downsample));

        EnsurePersistentRT(w, h, source);

        var filter = bilinear ? FilterMode.Bilinear : FilterMode.Point;

        // Always publish at least a valid copy of the camera image (diagnostics + safe fallback).
        Graphics.Blit(source, _persistentBlur);

        // Use a widely-supported format to avoid platform/Unity version edge cases with source.format.
        const RenderTextureFormat BlurFormat = RenderTextureFormat.ARGB32;
        RenderTexture rt1 = RenderTexture.GetTemporary(w, h, 0, BlurFormat);
        RenderTexture rt2 = RenderTexture.GetTemporary(w, h, 0, BlurFormat);
        rt1.filterMode = filter;
        rt2.filterMode = filter;

        // Downsample from source into rt1.
        Graphics.Blit(source, rt1);

        if (_blurMat && iterations > 0 && radius > 0.0001f)
        {
            // Kawase iterations (ping-pong).
            for (int i = 0; i < iterations; i++)
            {
                float offset = radius + i; // increasing offset per iteration improves blur spread cheaply
                _blurMat.SetVector("_Offset", new Vector4(offset / w, offset / h, 0f, 0f));
                Graphics.Blit(rt1, rt2, _blurMat, 0);

                // swap
                (rt1, rt2) = (rt2, rt1);
            }

            // Overwrite persistent RT with blurred result so consumers see blur (and debug shows it).
            Graphics.Blit(rt1, _persistentBlur);
        }

        Shader.SetGlobalTexture(_globalTexId, _persistentBlur);
        Shader.SetGlobalVector(_globalTexelSizeId, new Vector4(1f / w, 1f / h, w, h));

        // Output
        if (debugOutputBlurToScreen)
            Graphics.Blit(_persistentBlur ? _persistentBlur : source, destination);
        else
            Graphics.Blit(source, destination);

        RenderTexture.ReleaseTemporary(rt2);
        RenderTexture.ReleaseTemporary(rt1);
    }

    void EnsurePersistentRT(int w, int h, RenderTexture source)
    {
        // Keep persistent RT in a safe ubiquitous format.
        const RenderTextureFormat BlurFormat = RenderTextureFormat.ARGB32;
        if (_persistentBlur && (_lastW != w || _lastH != h))
        {
            _persistentBlur.Release();
            DestroyImmediate(_persistentBlur);
            _persistentBlur = null;
        }

        if (!_persistentBlur)
        {
            _persistentBlur = new RenderTexture(w, h, 0, BlurFormat)
            {
                name = "BackgroundBlurCapture_Persistent",
                hideFlags = HideFlags.HideAndDontSave,
                filterMode = bilinear ? FilterMode.Bilinear : FilterMode.Point,
                wrapMode = TextureWrapMode.Clamp,
                useMipMap = false,
                autoGenerateMips = false
            };
            _persistentBlur.Create();
            _lastW = w;
            _lastH = h;
        }

        // Keep filter mode in sync with inspector.
        _persistentBlur.filterMode = bilinear ? FilterMode.Bilinear : FilterMode.Point;
    }
}


