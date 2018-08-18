using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

public static class UnityNativeWin
{
    static bool isSetuped;
    public static bool IsSetuped { get { return UnityNativeWin.isSetuped; } }
    public static void Setup()
    {
        if (UnityNativeWin.isSetuped)
            return;

        UnityNativeWin.isSetuped = true;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        RuntimePlatform platform = Application.platform;
        if (RuntimePlatform.WindowsEditor == platform || RuntimePlatform.WindowsPlayer == platform)
        {
            var paths = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
            var pathCurDir = Environment.CurrentDirectory;
            var sepaDir = Path.DirectorySeparatorChar;
            var sepaPath = Path.PathSeparator;

            var sb = new StringBuilder(1024);
            sb.Append(pathCurDir).Append(sepaDir);
#if UNITY_EDITOR
            sb.Append("Assets").Append(sepaDir);
            sb.Append("Plugins").Append(sepaDir);
            sb.Append("x86_64").Append(sepaDir);
#elif UNITY_STANDALONE_WIN
            var pathProcess = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            sb.Append(pathProcess).Append("_Data").Append(sepaDir);
            sb.Append("Plugins").Append(sepaDir);
#endif// UNITY_*

            var pathDll = sb.ToString();
#if LOG_DEBUG
            Debug.Log(string.Format("UNITY_NATIVE_WIN:PATHS:{0}", paths));
            Debug.Log(string.Format("UNITY_NATIVE_WIN:CUR_DIR:{0}, DLL:{1}, SEPA_DIR:{2}, SEPA_PATH:{3}", pathCurDir, pathDll, sepaDir, sepaPath));
#endif// LOG_DEBUG
            if (!paths.Contains(pathDll))
            {
                sb.Length = 0;
                sb.Append(paths).Append(sepaPath).Append(pathDll);
                paths = sb.ToString();
#if LOG_DEBUG
                Debug.Log(string.Format("UNITY_NATIVE_WIN:PATHS_NEXT:{0}", paths));
#endif// LOG_DEBUG
                Environment.SetEnvironmentVariable("PATH", paths, EnvironmentVariableTarget.Process);
            }
        }
#endif// UNITY_EDITOR || UNITY_STANDALONE_WIN
    }
}