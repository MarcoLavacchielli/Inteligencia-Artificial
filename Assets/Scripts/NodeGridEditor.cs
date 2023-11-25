#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PhysicalNodeGrid))]
public class NodeGridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PhysicalNodeGrid grid = (PhysicalNodeGrid)target;

        if (GUILayout.Button("Generate Grid"))
        {
            grid.Generate();
        }

        if (GUILayout.Button("Clear Grid"))
        {
            grid.ClearGrid();
        }
    }
}
#endif