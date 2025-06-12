using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class OutlineOnlyVisibleController : MonoBehaviour
{
    [SerializeField] private OutlineOnlyVisibleSettings _outlineSettings;
    [SerializeField] private bool _outlineEnabled = true;

    private Renderer _renderer;
    private Material _outlineMaterial;
    private Material[] _originalMaterials;
    private Material[] _materialsWithOutline;

    private static readonly string OUTLINE_SHADER_NAME = "Custom/OutlineURP";

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        InitializeMaterials();
    }

    private void InitializeMaterials()
    {
        var outlineShader = Shader.Find(OUTLINE_SHADER_NAME);
        if (outlineShader == null)
        {
            Debug.LogError($"Outline shader '{OUTLINE_SHADER_NAME}' not found!");
            enabled = false;
            return;
        }

        _originalMaterials = _renderer.sharedMaterials;
        _outlineMaterial = new Material(outlineShader);

        CreateMaterialsWithOutline();
        UpdateOutlineSettings();
    }

    private void CreateMaterialsWithOutline()
    {
        _materialsWithOutline = new Material[_originalMaterials.Length + 1];
        _materialsWithOutline[0] = _outlineMaterial;

        for (int i = 0; i < _originalMaterials.Length; i++)
        {
            _materialsWithOutline[i + 1] = _originalMaterials[i];
        }
    }

    private void OnEnable()
    {
        if (_outlineEnabled && _materialsWithOutline != null)
        {
            _renderer.materials = _materialsWithOutline;
        }
    }

    private void OnDisable()
    {
        if (_originalMaterials != null)
        {
            _renderer.materials = _originalMaterials;
        }
    }

    private void OnDestroy()
    {
        if (_outlineMaterial != null)
        {
            Destroy(_outlineMaterial);
        }
    }

    public void SetOutlineEnabled(bool enabled)
    {
        _outlineEnabled = enabled;
        _renderer.materials = enabled ? _materialsWithOutline : _originalMaterials;
    }

    public void UpdateOutlineSettings()
    {
        if (_outlineSettings != null && _outlineMaterial != null)
        {
            _outlineSettings.ApplyToMaterial(_outlineMaterial);
        }
    }

    private void OnValidate()
    {
        if (Application.isPlaying && _renderer != null)
        {
            UpdateOutlineSettings();
            SetOutlineEnabled(_outlineEnabled);
        }
    }
}
