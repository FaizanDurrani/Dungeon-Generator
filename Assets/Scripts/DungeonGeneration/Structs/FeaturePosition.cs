using Enums;
using UnityEngine;

namespace DungeonGeneration.Structs
{
    public struct FeaturePosition
    {
        public int x, y;
        public Direction Direction;
        
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

        public static implicit operator Vector3Int(FeaturePosition f)
        {
            return new Vector3Int(f.x, f.y, 0);
        }
        public static implicit operator Vector3(FeaturePosition f)
        {
            return new Vector3(f.x, f.y, 0);
        }

        public string ToString()
        {
            return "(" + x + ", " + y + ", " + Direction + ")";
        }
    }
}