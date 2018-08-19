using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

class UnityInputPopup : EditorWindow
{
    Action<string, bool> callback = null;
    public static UnityInputPopup OpenWindow(string name = "input", string label = "input", string defaultText = "", Action<string, bool> callback = null)
    {
        UnityInputPopup pop = GetWindow<UnityInputPopup>(name, true);
        pop.label = label;
        pop.text = defaultText;
        pop.callback = callback;
        return pop;
    }

    void OnLostFocus()
    {
        Focus();
    }

    public string label = "input";
    public string text = "";
    public bool isOk = false;
    public bool isDone = false;
    void OnGUI()
    {
        this.text = EditorGUILayout.TextField(this.label, this.text);

        EditorGUILayout.BeginHorizontal();
        bool clickOk = GUILayout.Button("OK");
        bool clickCancel = GUILayout.Button("CANCEL");
        EditorGUILayout.EndHorizontal();
        if (clickOk || clickCancel)
        {
            this.isOk = clickOk;
            this.isDone = true;
            if (null != this.callback)
                this.callback(this.text, this.isOk);

            this.Close();
            return;
        }
    }
}
