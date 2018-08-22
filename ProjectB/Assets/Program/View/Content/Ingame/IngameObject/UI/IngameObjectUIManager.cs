using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Program.View.Content.Ingame.IngameObject.UI
{
    public static class IngameObjectUIManager
    {
        static bool isOpened;
        public static bool IsOpened
        {
            get { return IngameObjectUIManager.isOpened; }
        }

        public static void Open()
        {
            IngameObjectUIManager.isOpened = true;

            IngameObjectUIManager.hpBars = new List<HpBar>(32);
        }

        public static void Close()
        {
            IngameObjectUIManager.isOpened = false;
            IngameObjectUIManager.Clear();

            IngameObjectUIManager.hpBars = null;
        }

        static List<HpBar> hpBars;
        public static void Regist(HpBar item)
        {
            var list = IngameObjectUIManager.hpBars;
            if (null != list && 0 < list.Count)
            {
                if (!list.Contains(item))
                    list.Add(item);
            }
        }
        public static void Unregist(HpBar item)
        {
            var list = IngameObjectUIManager.hpBars;
            if (null != list && 0 < list.Count)
            {
                if (list.Contains(item))
                    list.Remove(item);
            }
        }

        public static void Update()
        {
            var list = IngameObjectUIManager.hpBars;
            for (int n = 0, cnt = list.Count; n < cnt; ++n)
            {
                var hpBar = list[n];
                hpBar.UIUpdate();
            }
        }

        public static void Clear()
        {
            IngameObjectUIManager.hpBars.Clear();
        }
    }
}
