using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using Ext.Unity3D;
using Ext.IO;
using Program.Presents.Helper;

namespace Program
{
    public class Restart : MonoBehaviour
    {
        static bool isRestarting;
        public static bool IsRestarting { get { return Restart.isRestarting; } }

        static int generation;
        public static int Generation { get { return Restart.generation; } }

        public static void Open()
        {
            if (Restart.isRestarting)
                return;

            Preference.Save();
            Restart.isRestarting = true;

            if (Startup.Instance)
                GameObject.Destroy(Startup.Instance.gameObject);

            SceneManager.LoadScene("Restart");
        }



        void Awake()
        {
            GameObject.DontDestroyOnLoad(this.gameObject);
        }

        void Start()
        {
            var startup = Startup.Instance;
            if (startup)
                startup.gameObject.Destroy();
        }

        int frameCount = 0;
        void Update()
        {
            ++this.frameCount;
            if (10 == this.frameCount)
            {
                Resources.UnloadUnusedAssets();
            }
            else if (20 == this.frameCount)
            {
                SceneManager.LoadScene(0);

                Resources.UnloadUnusedAssets();
            }
            else if (30 == this.frameCount)
            {
                GameObject.Destroy(this.gameObject);

                Restart.isRestarting = false;
                ++Restart.generation;

                Resources.UnloadUnusedAssets();
            }
        }
    }
}