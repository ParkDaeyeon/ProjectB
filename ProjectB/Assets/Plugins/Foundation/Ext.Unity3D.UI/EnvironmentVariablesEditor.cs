using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Ext.IO;

namespace Ext.Unity3D.UI
{
    public class EnvironmentVariablesEditor : ManagedUIComponent, IEnumerable<EnvironmentVariablesEditor.Element>
    {
        public static EnvironmentVariablesEditor Open(string group,
                                                      Rect rect,
                                                      Vector3 scale,
                                                      Action confirmCallback)
        {
            EnvironmentVariablesEditor editor = GameObject.FindObjectOfType<EnvironmentVariablesEditor>();

            if (!editor)
            {
                GameObject go = new GameObject("EnvironmentVariablesEditor");
                editor = go.AddComponent<EnvironmentVariablesEditor>();
                editor.group = group;
                editor.rect = rect;
                editor.mat = Matrix4x4.Scale(scale);
                
                editor.confirmCallback = confirmCallback;
                GameObject.DontDestroyOnLoad(go);
            }
            else
            {
                Debug.LogWarning("EnvironmentVariablesEditor already exsist");
            }

            return editor;
        }

        public class Element : IDisposable
        {
            Element() { }

            public static Element Load(EnvironmentVariablesEditor editor, string key, string defaultValue = "")
            {
                var elem = new Element();
                elem.editor = editor;
                elem.key = key;
                elem.Value = Preference.GetString(key, defaultValue);
                return elem;
            }

            public static Element New(EnvironmentVariablesEditor editor, string key, string value)
            {
                var elem = new Element();
                elem.editor = editor;
                elem.key = key;
                elem.value = value;
                elem.Save();
                return elem;
            }

            public void Dispose()
            {
                this.editor = null;
            }


            EnvironmentVariablesEditor editor;

            string key;
            public string Key
            {
                set { this.key = value; }
                get { return this.key; }
            }
            public string KeyPref { get { return string.Format("{0}.{1}", editor.Group, this.Key); } }

            string value;
            public string Value
            {
                set { this.value = value; }
                get { return this.value; }
            }

            public void Save()
            {
                Preference.SetString(this.KeyPref, this.Value);
                this.editor.RegistAll();
                Preference.Save();
            }

            public void Discard()
            {
                Preference.DeleteKey(this.Key);
                this.editor.RegistAll();
                Preference.Save();
            }
        }

        List<Element> list = new List<Element>();
        
        Element GetElement(int index)
        {
            if (0 > index || index >= this.Count)
                return null;

            return this.list[index];
        }

        public int FindIndex(string key)
        {
            for (int n = 0, cnt = this.Count; n < cnt; ++n)
                if (this.GetElement(n).Key == key)
                    return n;

            return -1;
        }

        public bool Contains(string key)
        {
            return -1 != this.FindIndex(key);
        }

        public string GetValue(string key)
        {
            var index = this.FindIndex(key);
            if (-1 == index)
                return null;

            return this.GetElement(index).Value;
        }

        public bool Load(string key, string defaultValue = "", int index = int.MaxValue)
        {
            if (this.Contains(key))
                return false;

            if (0 > index)
                index = 0;
            else if (index > this.Count)
                index = this.Count;

            this.list.Insert(index, Element.Load(this, key, defaultValue));
            return true;
        }
        
        public bool New(string key, string value, int index = int.MaxValue)
        {
            if (this.Contains(key))
                return false;

            if (0 > index)
                index = 0;
            else if (index > this.Count)
                index = this.Count;

            this.list.Insert(index, Element.New(this, key, value));
            return true;
        }

        public bool AssignAt(int index, string key, string value)
        {
            var elem = this.GetElement(index);
            if (null == elem)
                return false;

            if (this.Contains(key))
                return false;

            if (key != elem.Key)
                elem.Discard();

            elem.Key = key;
            elem.Value = value;
            elem.Save();
            return true;
        }

        public bool RemoveAt(int index)
        {
            if (0 > index || index >= this.Count)
                return false;

            var elem = this.GetElement(index);
            this.list.RemoveAt(index);
            elem.Discard();
            elem.Dispose();
            return true;
        }

        public bool Remove(string key)
        {
            var index = this.FindIndex(key);
            if (-1 == index)
                return false;
            
            return this.RemoveAt(index);
        }

