using System;
namespace Ext.Unity3D
{
    /*!
     * @brief	attribute that allows static functions to have callbacks (from C) generated AOT
     */
    [AttributeUsage(AttributeTargets.Method)]
    public class MonoPInvokeCallbackAttribute : System.Attribute
    {
#pragma warning disable 0414
        Type type;
#pragma warning restore 0414
        public MonoPInvokeCallbackAttribute(Type t) { this.type = t; }
    }
}