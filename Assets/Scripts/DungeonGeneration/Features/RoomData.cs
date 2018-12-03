using System;
using System.Collections.Generic;
using DungeonGeneration.Interfaces;
using DungeonGeneration.Structs;
using DungeonGeneration.Tiles;
using Enums;
using Extensions;
using Singletons;
using Structs;
using UnityEngine;
using UnityEngine.Tilemaps;
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
        public Bounds Bounds;

        public List<Vector3Int> Exits;

        // https://pastebin.com/38auMjRA

        private List<Direction> _directions;

        public RoomData(FeaturePosition position, Vector2Int size, Vector3Int entrance, Tilemap tilemap)
        {
            Position = position;
            Size = size;
            Exits = new List<Vector3Int> {entrance};
            Bounds = new Bounds();
            Bounds.SetMinMax(position, new Vector3(position.x + size.x, position.y + size.y));
            _directions = new List<Direction> {Direction.Bottom, Direction.Left, Direction.Right, Direction.Top};

            var intD = (int) position.Direction;
            if (position.Direction != Direction.None)
            {
                if (intD > 1)
                    _directions.Remove((Direction) intD - 2);
                else
                    _directions.Remove((Direction) intD + 2);
            }


            var tile = ScriptableObject.CreateInstance<FloorTile>();
            tile.colliderType = Tile.ColliderType.None;
            tile.color = GameSettings.Instance.FloorTileData.Color;
            tile.sprite = GameSettings.Instance.FloorTileData.Sprite;

            tilemap.SetTile(entrance, tile);
            // Add tiles to the tilemap
            Vector3Int tilepos = Vector3Int.zero;

            for (int i = 0; i < Size.x * Size.y; i++)
            {
                tile = ScriptableObject.CreateInstance<FloorTile>();
                tile.colliderType = Tile.ColliderType.None;
                tile.color = GameSettings.Instance.FloorTileData.Color;
                tile.sprite = GameSettings.Instance.FloorTileData.Sprite;

                tilemap.SetTile(position + tilepos, tile);

                if (tilepos.x == Size.x - 1)
                {
                    tilepos.x = 0;
                    tilepos.y++;
                }
                else tilepos.x++;
            }
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

            Vector3IntMinMax rand = new Vector3IntMinMax();
            Vector3Int pos = Vector3Int.zero;
            Direction d = _directions[Random.Range(0, _directions.Count)];
            switch (d)
            {
                case Direction.Top:
                    rand.Min = Bounds.TopLeft();
                    rand.Min.y += 1;

                    rand.Max = Bounds.TopRight();
                    rand.Max.y += 1;

                    pos = rand.GetRandomValue();
                    break;
                case Direction.Left:
                    rand.Min = Bounds.BottomLeft();
                    rand.Min.x -= 1;

                    rand.Max = Bounds.TopLeft();
                    rand.Max.x -= 1;

                    pos = rand.GetRandomValue();
                    break;
                case Direction.Right:
                    rand.Min = Bounds.BottomRight();
                    rand.Min.x += 1;

                    rand.Max = Bounds.TopRight();
                    rand.Max.x += 1;

                    pos = rand.GetRandomValue();
                    break;
                case Direction.Bottom:
                    rand.Min = Bounds.BottomLeft();
                    rand.Min.y -= 1;

                    rand.Max = Bounds.BottomRight();
                    rand.Max.y -= 1;

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

        public bool Contains(Vector3Int point)
        {
            return Bounds.Contains(point);
        }

        public List<Vector3Int> GetExits()
        {
            return Exits;
        }

        public void AddExit(Vector3Int e)
        {
            Exits.Add(e);
        }
    }
}