        public void RemoveAll()
        {
            for (int n = 0, cnt = this.Count; n < cnt; ++n)
                this.GetElement(n).Discard();

            this.Clear();
        }

        public void Clear()
        {
            for (int n = 0, cnt = this.Count; n < cnt; ++n)
                this.GetElement(n).Dispose();

            this.list.Clear();
        }

        public int Count { get { return list.Count; } }
        
        public IEnumerator<Element> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        string registKey { get { return string.Format("{0}_list", this.group); } }

        void RegistAll()
        {
            var sb = new StringBuilder(1024);
            for (int n = 0, cnt = this.Count; n < cnt; ++n)
                sb.Append('\"').Append(this.GetElement(n).Key).Append('\"').Append(',');
            Preference.SetString(this.registKey, sb.ToString());
        }

        public void LoadAll()
        {
            var str = Preference.GetString(this.registKey, "");
            var begin = 0;
            while (begin < str.Length)
            {
                var after = str.IndexOf("\",", begin);
                if (-1 != after)
                {
                    var key = str.Substring(begin + 1, after - (begin + 1));
                    this.Load(key);

                    begin = after + 2;
                }
                else
                    break;
            }
        }

        Action confirmCallback;
        public Action ConfirmCallback { set { this.confirmCallback = value; } get { return this.confirmCallback; } }

        string group = "envedit";
        public string Group { get { return this.group; } }
        Rect rect = new Rect(0, 0, 100, 100);
        Matrix4x4 mat = Matrix4x4.identity;
        Vector2 scrollPosition = Vector2.zero;
        LinkedList<int> removedBuffer = new LinkedList<int>();
        string addKey = "new";


        Action overrideOnGUI;
        public Action OverrideOnGUI
        {
            set { this.overrideOnGUI = value; }
            get { return this.overrideOnGUI; }
        }


        void OnGUI()
        {
            GUI.matrix = this.mat;

            GUILayout.BeginArea(this.rect);
            GUILayout.BeginVertical();
            this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, (GUIStyle)"Window");
            

            var padd = GUI.skin.box.padding;
            var margin = GUI.skin.box.margin;
            var w = this.rect.width - padd.horizontal - margin.horizontal;
            //var h = this.rect.height - padd.vertical - margin.vertical;


            this.removedBuffer.Clear();
            for (int n = 0, cnt = this.Count; n < cnt; ++n)
            {
                GUILayout.BeginHorizontal();
                var elem = this.GetElement(n);
                var newKey = GUILayout.TextField(elem.Key, GUILayout.Width(w * 0.25f));
                if (newKey != elem.Key)
                {
                    if (!this.Contains(newKey))
                    {
                        elem.Discard();
                        elem.Key = newKey;
                        elem.Save();
                    }
                    else
                    {
                        Debug.LogWarning(string.Format("{0} is already exists!", newKey));
                    }
                }

                var newValue = GUILayout.TextField(elem.Value, GUILayout.Width(w * 0.65f));
                if (newValue != elem.Value)
                {
                    elem.Value = newValue;
                    elem.Save();
                }
                
                if (GUILayout.Button("Remove"))
                {
                    this.removedBuffer.AddFirst(n);
                }
                GUILayout.EndHorizontal();
            }

            for (int n = 0, cnt = this.removedBuffer.Count; n < cnt; ++n)
            {
                var atIndex = this.removedBuffer.First.Value;
                this.removedBuffer.RemoveFirst();

                this.RemoveAt(atIndex);
            }


            GUILayout.BeginHorizontal();
            this.addKey = GUILayout.TextField(this.addKey, GUILayout.Width(w * 0.25f));

            if (GUILayout.Button("Add"))
            {
                if (!this.Contains(this.addKey))
                {
                    this.New(this.addKey, "value");
                }
                else
                {
                    Debug.LogWarning(string.Format("{0} is already exists!", this.addKey));
                }
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Remove All"))
            {
                this.RemoveAll();
            }

            GUILayout.EndScrollView();

            if (null != this.overrideOnGUI)
                this.overrideOnGUI();

            if (GUILayout.Button("Confirm"))
            {
                if (null != this.confirmCallback)
                    this.confirmCallback();

                GameObject.Destroy(this.gameObject);
            }
            
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        void OnDistory()
        {
            this.Clear();
        }
    }
}
