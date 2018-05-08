using UnityEngine;

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
}