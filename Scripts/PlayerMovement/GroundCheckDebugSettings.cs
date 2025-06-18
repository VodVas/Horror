using UnityEngine;

[System.Serializable]
public sealed class GroundCheckDebugSettings
{
    public bool showDebugInfo = false;
    public bool showGizmos = true;
    public Color groundedColor = Color.green;
    public Color airborneColor = Color.red;
    public Color rayColor = Color.yellow;
    public Color hitPointColor = Color.blue;
}
