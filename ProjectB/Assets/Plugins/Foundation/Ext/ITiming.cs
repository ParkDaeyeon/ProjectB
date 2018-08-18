namespace Ext
{
    public interface ITiming
    {
        long? StartAt { get; }
        long? EndAt { get; }
    }

    public static class ITimingExtension
    {
        public static Term ToTerm(this ITiming thiz, int clientStartOffsetSeconds = 0, int clientEndOffsetSeconds = 0)
        {
            return new Term(thiz.StartAt, thiz.EndAt, clientStartOffsetSeconds, clientEndOffsetSeconds);
        }
    }
}
