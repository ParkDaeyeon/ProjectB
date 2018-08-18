using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

class UnityNoticePopup : EditorWindow
{
    public static UnityNoticePopup OpenWindow(string name = "notice", string message = "Message", string[] buttons = null, Action<int> callback = null)
    {
        UnityNoticePopup pop = GetWindow<UnityNoticePopup>(name, true);
        pop.message = message;
        if (null != buttons && 0 < buttons.Length)
            pop.buttons = buttons;
        pop.callback = callback;
        return pop;
    }

    void OnLostFocus()
    {
        Focus();
    }

    Action<int> callback = null;
    public string message = "Message";
    public string[] buttons = new string[]{"OK", "Cancel"};
    public int choice = 0;
    public bool isDone = false;
    void OnGUI()
    {
        GUILayout.Label(this.message);
        EditorGUILayout.BeginHorizontal();
        for (int n = 0; n < this.buttons.Length; ++n)
        {
            if (GUILayout.Button(this.buttons[n]))
            {
                this.choice = n;
                this.isDone = true;
                break;
            }
        }
        EditorGUILayout.EndHorizontal();
        if (this.isDone)
        {
            if (null != this.callback)
                this.callback(this.choice);

            this.Close();
            return;
        }
    }
}
