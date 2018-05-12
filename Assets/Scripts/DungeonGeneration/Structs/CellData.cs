using System;
using System.Collections.Generic;
using Enums;
using UnityEngine;
using Display = PhiOS.Scripts.PhiOS.Display;

namespace DungeonGeneration.Structs
{
    [Serializable]
    public class CellData
    {
        public Vector2Int Position;
        public string Description;
        public CellType Type;
        public RenderInfo RenderInfo;
        private RenderInfo _lastSeen;
        public bool Visible;
        public bool Discovered;
        public bool CanWalkOn;

        public List<Vector2Int> Neighbours;
        public Vector2Int Left => Position + Vector2Int.left;
        public Vector2Int Top => Position - Vector2Int.up;
        public Vector2Int Right => Position + Vector2Int.right;
        public Vector2Int Bottom => Position - Vector2Int.down;

        public Vector2Int TopLeft => Position + Vector2Int.left - Vector2Int.up;
        public Vector2Int TopRight => Position + Vector2Int.right - Vector2Int.up;
        public Vector2Int BottomRight => Position + Vector2Int.right - Vector2Int.down;
        public Vector2Int BottomLeft => Position + Vector2Int.left - Vector2Int.down;

        public CellData(Vector2Int position, CellType type, RenderInfo renderInfo, bool canWalkOn)
        {
            Discovered = true;
            Visible = false;
            CanWalkOn = canWalkOn;
            Position = position;
            Type = type;
            RenderInfo = new RenderInfo(renderInfo);
            Description = "";
            Neighbours = new List<Vector2Int> {Left, Top, Right, Bottom, TopLeft, TopRight, BottomLeft, BottomRight};
            _lastSeen = new RenderInfo(renderInfo);
        }

        public void RefreshNeighbourPositions()
        {
            Neighbours = new List<Vector2Int> {Left, Top, Right, Bottom, TopLeft, TopRight, BottomLeft, BottomRight};
        }

        public RenderInfo GetRenderInfo()
        {
            if (!Visible)
            {
                RenderInfo r = new RenderInfo(_lastSeen);
                if (Discovered)
                {
                    r.BackColor = _lastSeen.BackColor * _lastSeen.HiddenTint;
                    r.ForeColor = _lastSeen.ForeColor * _lastSeen.HiddenTint;
                }
                else
                {
                    r.BackColor = Display.GET.clearColor;
                    r.ForeColor = Display.GET.clearColor;
                }
                return r;
            }

            _lastSeen = new RenderInfo(RenderInfo);
            return _lastSeen;
        }
    }
}