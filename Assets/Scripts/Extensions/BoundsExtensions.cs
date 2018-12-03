using UnityEngine;

namespace Extensions
{
    public static class BoundsExtensions
    {
        public static Vector3Int BottomLeft(this Bounds b)
        {
            return new Vector3Int((int) b.min.x, (int) b.min.y, 0);
        }

        public static Vector3Int BottomRight(this Bounds b)
        {
            return new Vector3Int((int) b.max.x - 1, (int) b.min.y, 0);
        }

        public static Vector3Int TopLeft(this Bounds b)
        {
            return new Vector3Int((int) b.min.x, (int) b.max.y - 1, 0);
        }

        public static Vector3Int TopRight(this Bounds b)
        {
            return new Vector3Int((int) b.max.x - 1, (int) b.max.y - 1, 0);
        }

        public static Bounds Clamp(this Bounds a, Bounds b)
        {
            Bounds c = new Bounds();
            c.SetMinMax
            (
                new Vector3(Mathf.Clamp(a.min.x, b.min.x, b.max.x),
                    Mathf.Clamp(a.min.y, b.min.y, b.max.y)),
                new Vector3(Mathf.Clamp(a.max.x, b.min.x, b.max.x),
                    Mathf.Clamp(a.max.y, b.min.y, b.max.y))
            );

            return c;
        }
    }
}