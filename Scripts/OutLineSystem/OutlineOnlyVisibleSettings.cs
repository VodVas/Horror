using UnityEngine;

[CreateAssetMenu(fileName = "OutlineSettingsNew", menuName = "Effects/Outline SettingsNew")]
public class OutlineOnlyVisibleSettings : ScriptableObject
{
    [SerializeField] private Color _outlineColor = Color.yellow;
    [SerializeField] private float _outlineWidth = 0.03f;
    [SerializeField] private string _colorPropertyName = "_OutlineColor";
    [SerializeField] private string _widthPropertyName = "_OutlineWidth";

    public Color OutlineColor => _outlineColor;
    public float OutlineWidth => _outlineWidth;

    private static readonly int ColorPropertyID = Shader.PropertyToID("_OutlineColor");
    private static readonly int WidthPropertyID = Shader.PropertyToID("_OutlineWidth");

    public void ApplyToMaterial(Material material)
    {
        if (material == null) return;

        material.SetColor(ColorPropertyID, _outlineColor);
        material.SetFloat(WidthPropertyID, _outlineWidth);
    }

    private void OnValidate()
    {
        _outlineWidth = Mathf.Clamp(_outlineWidth, 0f, 0.1f);
    }
}
