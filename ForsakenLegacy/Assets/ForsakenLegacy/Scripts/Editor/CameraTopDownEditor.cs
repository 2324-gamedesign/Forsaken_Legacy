using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraTopDown))]
public class CameraTopDownEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CameraTopDown cameraTopDown = (CameraTopDown)target;

        if (GUILayout.Button("Recalculate Angle"))
        {
            cameraTopDown.CalculateAngle();
        }
    }
}