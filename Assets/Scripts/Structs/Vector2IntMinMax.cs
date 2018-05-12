using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Structs
{
    [Serializable]
    public struct Vector2IntMinMax
    {
        public Vector2Int Min, Max;

        public Vector2IntMinMax(Vector2Int center, Vector2Int size)
        {
            var halfSize = new Vector2Int(size.x / 2, size.y / 2);
            Min = center - halfSize;
            Max = center + halfSize;
        }

        public void SetMinMax(Vector2Int min, Vector2Int max)
        {
            Min = min;
            Max = max;
        }

        public Vector2Int GetRandomValue()
        {
            return new Vector2Int(Random.Range(Min.x, Max.x), (int) Random.Range(Min.y, (float) Max.y));
        }

        public static Vector2IntMinMax Difference(Vector2IntMinMax a, Vector2IntMinMax b)
        {
            Vector2IntMinMax c = new Vector2IntMinMax
            {
                Min = a.Min - b.Min,
                Max = a.Max - b.Max
            };

            return c;
        }

        public Vector2IntMinMax Clamp(Vector2IntMinMax b)
        {
            Vector2IntMinMax c = new Vector2IntMinMax
            {
                Min = new Vector2Int(Mathf.Clamp(Min.x, b.Min.x, b.Max.x), Mathf.Clamp(Min.y, b.Min.y, b.Max.y)),
                Max = new Vector2Int(Mathf.Clamp(Max.x, b.Min.x, b.Max.x), Mathf.Clamp(Max.y, b.Min.y, b.Max.y))
            };

            return c;
        }

        public static explicit operator Rect(Vector2IntMinMax b)
        {
            return Rect.MinMaxRect(b.Min.x, b.Min.y, b.Max.x, b.Max.y);
        }
    }
}