using UnityEngine;

public static class VectorExtensions
{
    public static Vector2Int ToVector2Int(this Vector2 v)
    {
        return new Vector2Int((int) v.x, (int) v.y);
    }
    
    public static Vector2Int ToVector2Int(this Vector3 v)
    {
        return new Vector2Int((int) v.x, (int) v.y);
    }
    
    public static Vector2 ToVector2(this Vector2Int v)
    {
        return new Vector2(v.x, v.y);
    }
    
    public static Vector3 ToVector3(this Vector2Int v)
    {
        return new Vector3(v.x, v.y);
    }

    public static Vector2Int Abs(this Vector2Int v)
    {
        return new Vector2Int(Mathf.Abs(v.x),Mathf.Abs(v.y));
    }
}