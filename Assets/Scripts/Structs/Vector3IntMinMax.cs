using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Structs
{
    [Serializable]
    public struct Vector3IntMinMax
    {
        public Vector3Int Min, Max;

        public Vector3IntMinMax(Vector3Int center, Vector3Int size)
        {
            var halfSize = new Vector3Int(size.x / 2, size.y / 2, 0);
            Min = center - halfSize;
            Max = center + halfSize;
        }

        public void SetMinMax(Vector3Int min, Vector3Int max)
        {
            Min = min;
            Max = max;
        }

        public Vector3Int GetRandomValue()
        {
            return new Vector3Int((int) Random.Range(Min.x, (float) Max.x),
                (int) Random.Range(Min.y, (float) Max.y), 0);
        }

        public static Vector3IntMinMax Difference(Vector3IntMinMax a, Vector3IntMinMax b)
        {
            Vector3IntMinMax c = new Vector3IntMinMax
            {
                Min = a.Min - b.Min,
                Max = a.Max - b.Max
            };

            return c;
        }
        public static explicit operator Rect(Vector3IntMinMax b)
        {
            return Rect.MinMaxRect(b.Min.x, b.Min.y, b.Max.x, b.Max.y);
        }
    }
}