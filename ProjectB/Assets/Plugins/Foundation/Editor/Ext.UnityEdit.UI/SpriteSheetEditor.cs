using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using Ext.Unity3D.UI;
namespace Ext.UnityEdit.UI
{
    // NOTE: SpriteSheetEditor Ver.1
    public class SpriteSheetEditor : EditorWindow
    {
        SpriteSheet editedSheet = null;
        bool isReloaded = false;

        List<Sprite> selectedSprites = new List<Sprite>(1024);
        List<Sprite> duplicatedSprites = new List<Sprite>(1024);
        List<Sprite> availableSprites = new List<Sprite>(1024);
        List<Sprite> sortedSprites = new List<Sprite>(1024);
        Vector2 scroll = Vector2.zero;

        [MenuItem("Window/Sprite Sheet")]
        public static void CreateWindow()
        {
            EditorWindow.GetWindow<SpriteSheetEditor>("Sprite sheet", true);
        }

        void AddSpriteSelection(Dictionary<string, Sprite> spriteMap, Sprite s)
        {
            if (this.selectedSprites.Contains(s))
                return;

            this.selectedSprites.Add(s);

            if (null != spriteMap)
            {
                if (spriteMap.ContainsKey(s.name))
                    this.duplicatedSprites.Add(s);
                else
                    this.availableSprites.Add(s);
            }
            else
            {
                this.availableSprites.Add(s);
            }
        }

        void CheckSelectedSprites(SpriteSheet sheet)
        {
            Dictionary<string, Sprite> spriteMap = sheet ? sheet.SpriteMap : null;

            this.selectedSprites.Clear();
            this.duplicatedSprites.Clear();
            this.availableSprites.Clear();

            UnityEngine.Object[] objects = Selection.objects;
            if (null != objects)
            {
                foreach (UnityEngine.Object o in objects)
                {
                    if (!o)
                        continue;

                    if (o is Sprite)
                    {
                        this.AddSpriteSelection(spriteMap, (Sprite)o);
                    }
                    else if (o is Texture2D)
                    {
                        string path = AssetDatabase.GetAssetPath(o);
                        Sprite s = AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;
                        if (s)
                        {
                            this.AddSpriteSelection(spriteMap, s);
                        }
                    }
                }
            }
        }

        void PushAvailableSprites(SpriteSheet sheet)
        {
            if (!sheet)
                return;

            List<Sprite> sprites = sheet.Sprites;
            Dictionary<string, Sprite> spriteMap = sheet.SpriteMap;

            foreach (Sprite s in this.availableSprites)
            {
                if (!s)
                    continue;

                if (!spriteMap.ContainsKey(s.name))
                    sprites.Add(s);
            }

            this.Save(sheet);
        }

        void OnEnable()
        {
            this.isReloaded = true;
        }

        UnityNoticePopup popup = null;
        void OnGUI()
        {
            try
            {
                this.TryGUI();
            }
            catch (Exception e)
            {
                Debug.LogWarning("SPRITE_SHEET_EDITOR_ON_GUI_EXCEPT:" + e);
                this.isReloaded = true;
            }
        }

        void TryGUI()
        {
            SpriteSheet newSheet = (SpriteSheet)EditorGUILayout.ObjectField("Sheet", this.editedSheet, typeof(SpriteSheet), false);
            if (this.editedSheet != newSheet || this.isReloaded)
            {
                this.isReloaded = false;
                this.editedSheet = newSheet;
                if (this.editedSheet)
                {
                    this.editedSheet.ResetMap(false);
                    List<int> indexes = this.editedSheet.GetMissingSpriteIndexes();
                    if (null != indexes)
                    {
                        this.popup = UnityNoticePopup.OpenWindow(string.Format("Found missing sprites!:{0}", indexes.Count), "Remove the missing sprites?\nThen it is automatically saved.", new string[] { "Yes", "No" }, (choice) =>
                        {
                            if (0 == choice)
                            {
                                this.Save(this.editedSheet);
                            }
                            else
                            {
                                this.editedSheet = null;
                            }

                            this.popup = null;
                        });
                        return;
                    }
                }
            }

            if (this.popup)
                return;

            this.CheckSelectedSprites(this.editedSheet);

            Color colorOrigin = GUI.color;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(string.Format("Selected Sprites: {0} Item", this.selectedSprites.Count));
            GUI.color = 0 < this.duplicatedSprites.Count ? Color.red : colorOrigin;
            EditorGUILayout.LabelField(string.Format("Duplicated: {0} Item", this.duplicatedSprites.Count));
            GUI.color = colorOrigin;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            bool enableOld = GUI.enabled;
            GUI.enabled = 0 < this.availableSprites.Count;
            GUI.color = !GUI.enabled ? colorOrigin : this.editedSheet ? Color.green : Color.cyan;
            EditorGUILayout.LabelField(string.Format("Available Sprites: {0} Item", this.availableSprites.Count));
            if (this.editedSheet)
            {
                if (GUILayout.Button("Add Sprites"))
                {
                    this.PushAvailableSprites(this.editedSheet);
                }
            }
            else
            {
                if (GUILayout.Button("Create Sprite Sheet"))
                {
                    string path = EditorUtility.SaveFilePanelInProject("Create Sprite Sheet", "sheet.prefab", "prefab", "Please enter a file name to save the sprite sheet to");
                    if (!string.IsNullOrEmpty(path))
                    {
                        this.editedSheet = ScriptableObject.CreateInstance(typeof(SpriteSheet)) as SpriteSheet;
                        this.PushAvailableSprites(this.editedSheet);

                        AssetDatabase.CreateAsset(this.editedSheet, AssetDatabase.GenerateUniqueAssetPath(path));
                        AssetDatabase.Refresh();
                        this.Repaint();

                        Selection.activeObject = this.editedSheet;
                        return;
                    }
                }
            }
            GUI.color = colorOrigin;
            GUI.enabled = enableOld;
            EditorGUILayout.EndHorizontal();


            if (!this.editedSheet)
            {
                EditorGUILayout.LabelField("Sprite Sheet is empty.");
                return;
            }

            EditorGUILayout.LabelField(string.Format("Contain Sprites: {0} Item", this.editedSheet.Sprites.Count));

            this.scroll = EditorGUILayout.BeginScrollView(this.scroll);

            Sprite lookAt = null;
            bool isRemoved = false;
            this.sortedSprites.Clear();
            foreach (Sprite s in this.editedSheet.Sprites)
                this.sortedSprites.Add(s);
            this.sortedSprites.Sort((l, r) => { return l.name.CompareTo(r.name); });
            for (int n = 0; n < this.sortedSprites.Count; ++n)
            {
                Sprite s = this.sortedSprites[n];
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(s.name);
                if (GUILayout.Button("Look At"))
                {
                    lookAt = s;
                }
                if (GUILayout.Button("Remove"))
                {
                    this.editedSheet.Sprites.Remove(s);
                    isRemoved = true;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            if (null != lookAt)
            {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = lookAt;
            }

            if (isRemoved)
                this.Save(this.editedSheet);
        }



        void Save(SpriteSheet sheet, bool isRequireUpdate = true)
        {
            if (!sheet)
                return;

            try
            {
                if (isRequireUpdate)
                    sheet.ForceUpdateAll();

                string path = AssetDatabase.GetAssetPath(sheet);
                if (string.IsNullOrEmpty(path))
                    return;

                EditorUtility.SetDirty(sheet);
                AssetDatabase.StartAssetEditing();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogWarning(string.Format("SPRITE_SHEET_EDITOR_SAVE_EXCEPTION:{0}", e));
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }
    }
}