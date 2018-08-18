using System;
namespace Ext
{
    public static class ResolutionChecker
    {
        public static bool IsOpened { get { return null != ResolutionChecker.currentCache; } }

        public static void Open(OnGetScreenSizeCallback onGetScreenSizeFunc, Action<ResolutionChecker.ScreenInfo>[] onChangedFuncs = null)
        {
            ResolutionChecker.Close();

            ResolutionChecker.currentCache = new ResolutionChecker.ScreenInfo();

            ResolutionChecker.onGetScrrenSize = onGetScreenSizeFunc;
            if (null != onGetScreenSizeFunc)
                ResolutionChecker.currentCache.Reset(onGetScreenSizeFunc());

            if (null != onChangedFuncs)
            {
                for (int n = 0, cnt = onChangedFuncs.Length; n < cnt; ++n)
                    if (null != onChangedFuncs[n])
                        ResolutionChecker.onChanged += onChangedFuncs[n];
            }
        }


        public static void Close()
        {
            if (ResolutionChecker.IsOpened)
            {
                ResolutionChecker.currentCache = null;
                ResolutionChecker.onGetScrrenSize = null;
                ResolutionChecker.onChanged = null;
            }
        }


        public static ResolutionChecker.ScreenInfo Current
        {
            get
            {
                return ResolutionChecker.currentCache;
            }
        }


        public static void Update()
        {
            if (!ResolutionChecker.IsOpened)
                return;

            var onGetScreenSizeFunc = ResolutionChecker.onGetScrrenSize;
            if (null == onGetScreenSizeFunc)
                return;

            var size = onGetScreenSizeFunc();
            if (ResolutionChecker.currentCache.Size != size)
            {
                ResolutionChecker.currentCache.Reset(size);

                var onChangedEvents = ResolutionChecker.onChanged;
                if (null != onChangedEvents)
                    onChangedEvents(ResolutionChecker.currentCache);
            }
        }



        public struct ScreenSize
        {
            public ScreenSize(int w = 1, int h = 1)
            {
                this.w = 0 < w ? w : 1;
                this.h = 0 < h ? h : 1;
            }

            int w;
            public int Width
            {
                set { this.w = 0 < value ? value : 1; }
                get { return this.w; }
            }

            int h;
            public int Height
            {
                set { this.h = 0 < value ? value : 1; }
                get { return this.h; }
            }


            public static bool operator ==(ScreenSize l, ScreenSize r)
            {
                return l.w == r.w && l.h == r.h;
            }
            public static bool operator !=(ScreenSize l, ScreenSize r)
            {
                return l.w != r.w || l.h != r.h;
            }

            
            public override bool Equals(object obj)
            {
                if (obj is ScreenSize)
                    return this == ((ScreenSize)obj);

                return false;
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
            public override string ToString()
            {
                return string.Format("{{\"Width\": {0}, \"Height\": {1}}}", this.w, this.h);
            }
        }


        public class ScreenInfo
        {
            public ResolutionChecker.ScreenSize Size;
            public float AspectRatio = 1;
            public bool IsLandscape = false;

            public ScreenInfo() { }
            public ScreenInfo(ResolutionChecker.ScreenInfo rhs)
            {
                this.Sync(rhs);
            }

            public void Sync(ResolutionChecker.ScreenInfo rhs)
            {
                this.Size = rhs.Size;
                this.AspectRatio = rhs.AspectRatio;
                this.IsLandscape = rhs.IsLandscape;
            }

            public void Reset(ResolutionChecker.ScreenSize size)
            {
                this.Size = size;
                this.AspectRatio = 0 != this.Size.Height ? this.Size.Width / (float)this.Size.Height : 0;
                this.IsLandscape = 1 < this.AspectRatio;
            }

            public override string ToString()
            {
                return string.Format("{{\"Size\": {0}, \"AspectRatio\": {1}, \"IsLandscape\": {2}}}", this.Size, this.AspectRatio, this.IsLandscape);
            }
        }
        static ResolutionChecker.ScreenInfo currentCache;



        public delegate ResolutionChecker.ScreenSize OnGetScreenSizeCallback();
        static OnGetScreenSizeCallback onGetScrrenSize;
        public static OnGetScreenSizeCallback OnGetScreenSize
        {
            set
            {
                ResolutionChecker.onGetScrrenSize = value;
            }
            get
            {
                return ResolutionChecker.onGetScrrenSize;
            }
        }



        static event Action<ResolutionChecker.ScreenInfo> onChanged;
        public static event Action<ResolutionChecker.ScreenInfo> OnChanged
        {
            add
            {
                ResolutionChecker.onChanged += value;
            }
            remove
            {
                ResolutionChecker.onChanged -= value;
            }
        }
    }
}