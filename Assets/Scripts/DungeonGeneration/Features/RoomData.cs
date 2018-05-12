using System;
using System.Collections.Generic;
using DungeonGeneration.Interfaces;
using DungeonGeneration.Structs;
using Enums;
using Extensions;
using Structs;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DungeonGeneration.Features
{
    [Serializable]
    public struct RoomData : IFeature
    {
        /// <summary>
        /// Starts from (0, 0).
        /// </summary>
        public FeaturePosition Position;

        /// <summary>
        /// Will be greater than (0, 0). Never negative
        /// </summary>
        public Vector2Int Size;

        /// <summary>
        /// Rect of this room
        /// </summary>
        public Rect Rect;

        /// <summary>
        /// How this room will be rendered
        /// </summary>
        public RenderInfo RenderInfo;

        public List<Vector2Int> Exits;

        // https://pastebin.com/38auMjRA

        private List<Direction> _directions;

        public RoomData(FeaturePosition position, Vector2Int size, Vector2Int entrance, RenderInfo rInfo)
        {
            Position = position;
            RenderInfo = rInfo;
            Size = size;
            Exits = new List<Vector2Int> {entrance};
            Rect = new Rect();
            Rect.Set(Position.x, Position.y, size.x, size.y);
            _directions = new List<Direction> {Direction.Bottom, Direction.Left, Direction.Right, Direction.Top};

            var intD = (int) position.Direction;
            if (position.Direction != Direction.None)
            {
                if (intD > 1)
                    _directions.Remove((Direction) intD - 2);
                else
                    _directions.Remove((Direction) intD + 2);
            }
        }

        public List<CellData> GetCells(HashSet<Vector2Int> existingTiles)
        {
            List<CellData> cells = new List<CellData>();
            foreach (Vector2Int exit in Exits)
            {
                if (!existingTiles.Contains(exit))
                {
                    cells.Add(new CellData(exit, CellType.Floor, RenderInfo, true));
                    existingTiles.Add(exit);
                }
            }

            Vector2Int position = Vector2Int.zero;

            for (int i = 0; i < Size.x * Size.y; i++)
            {
                var cell = new CellData(position + Position,
                    CellType.Floor, RenderInfo, true);

                existingTiles.Add(cell.Position);
                cells.Add(cell);

                if (position.x == Size.x - 1)
                {
                    position.x = 0;
                    position.y++;
                }
                else position.x++;
            }

            return cells;
        }

        public int GetNewFeatureCount()
        {
            return _directions.Count;
        }

        public FeaturePosition GetNewFeaturePosition()
        {
            /*  # = possible position to return
                F = actual room floor
                           FFFFFF
                          F000000F
                          F000000F
                          F000000F
                          F000000F
                           FFFFFF                 */
            
            Vector2IntMinMax rand = new Vector2IntMinMax();
            Vector2Int pos = Vector2Int.zero;
            Direction d = _directions[Random.Range(0, _directions.Count)];
            switch (d)
            {
                case Direction.Top:
                    rand.Min = Rect.TopLeft().ToVector2Int();
                    rand.Min.y -= 1;

                    rand.Max = Rect.TopRight().ToVector2Int();
                    rand.Max.y -= 1;

                    pos = rand.GetRandomValue();
                    break;
                case Direction.Left:
                    rand.Min = Rect.BottomLeft().ToVector2Int();
                    rand.Min.x -= 1;

                    rand.Max = Rect.TopLeft().ToVector2Int();
                    rand.Max.x -= 1;

                    pos = rand.GetRandomValue();
                    break;
                case Direction.Right:
                    rand.Min = Rect.BottomRight().ToVector2Int();
                    rand.Min.x += 1;

                    rand.Max = Rect.TopRight().ToVector2Int();
                    rand.Max.x += 1;

                    pos = rand.GetRandomValue();
                    break;
                case Direction.Bottom:
                    rand.Min = Rect.BottomLeft().ToVector2Int();
                    rand.Min.y += 1;

                    rand.Max = Rect.BottomRight().ToVector2Int();
                    rand.Max.y += 1;

                    pos = rand.GetRandomValue();
                    break;
            }

            _directions.Remove(d);
            return new FeaturePosition(pos.x, pos.y, d);
        }

        public bool CanMakeNewFeature()
        {
            return _directions.Count > 0;
        }

        public bool Contains(Vector2Int point)
        {
            return Rect.Contains(point);
        }

        public List<Vector2Int> GetExits()
        {
            return Exits;
        }

        public void AddExit(Vector2Int e)
        {
            Exits.Add(e);
        }

        public void SetPosition(Vector2Int position)
        {
            for (int i = 0; i < Exits.Count; i++)
            {
                Exits[i] -= Position - position;
            }
            
            Position.x = position.x;
            Position.y = position.y;
            Rect.position = position;
        }

        public Vector2Int GetPosition()
        {
            return Position;
        }
    }
}