using UnityEngine;

namespace Ext.Unity3D
{
    public static class CanvasExtension
    {
        static Vector3[] cornersOfGetScreenRect = new Vector3[4];
        public static Rect GetScreenRect(this RectTransform rectTransform, Canvas canvas, Camera camera = null)
        {
            var corners = CanvasExtension.cornersOfGetScreenRect;
            Vector3 screenCorner0;
            Vector3 screenCorner1;

            if (!canvas)
                canvas = rectTransform.GetComponentInParent<Canvas>();

            rectTransform.GetWorldCorners(corners);

            if (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace)
            {
                screenCorner0 = RectTransformUtility.WorldToScreenPoint(camera ? camera : canvas.worldCamera, corners[0]);
                screenCorner1 = RectTransformUtility.WorldToScreenPoint(camera ? camera : canvas.worldCamera, corners[2]);
            }
            else
            {
                screenCorner0 = RectTransformUtility.WorldToScreenPoint(null, corners[0]);
                screenCorner1 = RectTransformUtility.WorldToScreenPoint(null, corners[2]);
            }

            var size = screenCorner1 - screenCorner0;
            var sizeHalf = size * 0.5f;
            var localPosition = screenCorner0 + sizeHalf;

            var screenSize = UnityExtension.ScreenSize;
            var screenSizeHalf = screenSize * 0.5f;
            var position = new Vector2(localPosition.x - screenSizeHalf.x,
                                       localPosition.y - screenSizeHalf.y);

            var min = new Vector2(position.x - sizeHalf.x, position.y - sizeHalf.y);
            var max = new Vector2(position.x + sizeHalf.x, position.y + sizeHalf.y);

            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }
    }
}
