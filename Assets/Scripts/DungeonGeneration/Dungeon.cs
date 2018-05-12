using System;
using System.Collections.Generic;
using DungeonGeneration.FeatureFactories;
using DungeonGeneration.Interfaces;
using DungeonGeneration.Structs;
using Enums;
using Extensions;
using Pathfinding;
using Singletons;
using Structs;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DungeonGeneration
{
    [Serializable]
    public class Dungeon
    {
        public Vector2IntMinMax RoomSize { get; }
        public int FeatureCount { get; }
        public CellData[,] AllCells { private set; get; }
        public Vector2Int GridSize { private set; get; }
        public Vector2Int StartPos { private set; get; }
        public List<IFeature> AllFeatures { private set; get; }
        public Astar Astar { private set; get; }

        private int _currFeatureCount = 0;
        private Vector2IntMinMax _grid;
        private HashSet<Vector2Int> _floorPositions;

        // Temporary vars, cleared after they are used
        private Queue<IFeature> _featureQueue;
        private List<CellData> _floorCells;
        private List<IFeatureFactory> _factories;

        public Dungeon(Vector2IntMinMax roomSize, int features)
        {
            FeatureCount = features;
            RoomSize = roomSize;
        }

        public void GenerateDungeon()
        {
            _floorPositions = new HashSet<Vector2Int>();
            _floorCells = new List<CellData>();
            _factories = new List<IFeatureFactory> {new RoomFactory()};
            _featureQueue = new Queue<IFeature>();

            // First pass
            GenerateFeatures();
            // Second pass
            GenerateWalls();
            // Thrid pass
            CalculateGrid();

            Astar = new Astar();
            Astar.Initialize(this);

            _floorCells = null;
            _factories = null;
//            _features = null;
        }

        public bool CellExists(Vector2Int position)
        {
            return position.x >= 0 &&
                   position.y >= 0 &&
                   position.x < AllCells.GetLength(0) &&
                   position.y < AllCells.GetLength(1) &&
                   GetCell(position)?.GetRenderInfo().Representation != null;
        }

        public bool TryGetCell(Vector2Int position, out CellData c)
        {
            if (position.x >= 0 &&
                position.y >= 0 &&
                position.x < AllCells.GetLength(0) &&
                position.y < AllCells.GetLength(1))
            {
                c = AllCells[position.x, position.y];
                return c?.GetRenderInfo().Representation != null;
            }

            c = null;
            return false;
        }

        public void SetAllCells(CellData[,] allCells)
        {
            AllCells = allCells;
        }

        public void SetCell(Vector2Int pos, CellData cell)
        {
            AllCells[pos.x, pos.y] = cell;
        }

        public CellData[,] GetAllCells()
        {
            return AllCells;
        }

        public CellData GetCell(Vector2Int pos)
        {
            return AllCells[pos.x, pos.y];
        }

        private void CalculateGrid()
        {
            // Get the grid size
            GridSize = _grid.Max - _grid.Min;
            // Initialize the array that will hold all the cells
            AllCells = new CellData[GridSize.x + 1, GridSize.y + 1];

            // Offset the start Position
            StartPos -= _grid.Min;

            // loop over all the tiles...
            for (var index = 0; index < _floorCells.Count; index++)
            {
                var cellData = _floorCells[index];
                cellData.Position -= _grid.Min;
                cellData.RefreshNeighbourPositions();
                AllCells[cellData.Position.x, cellData.Position.y] = cellData;
            }

            // Offset all the features
            foreach (IFeature feature in AllFeatures)
            {
                feature.SetPosition(feature.GetPosition() - _grid.Min);
            }

            // Offset the grid
            _grid.Min -= _grid.Min;
            _grid.Max -= _grid.Min;
        }

        private void GenerateFeatures()
        {
            AllFeatures = new List<IFeature>();
            _currFeatureCount = 0;
            // start top left
            FeaturePosition position = new FeaturePosition(1, 1, Direction.None);
            Vector2Int size = RoomSize.GetRandomValue();
            IFeature tempFeature = null;

            StartPos = position + new Vector2Int(size.x / 2, size.y / 2);

            _factories[0].TryCreateFeature(position, size, _floorPositions, ref tempFeature);
            _floorCells.AddRange(tempFeature.GetCells(_floorPositions));
            _featureQueue.Enqueue(tempFeature);
            AllFeatures.Add(tempFeature);

            _currFeatureCount++;

            while (_featureQueue.Count > 0 && _currFeatureCount < FeatureCount)
            {
                var lastFeature = _featureQueue.Dequeue();

                position = lastFeature.GetNewFeaturePosition();
                size = RoomSize.GetRandomValue();

                IFeatureFactory factory = _factories[Random.Range(0, _factories.Count)];
                IFeature newFeature = null;
                if (factory.TryCreateFeature(position, size, _floorPositions, ref newFeature))
                {
                    lastFeature.AddExit(position);
                    AllFeatures.Add(newFeature);

                    var cells = newFeature.GetCells(_floorPositions);
                    _floorCells.AddRange(cells);
                    _featureQueue.Enqueue(newFeature);
                    _currFeatureCount++;
                }

                if (lastFeature.CanMakeNewFeature())
                    _featureQueue.Enqueue(lastFeature);
            }
        }

        private void GenerateWalls()
        {
            _grid.Min = new Vector2Int(0, 0);
            _grid.Max = new Vector2Int(0, 0);
            // loop through all the tiles
            var temp = new List<CellData>(_floorCells);
            foreach (var cellData in temp)
            {
                // check if the current tile is of type "floor"
                if (cellData.Type == CellType.Floor)
                {
                    foreach (var neighbour in cellData.Neighbours)
                    {
                        // if there's no tile on the left side of the current tile...
                        if (!_floorPositions.Contains(neighbour))
                        {
                            // ...make a wall there
                            var wall = MakeWall(neighbour);

                            // MAX
                            if (wall.Position.x > _grid.Max.x)
                                _grid.Max.x = wall.Position.x;

                            if (wall.Position.y > _grid.Max.y)
                                _grid.Max.y = wall.Position.y;

                            // MIN
                            if (wall.Position.x < _grid.Min.x)
                                _grid.Min.x = wall.Position.x;

                            if (wall.Position.y < _grid.Min.y)
                                _grid.Min.y = wall.Position.y;
                        }
                    }
                }


                // MAX
                if (cellData.Position.x > _grid.Max.x)
                    _grid.Max.x = cellData.Position.x;

                if (cellData.Position.y > _grid.Max.y)
                    _grid.Max.y = cellData.Position.y;

                // MIN
                if (cellData.Position.x < _grid.Min.x)
                    _grid.Min.x = cellData.Position.x;

                if (cellData.Position.y < _grid.Min.y)
                    _grid.Min.y = cellData.Position.y;
            }
        }

        private CellData MakeWall(Vector2Int position)
        {
            var wallData = new CellData(position,
                CellType.Wall,
                GameSettings.Instance.wallRenderInfo, false);

            _floorCells.Add(wallData);
            _floorPositions.Add(position);

            return wallData;
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