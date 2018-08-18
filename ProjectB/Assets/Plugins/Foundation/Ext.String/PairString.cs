namespace Ext.String
{
    public static class PairString
    {
        public static bool TryParse(string fullPath, string separator, out string head, out string body)
        {
            head = null;
            body = null;
            if (string.IsNullOrEmpty(fullPath))
                return false;
            if (string.IsNullOrEmpty(separator))
                return false;

            var index = fullPath.IndexOf(separator);
            if (-1 == index)
                return false;
            head = fullPath.Substring(0, index);
            body = fullPath.Substring(index + separator.Length);
            return true;
        }
    }
}
