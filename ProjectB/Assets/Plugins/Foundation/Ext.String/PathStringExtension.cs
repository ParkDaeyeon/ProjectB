using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;
namespace Ext.String
{
    public static class PathStringExtension
    {
        public static string ToSeparatedPath(this string path, char from = '\\', char to = '/')
        {
            if (string.IsNullOrEmpty(path))
                return "";

            if (path.Contains(from))
                path = path.Replace(from, to);

            return path;
        }
        
        public static string ToAbsPath(this string path, bool isDir)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(Path.GetPathRoot(path)))
                path = Path.Combine(Directory.GetCurrentDirectory(), path);

            path = path.ToSeparatedPath();

            if (isDir)
            {
                if (0 == path.Length || '/' != path[path.Length - 1])
                    path += '/';
            }

            return path;
        }
        
        public static string ToWithoutExtPath(this string path)
        {
            if (string.IsNullOrEmpty(path))
                return "";
            
            path = path.ToSeparatedPath();

            var extIdx = path.LastIndexOf('.');
            if (-1 != extIdx)
            {
                var slashIdx = path.IndexOf('/');
                if (-1 == slashIdx || slashIdx < extIdx)
                    path = path.Substring(0, extIdx);
            }
            return path;
        }
    }
}
