namespace Ext
{
    public struct Point2
    {
        public int x;
        public int y;

        public Point2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Point2 operator +(Point2 l, Point2 r)
        {
            return new Point2(l.x + r.x, l.y + r.y);
        }
        public static Point2 operator -(Point2 l, Point2 r)
        {
            return new Point2(l.x - r.x, l.y - r.y);
        }
        public static bool operator ==(Point2 l, Point2 r)
        {
            return l.x == r.x && l.y == r.y;
        }
        public static bool operator !=(Point2 l, Point2 r)
        {
            return !(l == r);
        }

        public override bool Equals(object obj)
        {
            if (obj is Point2)
                return this == ((Point2)obj);
            else if (obj is Point2f)
                return this == Point2f.ToPoint2((Point2f)obj);

            return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return string.Format("{{x:{0}, y:{1}}}", this.x, this.y);
        }

        public static readonly Point2 Zero = new Point2 { x = 0, y = 0, };
        public static readonly Point2 One = new Point2 { x = 1, y = 1, };
    }
}
