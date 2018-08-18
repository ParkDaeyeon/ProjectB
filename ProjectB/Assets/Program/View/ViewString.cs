using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Ext;
using Ext.Unity3D;
using Ext.Unity3D.UI;

namespace Program.View
{
    public static class ViewString<T>
    {
        public delegate string Provider(T arg, string defaultValue);
        
        static Provider provider;
        public static void SetProvider(Provider provider)
        {
            ViewString<T>.provider = provider;
        }

        public static string GetString(T arg, string defaultValue)
        {
            if (null != ViewString<T>.provider)
                return ViewString<T>.provider(arg, defaultValue);

            return defaultValue;
        }
    }
}
