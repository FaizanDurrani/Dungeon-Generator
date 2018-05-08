using System;
using System.Collections.Generic;
using Types;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Features
{
    [Serializable]
    public struct RoomData
    {
        /// <summary>
        /// Starts from (0, 0).
        /// </summary>
        public Vector2Int Position;

        /// <summary>
        /// Will be greater than (0, 0). Never negative
        /// </summary>
        public Vector2Int Size;

        /// <summary>
        /// Bounds of this room
        /// </summary>
        public Rect Bounds;

        public RoomData(Vector2Int position, Vector2Int size) : this()
        {
            Size = size;
            Position = position;
            Bounds.Set(position.x, position.y, size.x, size.y);
        }

        public RoomData(CorridorData corridor, Vector2Int size) : this()
        {
            Size = size;
            CellPosition pos;
            switch (corridor.Direction)
            {
                case Direction.Bottom:
                    pos.X = corridor.EndPosition.x - Random.Range(0, size.x);
                    pos.Y = corridor.EndPosition.y;
                    break;
                case Direction.Right:
                    pos.X = corridor.EndPosition.x;
                    pos.Y = corridor.EndPosition.y - Random.Range(0, size.y);
                    break;
                case Direction.Top:
                    pos.X = corridor.EndPosition.x - Random.Range(0, size.x);
                    pos.Y = corridor.EndPosition.y - size.y + 1;
                    break;
                case Direction.Left:
                    pos.X = corridor.EndPosition.x - size.x + 1;
                    pos.Y = corridor.EndPosition.y - Random.Range(0, size.y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Position = pos.ToVector2Int();
            if (Position.x < 1)
                Position.x = 1;
            if (Position.y < 1)
                Position.y = 1;

            Bounds.Set(Position.x, Position.y, size.x, size.y);
        }

        public void AddCells(HashSet<Vector2Int> floor, List<CellData> cells, string representation, Color foreColor,
            Color backColor)
        {
            Vector2Int position = Vector2Int.zero;
            for (int i = 0; i < Size.x * Size.y; i++)
            {
                var cell = new CellData(position + Position,
                    0,
                    foreColor);

                if (!floor.Contains(cell.Position)) floor.Add(cell.Position);
                cells.Add(cell);
                //else cells.Add(cell.Position, new List<CellData> {cell});

                if (position.x == Size.x - 1)
                {
                    position.x = 0;
                    position.y++;
                }
                else position.x++;
            }
        }

        public CorridorData GetCorridor(int size, CorridorData lastCorridor)
        {
            RandomVector2Int rand = new RandomVector2Int();

            Direction d = (Direction) Random.Range(0, 4);
            switch (d)
            {
                case Direction.Top:
                    rand.Min = Bounds.TopLeft().ToVector2Int();
                    rand.Max = Bounds.TopRight().ToVector2Int();
                    break;
                case Direction.Left:
                    rand.Min = Bounds.BottomLeft().ToVector2Int();
                    rand.Max = Bounds.TopLeft().ToVector2Int();
                    break;
                case Direction.Right:
                    rand.Min = Bounds.BottomRight().ToVector2Int();
                    rand.Max = Bounds.TopRight().ToVector2Int();
                    break;
                case Direction.Bottom:
                    rand.Min = Bounds.BottomLeft().ToVector2Int();
                    rand.Max = Bounds.BottomRight().ToVector2Int();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var c = new CorridorData(rand.GetRandomValue(), size, d);

            if (c.EndPosition.x < 1 || c.EndPosition.y < 1 ||
                (lastCorridor.Direction != d && ((int) lastCorridor.Direction + (int) d) % 2 == 0))
                return GetCorridor(size, lastCorridor);
            return c;
        }
    }
}