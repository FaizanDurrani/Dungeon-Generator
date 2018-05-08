using System;
using UnityEngine;

public struct CellPosition : IEquatable<CellPosition>
{
    public int X, Y;

    public CellPosition(int y, int x)
    {
        X = x;
        Y = y;
    }
    
    public CellPosition(Vector2Int p)
    {
        X = p.x;
        Y = p.y;
    }

    public Vector2Int ToVector2Int()
    {
        return new Vector2Int(X, Y);
    }

    public bool Equals(CellPosition other)
    {
        return X == other.X && Y == other.Y;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        return obj is CellPosition && Equals((CellPosition) obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (X * 397) ^ Y;
        }
    }
}