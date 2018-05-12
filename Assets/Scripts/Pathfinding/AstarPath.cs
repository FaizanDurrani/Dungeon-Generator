using System.Collections.Generic;
using DungeonGeneration;
using DungeonGeneration.Structs;
using UnityEngine;

namespace Pathfinding
{
    public class AstarPath
    {
        public Vector2Int Start, End;

        public List<Vector2Int> RoomPoints { private set; get; }
        public bool Completed { private set; get; }
        public bool Blocked { private set; get; }
        public float Distance;

        private readonly List<Vector2Int> _cachedPath;
        private int _currentPoint;
        private int _nextRoom;

        private Astar _astar;
        private Dungeon _dungeon;

        public AstarPath(Dungeon dungeon, List<Vector2Int> roomPoints)
        {
            RoomPoints = roomPoints;
            _astar = dungeon.Astar;
            _dungeon = dungeon;
            _cachedPath = new List<Vector2Int>();
            _nextRoom = 0;
        }

        public bool IsPathClear()
        {
            foreach (Vector2Int point in RoomPoints)
            {
                CellData c;
                if (!_dungeon.TryGetCell(point, out c) || !c.CanWalkOn)
                    return false;
            }

            return true;
        }

        public Vector2Int GetNextPoint()
        {
            if (_cachedPath.Count == 0)
            {
                //_nextRoom = RoomPoints.Count - 1;
                var temp = _astar.GetPathBetweenPoints(Start, RoomPoints[0]);
                temp.RemoveAt(0);
                _cachedPath.AddRange(temp);
                _nextRoom++;
            }
            else if (_currentPoint == _cachedPath.Count - 1)
            {
                if (_nextRoom <= RoomPoints.Count - 1)
                {
                    var temp = _astar.GetPathBetweenPoints(_cachedPath[_currentPoint], RoomPoints[_nextRoom]);
                    temp.RemoveAt(0);
                    _cachedPath.AddRange(temp);
                    _nextRoom++;
                }
                else if (_cachedPath[_currentPoint] == RoomPoints[RoomPoints.Count - 1])
                {
                    var temp = _astar.GetPathBetweenPoints(_cachedPath[_currentPoint], End);
                    temp.RemoveAt(0);
                    _cachedPath.AddRange(temp);
                    _nextRoom++;
                }
            }

//            Debug.Log($"Count: {_cachedPath.Count} Curr:{_currentPoint}");
            Completed = _currentPoint >= _cachedPath.Count - 1;

            return _cachedPath[_currentPoint++];
        }

        public void StartOver()
        {
            _currentPoint = 0;
            Completed = false;
        }
    }
}