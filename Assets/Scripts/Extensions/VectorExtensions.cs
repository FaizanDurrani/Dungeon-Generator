using UnityEngine;

namespace Extensions
{
    public static class VectorExtensions
    {
        public static Vector2Int ToVector2Int(this Vector2 v)
        {
            return new Vector2Int((int) v.x, (int) v.y);
        }

        public static Vector2Int ToVector2Int(this Vector3Int v)
        {
            return new Vector2Int(v.x, v.y);
        }

    }
}