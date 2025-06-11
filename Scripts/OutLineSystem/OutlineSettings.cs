using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "OutlineSettings", menuName = "Rendering/Outline Settings")]
public sealed class OutlineSettings : ScriptableObject
{
    [SerializeField, Range(0f, 10f)] private float _outlineWidth = 2f;
    [SerializeField, ColorUsage(true, true)] private Color _outlineColor = Color.black;
    [SerializeField] private LayerMask _outlineLayers = -1;
    [SerializeField] private bool _useSRPBatcher = true;
    [SerializeField] private bool _useGPUInstancing = true;
    [SerializeField, Range(0f, 1f)] private float _zOffset = 0.0001f;

    private Material _outlineMaterial;
    private readonly int _colorProperty = Shader.PropertyToID("_OutlineColor");
    private readonly int _widthProperty = Shader.PropertyToID("_OutlineWidth");
    private readonly int _zOffsetProperty = Shader.PropertyToID("_ZOffset");

    public float OutlineWidth => _outlineWidth;
    public Color OutlineColor => _outlineColor;
    public LayerMask OutlineLayers => _outlineLayers;
    public bool UseSRPBatcher => _useSRPBatcher;
    public bool UseGPUInstancing => _useGPUInstancing;
    public float ZOffset => _zOffset;

    public Material GetOutlineMaterial()
    {
        if (_outlineMaterial == null)
        {
            var shader = Shader.Find("Custom/URPOutline");
            if (shader == null)
            {
                Debug.LogError("Outline shader not found!");
                return null;
            }

            _outlineMaterial = CoreUtils.CreateEngineMaterial(shader);
            _outlineMaterial.enableInstancing = _useGPUInstancing;
        }

        UpdateMaterialProperties();
        return _outlineMaterial;
    }

    private void UpdateMaterialProperties()
    {
        if (_outlineMaterial == null) return;

        _outlineMaterial.SetColor(_colorProperty, _outlineColor);
        _outlineMaterial.SetFloat(_widthProperty, _outlineWidth * 0.001f);
        _outlineMaterial.SetFloat(_zOffsetProperty, _zOffset);
    }

    private void OnValidate()
    {
        UpdateMaterialProperties();
    }

    private void OnDestroy()
    {
        if (_outlineMaterial != null)
        {
            CoreUtils.Destroy(_outlineMaterial);
            _outlineMaterial = null;
        }
    }
}