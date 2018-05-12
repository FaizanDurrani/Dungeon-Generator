using Enums;
using UnityEngine;

namespace DungeonGeneration.Structs
{
    public struct FeaturePosition
    {
        public int x, y;
        public Direction Direction;
        
        public Vector2Int Left => this + Vector2Int.left;
        public Vector2Int Top => this - Vector2Int.up;
        public Vector2Int Right => this + Vector2Int.right;
        public Vector2Int Bottom => this - Vector2Int.down;

        public Vector2Int TopLeft => this + Vector2Int.left - Vector2Int.up;
        public Vector2Int TopRight => this + Vector2Int.right - Vector2Int.up;
        public Vector2Int BottomRight => this + Vector2Int.right - Vector2Int.down;
        public Vector2Int BottomLeft => this + Vector2Int.left - Vector2Int.down;
        
        public FeaturePosition(int x, int y)
        {
            this.x = x;
            this.y = y;
            Direction = 0;
        }

        public FeaturePosition(int x, int y, Direction d)
        {
            this.x = x;
            this.y = y;
            Direction = d;
        }

        public static implicit operator Vector2Int(FeaturePosition f)
        {
            return new Vector2Int(f.x, f.y);
        }

        public string ToString()
        {
            return "(" + x + ", " + y + ", " + Direction + ")";
        }
    }
}