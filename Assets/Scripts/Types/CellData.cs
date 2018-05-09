using System;
using System.Collections.Generic;
using UnityEngine;
using Display = PhiOS.Scripts.PhiOS.Display;

namespace Types
{
    [Serializable]
    public struct CellData
    {
        public Vector2Int Position;
        public string Description;
        public CellType Type;
        public RenderInfo RenderInfo;
        public RenderInfo LastSeen;
        public bool Visible;
        public bool Discovered;

        public List<Vector2Int> Neighbours;
        public Vector2Int Left => Position + Vector2Int.left;
        public Vector2Int Top => Position - Vector2Int.up;
        public Vector2Int Right => Position + Vector2Int.right;
        public Vector2Int Bottom => Position - Vector2Int.down;

        public Vector2Int TopLeft => Position + Vector2Int.left - Vector2Int.up;
        public Vector2Int TopRight => Position + Vector2Int.right - Vector2Int.up;
        public Vector2Int BottomRight => Position + Vector2Int.right - Vector2Int.down;
        public Vector2Int BottomLeft => Position + Vector2Int.left - Vector2Int.down;

        public CellData(Vector2Int position, CellType type, RenderInfo renderInfo) : this()
        {
            Position = position;
            Type = type;
            RenderInfo = renderInfo;
            Description = "";
            Neighbours = new List<Vector2Int> {Left, Top, Right, Bottom, TopLeft, TopRight, BottomLeft, BottomRight};
        }

        public RenderInfo GetRenderInfo()
        {
            if (!Visible)
            {
                RenderInfo r = new RenderInfo(LastSeen);

                if (Discovered)
                {
                    r.BackColor *= r.HiddenTint;
                    r.ForeColor *= r.HiddenTint;
                }
                else
                {
                    r.BackColor = Display.GET.clearColor;
                    r.ForeColor = Display.GET.clearColor;
                }
                return r;
            }

            LastSeen = new RenderInfo(RenderInfo);
            return RenderInfo;
        }
    }
}