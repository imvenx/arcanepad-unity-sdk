using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Arcane))]
public class ArcaneLibraryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw default inspector
        DrawDefaultInspector();

        // Disable editing
        GUI.enabled = false;

        // Manually draw read-only fields
        EditorGUILayout.TextField("Library Version", ((Arcane)target).LibraryVersion);
    }
}