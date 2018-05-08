using System;
using UnityEngine;

namespace Types
{
    [Serializable]
    public struct CellData
    {
        public readonly Vector2Int Position;
        public readonly string Description;
        public readonly CellType Type;
        public readonly RenderInfo RenderInfo;

        public Vector2Int Left => Position + Vector2Int.left;
        public Vector2Int Top => Position - Vector2Int.up;
        public Vector2Int Right => Position + Vector2Int.right;
        public Vector2Int Bottom => Position - Vector2Int.down;

        public CellData(Vector2Int position, CellType type, RenderInfo renderInfo)
        {
            Position = position;
            Type = type;
            RenderInfo = renderInfo;
            Description = "";
        }
    }
}