using System.Collections.Generic;
using DungeonGeneration.Features;
using DungeonGeneration.Interfaces;
using DungeonGeneration.Structs;
using Enums;
using Singletons;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DungeonGeneration.FeatureFactories
{
    public class RoomFactory : IFeatureFactory
    {
        private readonly List<RoomData> _currentRooms = new List<RoomData>();

        public bool TryCreateFeature(FeaturePosition position, Vector2Int size, Tilemap tilemap, ref IFeature feature)
        {
            Vector3Int entrance = position;
            switch (position.Direction)
            {
                case Direction.Top:
                    position.x = position.x - Random.Range(1, size.x-1);
                    position.y = position.y+1;
                    break;
                case Direction.Right:
                    position.x = position.x+1;
                    position.y = position.y - Random.Range(1, size.y-1);
                    break;
                case Direction.Bottom:
                    position.x = position.x - Random.Range(1, size.x-1);
                    position.y = position.y - size.y;
                    break;
                case Direction.Left:
                    position.x = position.x - size.x;
                    position.y = position.y - Random.Range(1, size.y-1);
                    break;
            }

            if (CanCreateFeature(position, size, tilemap))
            {
                Debug.Log($"room at {position.ToString()}");
                // we aren't gonna check if we can add here, we just gonna add the room
                // even if it overlaps, checking should be done where this is called          
                RoomData room = new RoomData(position, size, entrance, tilemap);                
                _currentRooms.Add(room);
                feature = room;
                
                return true;
            }
            return false;
        }

        public bool CanCreateFeature(FeaturePosition position, Vector2Int size, Tilemap tilemap)
        {
            Vector3Int p = new Vector3Int(-1,-1, 0);
            size += Vector2Int.one*2;
            // Check if there is space for a new room
            for (int i = 0; i < size.x * size.y; i++)
            {
                // if we find a tile on the position we are checking that means
                // we cant add the room there since there's probably something
                // there already
                if (tilemap.HasTile(p + position))
                {
                    Debug.Log($"{p+position} {tilemap.GetTile(p+position).name}");
                    return false;
                }

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
    }
}