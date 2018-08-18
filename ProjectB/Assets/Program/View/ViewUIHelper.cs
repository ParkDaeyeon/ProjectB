using UnityEngine;
using UnityEngine.UI;

namespace Program.View
{
    public static class ViewUIHelper
    {
        public static void SetIterate(Image[] images, Sprite[] sprites, bool autoVisible = false)
        {
            if (null == images || null == sprites)
                return;

            for (int n = 0, cnt = images.Length; n < cnt; ++n)
            {
                var image = images[n];
                if (!image)
                    continue;

                var sprite = n < sprites.Length ? sprites[n] : null;

                image.SetSprite(sprite, autoVisible);
            }
        }

        public static void SetIterate(Image[] images, Sprite[] sprites, Color[] colors, bool autoVisible = false)
        {
            if (null == images || null == sprites)
                return;

            for (int n = 0, cnt = images.Length; n < cnt; ++n)
            {
                var image = images[n];
                if (!image)
                    continue;

                var sprite = n < sprites.Length ? sprites[n] : null;
                var color = n < colors.Length ? colors[n] : Color.clear;

                image.SetSprite(sprite, color, autoVisible);
            }
        }


        public static bool SetSprite(this Image target,
                                     Sprite sprite,
                                     Color color,
                                     bool autoVisible = false)
        {
            if (!target.SetSprite(sprite, autoVisible))
                return false;

            target.color = color;

            return true;
        }

        public static bool SetSprite(this Image target,
                                     Sprite sprite,
                                     bool autoVisible = false)
        {
            if (!target)
                return false;

            if (autoVisible)
                target.gameObject.SetActive(false);

            target.sprite = sprite;

            if (sprite)
            {
                if (autoVisible)
                    target.gameObject.SetActive(true);
            }

            return true;
        }
    }
}
