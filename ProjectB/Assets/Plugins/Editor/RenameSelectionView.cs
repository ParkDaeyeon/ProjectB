using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class RenameSelectionView : EditorWindow
{
    public static string addPrefix = "";
    public static string addSuffix = "";

    public static string replaceFrom = "";
    public static string replaceTo = "";

    //public static bool isResource = false;

    private static StringBuilder SB = new StringBuilder(1024);

    [MenuItem("Assets/Rename Selection")]
    static void CreateWindow()
    {
        GetWindow<RenameSelectionView>(true, "Rename Selection View").Show();
    }

    Vector2 mScroll;
    void OnGUI()
    {
        addPrefix = EditorGUILayout.TextField("add prefix", addPrefix);
        addSuffix = EditorGUILayout.TextField("add suffix", addSuffix);

        EditorGUILayout.Separator();

        replaceFrom = EditorGUILayout.TextField("replace from", replaceFrom);
        replaceTo = EditorGUILayout.TextField("replace to", replaceTo);

        EditorGUILayout.Separator();

        //isResource = EditorGUILayout.Toggle("is resource", isResource);

        //EditorGUILayout.Separator();

        if (GUILayout.Button("Clear"))
        {
            addPrefix = addSuffix = replaceFrom = replaceTo = "";
            //isResource = false;
        }

        EditorGUILayout.Separator();

        if (false == IsEmpty)
        {
            if (GUILayout.Button("Apply Selection"))
            {
                ApplySelection();
            }
        }
    }

    public static void ApplySelection()
    {
        if (IsEmpty)
            return;

        Object[] selection = Selection.objects;
        for (int n = 0; n < selection.Length; ++n)
        {
            Object obj = selection[n];

            string from = obj.name;
            string to = Apply(from);
            EditorUtility.DisplayProgressBar("Rename Selection", "from = " + from + ", to = " + to, (float)n / selection.Length);

            try
            {
                string path = AssetDatabase.GetAssetPath(obj);
                if (false == string.IsNullOrEmpty(path))
                {
                    AssetDatabase.RenameAsset(path, to);
                    AssetDatabase.ImportAsset(path);
                }
                else
                    obj.name = to;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e);
            }
        }

        EditorUtility.ClearProgressBar();
    }

    public static string Apply(string s)
    {
        SB.Length = 0;

        if (false == string.IsNullOrEmpty(addPrefix))
            SB.Append(addPrefix);

        if (false == string.IsNullOrEmpty(replaceFrom))
            SB.Append(s.Replace(replaceFrom, replaceTo));
        else
            SB.Append(s);

        if (false == string.IsNullOrEmpty(addSuffix))
            SB.Append(addSuffix);

        return SB.ToString();
    }

    public static bool IsEmpty
    {
        get
        {
            return string.IsNullOrEmpty(addPrefix) && string.IsNullOrEmpty(replaceFrom) && string.IsNullOrEmpty(addSuffix);
        }
    }
}
