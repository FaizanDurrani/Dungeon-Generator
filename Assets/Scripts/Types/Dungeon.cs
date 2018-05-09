using System;
using System.Collections.Generic;
using FeatureFactories;
using Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Types
{
    [Serializable]
    public class Dungeon
    {
        public Vector2IntMinMax RoomSize { get; }
        public int FeatureCount { get; }
        public List<CellData>[,] AllCells { private set; get; }
        public Vector2Int GridSize { private set; get; }
        public Vector2Int StartPos { private set; get; }

        private int _currFeatureCount = 0;
        private Vector2IntMinMax _grid;
        private HashSet<Vector2Int> _floorPositions;

        // Temporary vars, cleared after they are used
        private List<CellData> _floorCells;
        private Queue<IFeature> _features;
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
            _features = new Queue<IFeature>();

            // First pass
            GenerateFeatures();
            // Second pass
            GenerateWalls();
            // Thrid pass
            CalculateGrid();

            _floorCells = null;
            _factories = null;
            _features = null;
        }

        public bool TileExists(Vector2Int position)
        {
            var exists = position.x >= 0 &&
                         position.y >= 0 &&
                         position.x < AllCells.GetLength(0) &&
                         position.y < AllCells.GetLength(1) &&
                         AllCells[position.x, position.y] != null &&
                         AllCells[position.x, position.y].Count > 0;

            return exists;
        }

        public void SetAllCells(List<CellData>[,] allCells)
        {
            AllCells = allCells;
        }

        public void SetCells(Vector2Int pos, List<CellData> cells)
        {
            AllCells[pos.x, pos.y] = cells;
        }

        public void SetCellAtIndex(Vector2Int pos, int index, CellData cell)
        {
            AllCells[pos.x, pos.y][index] = cell;
        }

        public void SetLastCell(Vector2Int pos, CellData cell)
        {
            AllCells[pos.x, pos.y][AllCells[pos.x, pos.y].Count - 1] = cell;
        }

        public List<CellData>[,] GetAllCells()
        {
            return AllCells;
        }

        public List<CellData> GetCells(Vector2Int pos)
        {
            return AllCells[pos.x, pos.y];
        }

        public CellData GetCellAtIndex(Vector2Int pos, int index)
        {
            return AllCells[pos.x, pos.y][index];
        }

        public CellData GetLastCell(Vector2Int pos)
        {
            return AllCells[pos.x, pos.y][AllCells[pos.x, pos.y].Count - 1];
        }

        private void CalculateGrid()
        {
            GridSize = _grid.Max - _grid.Min;
            AllCells = new List<CellData>[GridSize.x + 1, GridSize.y + 1];
            StartPos -= _grid.Min;

            // loop over all the tiles...
            for (var index = 0; index < _floorCells.Count; index++)
            {
                var cellData = _floorCells[index];
                var cellsOnPos = AllCells[cellData.Position.x - _grid.Min.x, cellData.Position.y - _grid.Min.y];

                // ...add them to the array
                if (cellsOnPos == null) // The list is initialized
                    cellsOnPos = new List<CellData>();

                cellsOnPos.Add(new CellData(
                    new Vector2Int(cellData.Position.x - _grid.Min.x,
                        cellData.Position.y - _grid.Min.y),
                    cellData.Type,
                    cellData.RenderInfo));

                AllCells[cellData.Position.x - _grid.Min.x, cellData.Position.y - _grid.Min.y] = cellsOnPos;
            }
        }

        private void GenerateFeatures()
        {
            _currFeatureCount = 0;
            // start top left
            FeaturePosition position = new FeaturePosition(1, 1, Direction.None);
            Vector2Int size = RoomSize.GetRandomValue();
            IFeature tempFeature = null;

            StartPos = position + new Vector2Int(size.x / 2, size.y / 2);

            _factories[0].TryCreateFeature(position, size, _floorPositions, ref tempFeature);
            _floorCells.AddRange(tempFeature.GetCells(_floorPositions));
            _features.Enqueue(tempFeature);

            _currFeatureCount++;

            while (_features.Count > 0 && _currFeatureCount < FeatureCount)
            {
                var lastFeature = _features.Dequeue();

                position = lastFeature.GetNewFeaturePosition();
                size = RoomSize.GetRandomValue();

                IFeatureFactory factory = _factories[Random.Range(0, _factories.Count)];
                IFeature newFeature = null;
                if (factory.TryCreateFeature(position, size, _floorPositions, ref newFeature))
                {
                    var cells = newFeature.GetCells(_floorPositions);
                    _floorCells.AddRange(cells);
                    _features.Enqueue(newFeature);
                    _currFeatureCount++;
                }

                if (lastFeature.CanMakeNewFeature())
                    _features.Enqueue(lastFeature);
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
                GameSettings.Instance.wallRenderInfo);

            _floorCells.Add(wallData);
            _floorPositions.Add(position);

            return wallData;
        }
    }
}