using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct RandomVector2Int
{
    public Vector2Int Min, Max;

    public Vector2Int GetRandomValue()
    {
        return new Vector2Int(Random.Range(Min.x, Max.x), (int) Random.Range(Min.y, (float) Max.y));
    }
}