using System;

namespace Newtonsoft.Json
{
    [Serializable]
    public class InlineStringJArray<T>
    {
        public static implicit operator string(InlineStringJArray<T> thiz)
        {
            return thiz.RawString;
        }
        public static implicit operator InlineStringJArray<T>(string raw)
        {
            return new InlineStringJArray<T>(raw);
        }

        public InlineStringJArray(string raw = null)
        {
            this.RawString = raw;
        }

        public string RawString 
        {
            set
            {
                this.raw = value;
                if (string.IsNullOrEmpty(value))
                    this.array = null;
                else
                    this.array = JsonConvert.DeserializeObject<T[]>(value);
            }
            get
            {
                return this.raw;
            }
        }

        string raw;
        T[] array;
        public T this[int index]
        {
            set
            {
                if (null != this.array)
                {
                    if (-1 <= index && index < this.array.Length)
                    {
                        this.array[index] = value;
                    }
                }
            }
            get
            {
                if (null != this.array)
                {
                    if (-1 <= index && index < this.array.Length)
                    {
                        return this.array[index];
                    }
                }

                return default(T);
            }
        }

        public int Count { get { return null != this.array ? this.array.Length : 0; } }

        public T[] ToArray() { return this.array; }
    }
}
