#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WaypointMover))]
public class WaypointMoverEditor : Editor
{
    private SerializedProperty _waypointsProperty;
    private SerializedProperty _patrolModeProperty;
    private SerializedProperty _moveSpeedProperty;
    private SerializedProperty _rotationSpeedProperty;

    private void OnEnable()
    {
        _waypointsProperty = serializedObject.FindProperty("_waypoints");
        _patrolModeProperty = serializedObject.FindProperty("_patrolMode");
        _moveSpeedProperty = serializedObject.FindProperty("_moveSpeed");
        _rotationSpeedProperty = serializedObject.FindProperty("_rotationSpeed");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        EditorGUILayout.Space();

        if (Application.isPlaying)
        {
            WaypointMover mover = (WaypointMover)target;

            EditorGUILayout.LabelField("Runtime Info", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Current Waypoint: {mover.CurrentWaypointIndex}");
            EditorGUILayout.LabelField($"Progress: {mover.ProgressToNextWaypoint:P0}");
            EditorGUILayout.LabelField($"Is Moving: {mover.IsMoving}");
            EditorGUILayout.LabelField($"Distance Traveled: {mover.TotalDistanceTraveled:F2}m");

            EditorGUILayout.Space();

            if (GUILayout.Button("Reset To Start"))
            {
                mover.ResetToStart();
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif