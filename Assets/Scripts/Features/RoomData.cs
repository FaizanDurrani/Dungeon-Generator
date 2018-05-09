using System;
using System.Collections.Generic;
using Interfaces;
using Types;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Features
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
        public Rect Bounds;

        /// <summary>
        /// How this room will be rendered
        /// </summary>
        public RenderInfo RenderInfo;

        public Vector2Int Entrance;

        // https://pastebin.com/38auMjRA

        private List<Direction> _directions;

        public RoomData(FeaturePosition position, Vector2Int size, Vector2Int entrance, RenderInfo rInfo)
        {
            Position = position;
            RenderInfo = rInfo;
            Size = size;
            Entrance = entrance;
            Bounds = new Rect();
            Bounds.Set(Position.x, Position.y, size.x, size.y);
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
            List<CellData> cells = new List<CellData> {new CellData(Entrance, CellType.Floor, RenderInfo)};
            if (!existingTiles.Contains(Entrance)) existingTiles.Add(Entrance);

            Vector2Int position = Vector2Int.zero;

            for (int i = 0; i < Size.x * Size.y; i++)
            {
                var cell = new CellData(position + Position,
                    CellType.Floor, RenderInfo);

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
            /*
             * # = possible position to return
             * F = actual room floor
             * 
             *            
             *            FFFFFF
             *           F000000F
             *           F000000F
             *           F000000F
             *           F000000F
             *            FFFFFF
             *           
             */

            Vector2IntMinMax rand = new Vector2IntMinMax();
            Vector2Int pos = Vector2Int.zero;
//            Direction d = Direction.Right;
            Direction d = _directions[Random.Range(0, _directions.Count)];
            switch (d)
            {
                case Direction.Top:
                    rand.Min = Bounds.TopLeft().ToVector2Int();
                    rand.Min.y -= 1;

                    rand.Max = Bounds.TopRight().ToVector2Int();
                    rand.Max.y -= 1;

                    pos = rand.GetRandomValue();
                    break;
                case Direction.Left:
                    rand.Min = Bounds.BottomLeft().ToVector2Int();
                    rand.Min.x -= 1;

                    rand.Max = Bounds.TopLeft().ToVector2Int();
                    rand.Max.x -= 1;

                    pos = rand.GetRandomValue();
                    break;
                case Direction.Right:
                    rand.Min = Bounds.BottomRight().ToVector2Int();
                    rand.Min.x += 1;

                    rand.Max = Bounds.TopRight().ToVector2Int();
                    rand.Max.x += 1;

                    pos = rand.GetRandomValue();
                    break;
                case Direction.Bottom:
                    rand.Min = Bounds.BottomLeft().ToVector2Int();
                    rand.Min.y += 1;

                    rand.Max = Bounds.BottomRight().ToVector2Int();
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
    }
}