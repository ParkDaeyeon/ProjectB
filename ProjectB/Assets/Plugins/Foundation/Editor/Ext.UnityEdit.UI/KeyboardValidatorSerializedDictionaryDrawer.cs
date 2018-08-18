using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

using Ext.Unity3D.UI;

namespace Ext.UnityEdit.UI
{
    [CustomPropertyDrawer(typeof(KeyboardValidator.KeyboardValidatorSerializedDictionary))]
    public class KeyboardValidatorSerializedDictionaryDrawer : SerializedDictionaryDrawer { }
}
