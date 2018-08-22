using Program.View.Content.Ingame.IngameObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Program.View.Content.Ingame
{
    public static class UnitSpritesManager
    {
        static bool isOpened = false;
        public static bool IsOpened
        {
            get { return UnitSpritesManager.isOpened; }
        }

        static Dictionary<long, UnitBody.UnitBodySprites> bodySpritesMap;
        public static void Open(Dictionary<long, UnitBody.UnitBodySprites> bodySprites)
        {
            UnitSpritesManager.isOpened = true;
            UnitSpritesManager.bodySpritesMap = bodySprites;
        }
        public static void Close()
        {
            if (null != UnitSpritesManager.bodySpritesMap)
                UnitSpritesManager.bodySpritesMap.Clear();

            UnitSpritesManager.bodySpritesMap = null;

            UnitSpritesManager.isOpened = false;
        }

        public static UnitBody.UnitBodySprites GetBodySpritesByCode(long code)
        {
            if (!UnitSpritesManager.IsOpened)
                return null;

            UnitBody.UnitBodySprites sprites;
            if (!UnitSpritesManager.bodySpritesMap.TryGetValue(code, out sprites))
                return null;

            return sprites;
        }

    }
}
