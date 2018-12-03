using System.Collections.Generic;
using DungeonGeneration;
using DungeonGeneration.Structs;
using UnityEngine;

namespace Pathfinding
{
    public class AstarPath
    {
        public Vector3Int Start, End;

        public List<Vector3Int> RoomPoints { get; }
        public bool Completed { private set; get; }
        public float Distance;

        private readonly List<Vector3Int> _cachedPath;
        private int _currentPoint;
        private int _nextRoom;

        private Astar _astar;
        private Dungeon _dungeon;

        public AstarPath(Dungeon dungeon, List<Vector3Int> roomPoints)
        {
            RoomPoints = roomPoints;
            _astar = dungeon.Astar;
            _dungeon = dungeon;
            _cachedPath = new List<Vector3Int>();
            _nextRoom = 0;
        }

        public AstarPath(List<Vector3Int> completePath)
        {
            _cachedPath = completePath;
            _nextRoom = 0;
        }

        public Vector3Int GetNextPoint()
        {
            if (RoomPoints != null)
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
                Completed = _currentPoint >= _cachedPath.Count - 1;

                return _cachedPath[_currentPoint++];
            }
            else
            {
                return Vector3Int.zero;
            }
        }

        public void StartOver()
        {
            _currentPoint = 0;
            Completed = false;
        }
    }
}