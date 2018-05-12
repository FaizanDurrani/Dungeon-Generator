using System;
using System.Collections.Generic;
using DungeonGeneration.Structs;
using Enums;
using UnityEngine;

namespace DungeonGeneration.Features
{
    public struct CorridorData
    {
        public readonly Vector2Int StartPosition;
        public readonly int Size;
        public readonly Direction Direction;
    
        public Vector2Int EndPosition
        {
            get
            {
                switch (Direction)
                {
                    case Direction.Bottom:
                        return new Vector2Int(StartPosition.x, StartPosition.y + Size -1);
                    case Direction.Right:
                        return new Vector2Int(StartPosition.x + Size -1, StartPosition.y);
                    case Direction.Top:
                        return new Vector2Int(StartPosition.x, StartPosition.y - Size+1);
                    case Direction.Left:
                        return new Vector2Int(StartPosition.x - Size+1, StartPosition.y);
                    default:
                        throw new ArgumentOutOfRangeException();
                } 
            }
        }
    
        public CorridorData(Vector2Int startPosition, int size, Direction direction)
        {
            StartPosition = startPosition;
            Size = size;
            Direction = direction;
        }
    
        public void AddCells(HashSet<Vector2Int> floor, List<CellData> cells)
        {
            for (int i = 0; i < Size; i++)
            {
                Vector2Int position;
                switch (Direction)
                {
                    case Direction.Bottom:
                        position = new Vector2Int(StartPosition.x, StartPosition.y + i);
                        break;
                    case Direction.Right:
                        position = new Vector2Int(StartPosition.x + i, StartPosition.y);
                        break;
                    case Direction.Top:
                        position = new Vector2Int(StartPosition.x, StartPosition.y - i);
                        break;
                    case Direction.Left:
                        position = new Vector2Int(StartPosition.x - i, StartPosition.y);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

//                var cell = new CellData(position, 0, foreColor);

                if (!floor.Contains(position)) floor.Add(position);
//                cells.Add(cell);
                //else cells.Add(position, new List<CellData>{cell});
            }
        }
    }
}