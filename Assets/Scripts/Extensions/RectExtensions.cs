using UnityEngine;

namespace Extensions
{
    public static class RectExtensions
    {
        public static Vector2 TopLeft(this Rect b)
        {
            return new Vector2(b.xMin, b.yMin);
        }

        public static Vector2 TopRight(this Rect b)
        {
            return new Vector2(b.xMax - 1, b.yMin);
        }

        public static Vector2 BottomLeft(this Rect b)
        {
            return new Vector2(b.xMin, b.yMax - 1);
        }

        public static Vector2 BottomRight(this Rect b)
        {
            return new Vector2(b.xMax - 1, b.yMax - 1);
        }

        public static Rect Clamp(this Rect a, Rect b)
        {
            Rect c = Rect.MinMaxRect
            (
                Mathf.Clamp(a.min.x, b.min.x, b.max.x),
                Mathf.Clamp(a.min.y, b.min.y, b.max.y),
                Mathf.Clamp(a.max.x, b.min.x, b.max.x), 
                Mathf.Clamp(a.max.y, b.min.y, b.max.y)
            );

            return c;
        }
    }
}