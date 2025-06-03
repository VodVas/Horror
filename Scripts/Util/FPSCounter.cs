using UnityEngine;
using UnityEngine.UI;
using System.Runtime.CompilerServices;
using TMPro;


[DefaultExecutionOrder(-1000)]
public sealed class FPSCounter : MonoBehaviour
{
    [Header("Display Settings")]
    [SerializeField] private float _updateInterval = 0.5f;
    [SerializeField] private string _format = "FPS: {0}";
    [SerializeField] private Color _textColor = Color.white;

    [Header("Performance")]
    [SerializeField] private bool _vsyncIndependent = true;
    [SerializeField] private bool _autoDisableWhenHidden = true;

    [Header("UI References (Auto-populated if null)")]
    [SerializeField] private Text _legacyText;

    [SerializeField] private TextMeshProUGUI _tmpText;


    private Canvas _canvas;
    private int _frameCount;
    private float _timeAccumulator;
    private float _lastUpdateTime;
    private bool _isUsingTMP;
    private readonly char[] _charBuffer = new char[32];

    private const float MIN_UPDATE_INTERVAL = 0.1f;
    private const float MAX_UPDATE_INTERVAL = 2f;

    private void Awake()
    {
        ValidateUpdateInterval();
        SetupUI();
        CacheComponents();

        if (_vsyncIndependent)
        {
            Application.targetFrameRate = -1;
        }
    }

    private void OnEnable()
    {
        ResetCounters();
    }

    private void Update()
    {
        UpdateFPSCalculation();
    }

    private void LateUpdate()
    {
        if (_autoDisableWhenHidden && !IsVisible())
        {
            enabled = false;
            return;
        }
    }

    private void OnBecameVisible()
    {
        if (_autoDisableWhenHidden)
        {
            enabled = true;
            ResetCounters();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateFPSCalculation()
    {
        _frameCount++;
        _timeAccumulator += Time.unscaledDeltaTime;

        if (_timeAccumulator >= _updateInterval)
        {
            int fps = Mathf.RoundToInt(_frameCount / _timeAccumulator);
            UpdateDisplay(fps);
            ResetCounters();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void UpdateDisplay(int fps)
    {
        if (_isUsingTMP && _tmpText != null)
        {
            _tmpText.SetText(_format, fps);
        }
        else

        if (_legacyText != null)
        {
            int length = FormatNumber(fps, _charBuffer);
            _legacyText.text = BuildString(_charBuffer, length);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ResetCounters()
    {
        _frameCount = 0;
        _timeAccumulator = 0f;
        _lastUpdateTime = Time.unscaledTime;
    }

    private void SetupUI()
    {
        if (_tmpText == null)
        {
            _tmpText = GetComponent<TextMeshProUGUI>();
        }
        
        if (_tmpText != null)
        {
            _isUsingTMP = true;
            _tmpText.color = _textColor;
            return;
        }

        if (_legacyText == null)
        {
            _legacyText = GetComponent<Text>();
        }

        if (_legacyText == null)
        {
            CreateDefaultUI();
        }
        else
        {
            _legacyText.color = _textColor;
        }
    }

    private void CreateDefaultUI()
    {
        _canvas = FindObjectOfType<Canvas>();

        if (_canvas == null)
        {
            GameObject canvasGO = new GameObject("FPS Canvas");
            _canvas = canvasGO.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 1000;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        GameObject fpsGO = new GameObject("FPS Counter", typeof(RectTransform));
        fpsGO.transform.SetParent(_canvas.transform, false);

        RectTransform rect = fpsGO.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(10, -10);
        rect.sizeDelta = new Vector2(200, 50);

        if (Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF") != null)
        {
            _tmpText = fpsGO.AddComponent<TextMeshProUGUI>();
            _tmpText.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            _tmpText.fontSize = 24;
            _tmpText.color = _textColor;
            _isUsingTMP = true;
        }
        else
        {
            _legacyText = fpsGO.AddComponent<Text>();
            _legacyText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            _legacyText.fontSize = 24;
            _legacyText.color = _textColor;
        }

        transform.SetParent(fpsGO.transform, false);
    }

    private void CacheComponents()
    {
        if (_canvas == null)
        {
            _canvas = GetComponentInParent<Canvas>();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool IsVisible()
    {
        return _canvas != null && _canvas.enabled;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ValidateUpdateInterval()
    {
        _updateInterval = Mathf.Clamp(_updateInterval, MIN_UPDATE_INTERVAL, MAX_UPDATE_INTERVAL);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int FormatNumber(int value, char[] buffer)
    {
        int index = buffer.Length - 1;
        int length = 0;

        do
        {
            buffer[index--] = (char)('0' + value % 10);
            value /= 10;
            length++;
        }
        while (value > 0 && index >= 0);

        int start = index + 1;
        if (start > 0)
        {
            for (int i = 0; i < length; i++)
            {
                buffer[i] = buffer[start + i];
            }
        }

        return length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string BuildString(char[] buffer, int numberLength)
    {
        string prefix = "FPS: ";
        char[] result = new char[prefix.Length + numberLength];

        for (int i = 0; i < prefix.Length; i++)
        {
            result[i] = prefix[i];
        }

        for (int i = 0; i < numberLength; i++)
        {
            result[prefix.Length + i] = buffer[i];
        }

        return new string(result);
    }

    private void OnValidate()
    {
        ValidateUpdateInterval();

        if (Application.isPlaying)
        {
            if (_legacyText != null)
            {
                _legacyText.color = _textColor;
            }

            if (_tmpText != null)
            {
                _tmpText.color = _textColor;
            }
        }
    }

#if UNITY_EDITOR
    private void Reset()
    {
        _updateInterval = 0.5f;
        _format = "FPS: {0}";
        _textColor = Color.white;
        _vsyncIndependent = true;
        _autoDisableWhenHidden = true;
    }
#endif
}