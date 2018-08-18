using System;
using System.Collections.Generic;
using System.Text;

namespace Ext.IO
{
    public class Archive<T>
    {
        public Archive(string schemeName)
        {
            this.schemeName = schemeName.ToLower();
            this.schemeNameSize = this.schemeName.Length;

            this.scheme = string.Format("{0}://", this.schemeName);
            this.schemeSize = this.scheme.Length;

            this.sb = new StringBuilder(this.scheme, 2048);

            this.map = new Dictionary<string, T>();
        }

        string schemeName;
        public string SchemeName
        {
            get { return this.schemeName; }
        }

        int schemeNameSize;
        public int SchemeNameSize
        {
            get { return this.schemeNameSize; }
        }

        string scheme;
        public string Scheme
        {
            get { return this.scheme; }
        }

        int schemeSize;
        public int SchemeSize
        {
            get { return this.schemeSize; }
        }

        public bool CheckScheme(string uri, bool schemeNameOnly = false)
        {
            if (string.IsNullOrEmpty(uri))
                return false;

            var size = schemeNameOnly ? this.schemeNameSize : this.schemeSize;
            if (uri.Length < size)
                return false;

            for (int n = 0; n < size; ++n)
            {
                if (uri[n] != this.scheme[n])
                    return false;
            }

            return true;
        }

        public int FindSeparateIndex(string uri, bool validScheme = false)
        {
            if (string.IsNullOrEmpty(uri))
                return -1;

            if (!validScheme)
            {
                if (!this.CheckScheme(uri))
                    return -1;
            }

            return uri.IndexOf(':', this.schemeSize);
        }

        public string ExtractHostname(string uri, int separateIndex = -1)
        {
            if (string.IsNullOrEmpty(uri))
                return "";

            if (-1 == separateIndex)
            {
                separateIndex = this.FindSeparateIndex(uri);
                if (-1 == separateIndex)
                    return "";
            }

            return uri.Substring(this.schemeSize, separateIndex - this.schemeSize);
        }

        public string ExtractFilename(string uri, int separateIndex = -1)
        {
            if (string.IsNullOrEmpty(uri))
                return "";

            if (-1 == separateIndex)
            {
                separateIndex = this.FindSeparateIndex(uri);
                if (-1 == separateIndex)
                    return "";
            }

            return uri.Substring(separateIndex + 1);
        }

        public Tuple<string, string> Extract(string uri, int separateIndex = -1)
        {
            if (string.IsNullOrEmpty(uri))
                return new Tuple<string, string>("", "");

            if (-1 == separateIndex)
            {
                separateIndex = this.FindSeparateIndex(uri);
                if (-1 == separateIndex)
                    return new Tuple<string, string>("", "");
            }

            return new Tuple<string, string>(this.ExtractHostname(uri, separateIndex),
                                             this.ExtractFilename(uri, separateIndex));
        }

        StringBuilder sb;
        public string AddScheme(string content)
        {
            if (this.CheckScheme(content))
                return content;

            content = this.sb.Append(content).ToString();
            this.sb.Length = this.schemeSize;
            return content;
        }

        public string RemoveScheme(string content)
        {
            if (this.CheckScheme(content))
                content = content.Substring(this.schemeSize);
            return content;
        }

        Dictionary<string, T> map;
        public void Mount(string hostname, T value)
        {
            if (string.IsNullOrEmpty(hostname))
                throw new Exception(string.Format("ARCHIVE<{0}>.{1}:MOUNT:INVALID_HOSTNAME:{2}",
                    typeof(T).Name,
                    this.schemeName,
                    hostname));

            this.map.Add(this.RemoveScheme(hostname), value);
        }

        public bool Unmount(string hostname)
        {
            if (string.IsNullOrEmpty(hostname))
                throw new Exception(string.Format("ARCHIVE<{0}>.{1}:UNMOUNT:INVALID_HOSTNAME:{2}",
                                                   typeof(T).Name,
                                                   this.schemeName,
                                                   hostname));

            return this.map.Remove(this.RemoveScheme(hostname));
        }

        public void UnmountAll()
        {
            this.map.Clear();
        }

        public T Get(string hostname)
        {
            var value = default(T);
            if (this.map.TryGetValue(this.RemoveScheme(hostname), out value))
                return value;

            return default(T);
        }

        public IEnumerable<KeyValuePair<string, T>> GetAll()
        {
            return this.map;
        }
    }
}
