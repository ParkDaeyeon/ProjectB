using System;
namespace Ext
{
    public class CountTiming
    {
        int count;
        public int Count
        {
            set { this.count = value; }
            get { return this.count; }
        }

        int current;
        public int Current
        {
            set { this.current = value; }
            get { return this.current; }
        }

        bool timing;
        public bool IsTiming { get { return this.timing; } }

        public CountTiming(int count, int start = 0)
        {
            this.count = count;
            this.current = start;
        }

        public bool UpdateTiming()
        {
            return this.timing = 0 == ((++this.current) % this.count);
        }

        public void Reset()
        {
            this.current = 0;
        }
    }
}