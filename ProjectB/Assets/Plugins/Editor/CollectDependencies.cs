using UnityEngine;
using UnityEditor;

public class CollectDependencies : EditorWindow
{
    static GameObject obj = null;


    [MenuItem("Window/Collect Dependencies")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        CollectDependencies window = (CollectDependencies)EditorWindow.GetWindow(typeof(CollectDependencies));
        window.Show();
    }

    void OnGUI()
    {
        obj = EditorGUI.ObjectField(new Rect(3, 3, position.width - 6, 20), "Find Dependency", obj, typeof(GameObject), false) as GameObject;

        if (obj)
        {
            Object[] roots = new Object[] { obj };

            if (GUI.Button(new Rect(3, 25, position.width - 6, 20), "Check Dependencies"))
                Selection.objects = EditorUtility.CollectDependencies(roots);
        }
        else
            EditorGUI.LabelField(new Rect(3, 25, position.width - 6, 20), "Missing:", "Select an object first");
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }
}