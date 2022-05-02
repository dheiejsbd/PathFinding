using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NavObstacle))]
public class NavObstacleEditor : Editor
{
    SerializedProperty shape;
    SerializedProperty nodeCount;
    SerializedProperty center;
    SerializedProperty size;

    private void Awake()
    {
        shape = serializedObject.FindProperty("shape");
        nodeCount = serializedObject.FindProperty("nodeCount");
        center = serializedObject.FindProperty("center");
        size = serializedObject.FindProperty("size");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(shape);

        if (Shape.GeoPoly == (Shape)shape.enumValueIndex)
        {
            EditorGUILayout.PropertyField(nodeCount);
        }

        EditorGUILayout.PropertyField(center);
        EditorGUILayout.PropertyField(size);

        serializedObject.ApplyModifiedProperties();
    }
}
