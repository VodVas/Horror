using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public sealed class OutlineController : MonoBehaviour
{
    private static readonly HashSet<OutlineController> _activeOutlines = new HashSet<OutlineController>();
    private static readonly int _outlineEnabledProperty = Shader.PropertyToID("_OutlineEnabled");

    private Renderer _renderer;
    private MaterialPropertyBlock _propertyBlock;
    private bool _isOutlineActive;

    public static IReadOnlyCollection<OutlineController> ActiveOutlines => _activeOutlines;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _propertyBlock = new MaterialPropertyBlock();
    }

    private void Start()
    {
        SetOutlineActive(true);
    }

    private void OnEnable()
    {
        if (_isOutlineActive)
        {
            _activeOutlines.Add(this);
        }
    }

    private void OnDisable()
    {
        _activeOutlines.Remove(this);
    }

    private void OnDestroy()
    {
        _activeOutlines.Remove(this);
    }

    [ContextMenu("CustomStart")]
    public void CustomStart()
    {
        SetOutlineActive(true);
    }

    public void SetOutlineActive(bool active)
    {
        if (_isOutlineActive == active) return;

        _isOutlineActive = active;

        if (active && enabled && gameObject.activeInHierarchy)
        {
            _activeOutlines.Add(this);
        }
        else
        {
            _activeOutlines.Remove(this);
        }

        _renderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetFloat(_outlineEnabledProperty, active ? 1f : 0f);
        _renderer.SetPropertyBlock(_propertyBlock);
    }

    public Renderer GetRenderer() => _renderer;
}
