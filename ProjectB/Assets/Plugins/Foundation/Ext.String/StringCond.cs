using System.Text;

namespace Ext.String
{
    public struct StringCond
    {
        public struct Keyword
        {
            public string value;

            public enum Cond
            {
                Prefix,
                Suffix,
                Contains,
                Exclude,
            }
            public Cond cond;

            public bool Evaluate(string content)
            {
                if (null == this.value || null == content)
                    return false;

                switch (this.cond)
                {
                case Cond.Prefix:
                    return 0 == content.IndexOf(this.value);

                case Cond.Suffix:
                    {
                        var index = content.LastIndexOf(this.value);
                        return -1 != index && (content.Length - this.value.Length) == index;
                    }

                case Cond.Contains:
                    return content.Contains(this.value);

                case Cond.Exclude:
                    return !content.Contains(this.value);
                }

                return false;
            }

            public override string ToString()
            {
                return this.AppendString(new StringBuilder()).ToString();
            }
            public StringBuilder AppendString(StringBuilder sb)
            {
                sb.Append('{');

                sb.Append("\"value\": ");
                if (null != this.value)
                    sb.Append('\"').Append(this.value).Append('\"');
                else
                    sb.Append("null");

                sb.Append(',').Append(' ');

                sb.Append("\"cond\": ");
                sb.Append('\"').Append(this.cond).Append('\"');

                sb.Append('}');
                return sb;
            }
        }

        public struct Chunk
        {
            public Keyword[] keywords;

            public enum Operator
            {
                And,
                Or,
            }
            public Operator oper;

            public bool Evaluate(string content)
            {
                if (null == this.keywords)
                    return false;

                switch (this.oper)
                {
                case Operator.And:
                    {
                        foreach (var keyword in this.keywords)
                            if (!keyword.Evaluate(content))
                                return false;

                        return true;
                    }

                case Operator.Or:
                    {
                        foreach (var keyword in this.keywords)
                            if (keyword.Evaluate(content))
                                return true;

                        return false;
                    }
                }

                return false;
            }

            public override string ToString()
            {
                return this.AppendString(new StringBuilder()).ToString();
            }
            public StringBuilder AppendString(StringBuilder sb)
            {
                sb.Append('{');

                sb.Append("\"keywords\": ");
                if (null != this.keywords)
                {
                    sb.Append('[');
                    var first = true;
                    foreach (var keyword in this.keywords)
                    {
                        if (first)
                            first = false;
                        else
                            sb.Append(',').Append(' ');
                        keyword.AppendString(sb);
                    }
                    sb.Append(']');
                }
                else
                    sb.Append("null");

                sb.Append(',').Append(' ');

                sb.Append("\"oper\": ");
                sb.Append('\"').Append(this.oper).Append('\"');

                sb.Append('}');
                return sb;
            }
        }
        public Chunk[] chunks;

        public bool Evaluate(string content)
        {
            if (null == this.chunks)
                return false;

            foreach (var chunk in chunks)
            {
                if (!chunk.Evaluate(content))
                    return false;
            }

            return true;
        }

        public override string ToString()
        {
            return this.AppendString(new StringBuilder()).ToString();
        }
        public StringBuilder AppendString(StringBuilder sb)
        {
            sb.Append('{');

            sb.Append("\"chunks\": ");
            if (null != this.chunks)
            {
                sb.Append('[');
                var first = true;
                foreach (var chunk in this.chunks)
                {
                    if (first)
                        first = false;
                    else
                        sb.Append(',').Append(' ');
                    chunk.AppendString(sb);
                }
                sb.Append(']');
            }
            else
                sb.Append("null");

            sb.Append('}');
            return sb;
        }
    }
}
