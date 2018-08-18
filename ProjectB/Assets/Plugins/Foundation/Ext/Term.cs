using System;
namespace Ext
{
    public class Term
    {
        protected DateTime startAt = DateTime.MinValue;
        public DateTime StartAt { get { return this.startAt; } }

        protected DateTime endAt = DateTime.MaxValue;
        public DateTime EndAt { get { return this.endAt; } }

        protected int clientStartOffsetSeconds = 0;
        public int ClientStartOffsetMinute { get { return this.clientStartOffsetSeconds; } }

        protected int clientEndOffsetSeconds = 0;
        public int ClientEndOffsetMinute { get { return this.clientEndOffsetSeconds; } }


        public Term(DateTime startAt, DateTime endAt, int clientStartOffsetSeconds = 0, int clientEndOffsetSeconds = 0)
        {
            this.Set(startAt, endAt, clientStartOffsetSeconds, clientEndOffsetSeconds);
        }

        public void Set(DateTime startAt, DateTime endAt, int clientStartOffsetSeconds = 0, int clientEndOffsetSeconds = 0)
        {
            if (default(DateTime) == startAt)
                startAt = DateTime.MinValue;
            else
                startAt = startAt.AddSeconds(clientStartOffsetSeconds);

            if (default(DateTime) == endAt)
                endAt = DateTime.MaxValue;
            else
                endAt = endAt.AddSeconds(clientEndOffsetSeconds);

            this.startAt = startAt;
            this.endAt = endAt;
            this.clientStartOffsetSeconds = clientStartOffsetSeconds;
            this.clientEndOffsetSeconds = clientEndOffsetSeconds;
        }


        public Term(long startAt, long endAt, int clientStartOffsetSeconds = 0, int clientEndOffsetSeconds = 0)
        {
            this.Set(startAt, endAt, clientStartOffsetSeconds, clientEndOffsetSeconds);
        }

        public void Set(long startAt, long endAt, int clientStartOffsetSeconds = 0, int clientEndOffsetSeconds = 0)
        {
            this.Set(startAt.MsToDateTime(), endAt.MsToDateTime(), clientStartOffsetSeconds, clientEndOffsetSeconds);
        }


        public Term(long? startAt, long? endAt, int clientStartOffsetSeconds = 0, int clientEndOffsetSeconds = 0)
        {
            this.Set(startAt, endAt, clientStartOffsetSeconds, clientEndOffsetSeconds);
        }

        public void Set(long? startAt, long? endAt, int clientStartOffsetSeconds = 0, int clientEndOffsetSeconds = 0)
        {
            DateTime tempStartAt = (startAt.HasValue) ? startAt.Value.MsToDateTime() : default(DateTime);
            DateTime tempEndAt = (endAt.HasValue) ? endAt.Value.MsToDateTime() : default(DateTime);
            this.Set(tempStartAt, tempEndAt, clientStartOffsetSeconds, clientEndOffsetSeconds);
        }



        public bool IsFuture(DateTime date)
        {
            return date < this.startAt;
        }

        public bool IsOld(DateTime date)
        {
            return this.endAt < date;
        }

        public bool IsValid
        {
            get
            {
                return this.startAt != DateTime.MinValue || this.endAt != DateTime.MaxValue;
            }
        }

        public bool Contains(DateTime date)
        {
            return !this.IsFuture(date) && !this.IsOld(date);
        }

        public override string ToString()
        {
            return string.Format("{{startAt:{0}, endAt:{1}}}", this.startAt, this.endAt);
        }
    }
}