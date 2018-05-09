using System.Collections.Generic;
using Features;
using Interfaces;
using JetBrains.Annotations;
using Types;
using UnityEngine;

namespace FeatureFactories
{
    public class RoomFactory : IFeatureFactory
    {
        private readonly List<RoomData> _currentRooms = new List<RoomData>();

        public bool TryCreateFeature(FeaturePosition position, Vector2Int size, HashSet<Vector2Int> existingTiles, ref IFeature feature)
        {
            Vector2Int entrance = position;
            switch (position.Direction)
            {
                case Direction.Bottom:
                    position.x = position.x - Random.Range(1, size.x-1);
                    position.y = position.y+1;
                    break;
                case Direction.Right:
                    position.x = position.x+1;
                    position.y = position.y - Random.Range(1, size.y-1);
                    break;
                case Direction.Top:
                    position.x = position.x - Random.Range(1, size.x-1);
                    position.y = position.y - size.y;
                    break;
                case Direction.Left:
                    position.x = position.x - size.x;
                    position.y = position.y - Random.Range(1, size.y-1);
                    break;
            }

            if (CanCreateFeature(position, size, existingTiles))
            {
                // we aren't gonna check if we can add here, we just gonna add the room
                // even if it overlaps, checking should be done where this is called
//                RenderInfo r = new RenderInfo(_currentRooms.Count.ToString());
//                RoomData room = new RoomData(position, size, entrance,r);                
                RoomData room = new RoomData(position, size, entrance, GameSettings.Instance.roomRenderInfo);                
                _currentRooms.Add(room);
                feature = room;
                
                return true;
            }
            return false;
        }

        public bool CanCreateFeature(FeaturePosition position, Vector2Int size, HashSet<Vector2Int> existingTiles)
        {
            Vector2Int p = Vector2Int.zero - Vector2Int.one;
            size += Vector2Int.one*2;
            // Check if there is space for a new room
            for (int i = 0; i < size.x * size.y; i++)
            {
                // if we find a tile on the position we are checking that means
                // we cant add the room there since there's probably something
                // there already
                if (existingTiles.Contains(p + position)) return false;

                // increament the position we want to check after we've checked the
                // current position
                if (p.x == size.x - 1)
                {
                    p.x = 0;
                    p.y++;
                }
                else p.x++;
            }

            // we didnt find any existing tiles in the place we want to
            // add the room, so theres space to add it
            return true;
        }

        public List<CellData> GetCellsFromAllFeatures(HashSet<Vector2Int> existingTiles)
        {
            List<CellData> cells = new List<CellData>();
            foreach (RoomData room in _currentRooms)
            {
                cells.AddRange(room.GetCells(existingTiles));
            }

            return cells;
        }
    }
}