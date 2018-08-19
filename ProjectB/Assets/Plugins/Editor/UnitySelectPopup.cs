using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

class UnitySelectPopup : EditorWindow
{
    Action<int, bool> callback = null;
    public static UnitySelectPopup OpenWindow(string name = "select", string[] list = null, int defaultSelect = 0, Action<int, bool> callback = null)
    {
        UnitySelectPopup pop = GetWindow<UnitySelectPopup>(name, true);
        pop.list = list;
        if (null == pop.list)
            pop.list = new string[0];
        pop.select = defaultSelect;
        pop.callback = callback;
        return pop;
    }

    void OnLostFocus()
    {
        Focus();
    }

    public string[] list;
    public int select = 0;
    public bool isOk = false;
    public bool isDone = false;
    void OnGUI()
    {
        this.select = EditorGUILayout.Popup(this.select, this.list);

        EditorGUILayout.BeginHorizontal();
        bool clickOk = GUILayout.Button("OK");
        bool clickCancel = GUILayout.Button("CANCEL");
        EditorGUILayout.EndHorizontal();
        if (clickOk || clickCancel)
        {
            this.isOk = clickOk;
            this.isDone = true;
            if (null != this.callback)
                this.callback(this.select, this.isOk);

            this.Close();
            return;
        }
    }
}
