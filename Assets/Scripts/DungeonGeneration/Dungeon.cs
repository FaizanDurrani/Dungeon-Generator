using System;
using System.Collections.Generic;
using DungeonGeneration.FeatureFactories;
using DungeonGeneration.Interfaces;
using DungeonGeneration.Structs;
using DungeonGeneration.Tiles;
using Enums;
using Extensions;
using Pathfinding;
using Singletons;
using Structs;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace DungeonGeneration
{
    [Serializable]
    public class Dungeon
    {
        public Vector3IntMinMax RoomSize { get; }
        public int FeatureCount { get; }
        public List<IFeature> AllFeatures { private set; get; }
        public Astar Astar { private set; get; }
        public Tilemap Tilemap { private set; get; }
        public Vector3Int StartPos { private set; get; }

        private int _currFeatureCount = 0;

        // Temporary vars, cleared after they are used
        private Queue<IFeature> _featureQueue;
        private List<IFeatureFactory> _factories;

        public Dungeon(Vector3IntMinMax roomSize, int features, Tilemap t)
        {
            FeatureCount = features;
            RoomSize = roomSize;
            Tilemap = t;
        }

        public void GenerateDungeon()
        {
            _factories = new List<IFeatureFactory> {new RoomFactory()};
            _featureQueue = new Queue<IFeature>();

            // First pass
            GenerateFeatures();
            // Second pass
            GenerateWalls();

            Astar = new Astar();
            Astar.Initialize(this);

            _factories = null;
        }

        private void GenerateFeatures()
        {
            AllFeatures = new List<IFeature>();
            _currFeatureCount = 0;
            // start top left
            FeaturePosition position = new FeaturePosition(1, 1, Direction.None);
            Vector3Int size = RoomSize.GetRandomValue();
            IFeature tempFeature = null;

            StartPos = position + new Vector3Int(size.x / 2, size.y / 2, 0);

            _factories[0].TryCreateFeature(position, size.ToVector2Int(), Tilemap, ref tempFeature);

            _featureQueue.Enqueue(tempFeature);
            AllFeatures.Add(tempFeature);

            _currFeatureCount++;

            while (_featureQueue.Count > 0 && _currFeatureCount < FeatureCount)
            {
                var lastFeature = _featureQueue.Dequeue();

                position = lastFeature.GetNewFeaturePosition();
                size = RoomSize.GetRandomValue();
                Debug.Log($"feature at {position.ToString()} of size {size}");

                IFeatureFactory factory = _factories[Random.Range(0, _factories.Count)];
                IFeature newFeature = null;
                if (factory.TryCreateFeature(position, size.ToVector2Int(), Tilemap, ref newFeature))
                {
                    lastFeature.AddExit(position);
                    AllFeatures.Add(newFeature);


                    _featureQueue.Enqueue(newFeature);
                    _currFeatureCount++;
                }

                if (lastFeature.CanMakeNewFeature())
                    _featureQueue.Enqueue(lastFeature);
            } 
        }

        private void GenerateWalls()
        {
            // loop through all the tiles
            Vector3Int tilepos = Tilemap.origin;

            for (int i = 0; i < Tilemap.size.x * Tilemap.size.y; i++)
            {
                FloorTile tile = Tilemap.GetTile<FloorTile>(tilepos);

                if (tile != null)
                {
                    foreach (var neighbour in GetNeighboursByPosition(tilepos))
                    {
                        FloorTile neighbourTile = Tilemap.GetTile<FloorTile>(neighbour);
                        // if there's no tile on the left side of the current tile...
                        if (neighbourTile == null)
                        {
                            var floorTile = ScriptableObject.CreateInstance<WallTile>();
                            floorTile.colliderType = Tile.ColliderType.None;
                            floorTile.color =  GameSettings.Instance.HiddenColor;
                            floorTile.sprite = GameSettings.Instance.WallTileData.Sprite;
                            
                            Tilemap.SetTile(neighbour, floorTile);
                        }
                    }
                }

                if (tilepos.x == Tilemap.origin.x + Tilemap.size.x - 1)
                {
                    tilepos.x = Tilemap.origin.x;
                    tilepos.y++;
                }
                else tilepos.x++;
            }
        }

        public List<Vector3Int> GetNeighboursByPosition(Vector3Int position)
        {
            var left = position + Vector3Int.left;
            var top = position + Vector3Int.up;
            var right = position + Vector3Int.right;
            var bottom = position + Vector3Int.down;
            var topLeft = position + Vector3Int.left + Vector3Int.up;
            var topRight = position + Vector3Int.right + Vector3Int.up;
            var bottomRight = position + Vector3Int.right + Vector3Int.down;
            var bottomLeft = position + Vector3Int.left + Vector3Int.down;
            return new List<Vector3Int> {left, top, right, bottom, topLeft, topRight, bottomRight, bottomLeft};
        }

//        private List<CellData> BoxSweep(Rect rect)
//        {
//            List<CellData> cells = new List<CellData>();
//            var clamppedRect = rect.Clamp((Rect) _grid);
//
//            var position = clamppedRect.position.ToVector2Int();
//            for (int i = 0; i < clamppedRect.size.x * clamppedRect.size.y; i++)
//            {
//                cells.Add();
//                
//                
//                if (position.x == (int) clamppedRect.xMax)
//                {
//                    position.x = (int) clamppedRect.xMin;
//                    position.y++;
//                }
//                else position.x++;
//            }
//        }
    }
}