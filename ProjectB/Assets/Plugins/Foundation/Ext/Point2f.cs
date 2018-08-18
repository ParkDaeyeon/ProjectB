namespace Ext
{
    public struct Point2f
    {
        public float x;
        public float y;

        public Point2f(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static Point2f operator +(Point2f l, Point2f r)
        {
            return new Point2f(l.x + r.x, l.y + r.y);
        }
        public static Point2f operator -(Point2f l, Point2f r)
        {
            return new Point2f(l.x - r.x, l.y - r.y);
        }
        public static bool operator ==(Point2f l, Point2f r)
        {
            return l.x == r.x && l.y == r.y;
        }
        public static bool operator !=(Point2f l, Point2f r)
        {
            return !(l == r);
        }

        public static implicit operator Point2f(Point2 pt)
        {
            return new Point2f(pt.x, pt.y);
        }
        public static Point2 ToPoint2(Point2f ptf)
        {
            return new Point2((int)ptf.x, (int)ptf.y);
        }


        public override bool Equals(object obj)
        {
            if (obj is Point2f || obj is Point2)
                return this == ((Point2f)obj);

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

        public static readonly Point2f Zero = new Point2f { x = 0, y = 0, };
        public static readonly Point2f One = new Point2f { x = 1, y = 1, };
    }
}