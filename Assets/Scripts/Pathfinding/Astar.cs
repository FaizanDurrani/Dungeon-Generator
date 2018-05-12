using System.Collections.Generic;
using System.Linq;
using DungeonGeneration;
using DungeonGeneration.Interfaces;
using DungeonGeneration.Structs;
using Singletons;
using UnityEngine;
using UnityEngine.XR.WSA.Persistence;

namespace Pathfinding
{
    public class Astar
    {
        private Dungeon _dungeon;

//        private AstarNode[,] _nodes;
        private HashSet<AstarNode> _checked;
        private SortedList<Vector2Int, AstarNode> _toCheck;

        private Dictionary<Vector2Int, AstarNode> _featureExits, _nodes;

        public void Initialize(Dungeon dungeon)
        {
            _dungeon = dungeon;
            _nodes = new Dictionary<Vector2Int, AstarNode>();
            _featureExits = new Dictionary<Vector2Int, AstarNode>();

            Vector2Int position = Vector2Int.zero;
            for (int i = 0; i < dungeon.GridSize.x * dungeon.GridSize.y; i++)
            {
                CellData cell;
                if (dungeon.TryGetCell(position, out cell))
                {
                    var node = new AstarNode(position, null, dungeon);

                    foreach (Vector2Int neighbour in cell.Neighbours)
                    {
                        if (dungeon.TryGetCell(neighbour, out cell))
                        {
                            node.Neighbours.Add(neighbour);
                        }
                    }

                    _nodes.Add(position, node);
                }

                if (position.x == dungeon.GridSize.x - 1)
                {
                    position.x = 0;
                    position.y++;
                }
                else position.x++;
            }

            foreach (IFeature feature in dungeon.AllFeatures)
            {
                var exits = feature.GetExits();
                foreach (Vector2Int exit in exits)
                {
                    if (!_featureExits.ContainsKey(exit))
                    {
                        _featureExits.Add(exit, new AstarNode(exit, null, dungeon)
                        {
                            Neighbours = exits
                        });
                    }
                    else
                    {
                        _featureExits[exit].Neighbours.AddRange(exits);
                    }

                    var c = dungeon.GetCell(exit);
                    c.RenderInfo = GameSettings.Instance.debugRenderInfo1;
                    c.Discovered = true;
                    dungeon.SetCell(exit, c);
                    _featureExits[exit].Neighbours = _featureExits[exit].Neighbours.Distinct().ToList();
                }
            }
        }

        public AstarPath GetPath(Vector2Int start, Vector2Int end)
        {
            AstarPath path = null;
            IFeature startFeature = null, endFeature = null;

            foreach (IFeature feature in _dungeon.AllFeatures)
            {
                if (feature.Contains(start))
                {
                    startFeature = feature;
                }

                if (feature.Contains(end))
                {
                    endFeature = feature;
                }
            }

            if (startFeature != null && endFeature != null)
            {
                List<AstarPath> paths = GetAllPathsBetweenExits(startFeature.GetExits(), endFeature.GetExits());
                if (paths != null)
                {
                    paths.Sort((x, y) => x.Distance.CompareTo(y.Distance));
                    path = paths[0];
                    path.Start = start;
                    path.End = end;
                }
            }

            return path;
        }

        public List<Vector2Int> GetPathBetweenPoints(Vector2Int start, Vector2Int end)
        {
            HashSet<Vector2Int> checkedNodes = new HashSet<Vector2Int>();
            List<AstarNode> uncheckedNodes = new List<AstarNode>();
            CellData c = null;

            var toCheck = _nodes[start].Clone();

            toCheck.CalculateNeighbours(ref checkedNodes, ref uncheckedNodes, ref _nodes, end);

            checkedNodes.Add(start);
            uncheckedNodes.Sort((x, y) => x.CompareTo(y));

            AstarNode endNode = null;
            while (uncheckedNodes.Count > 0 && endNode == null)
            {
                toCheck = uncheckedNodes[0];
                uncheckedNodes.RemoveAt(0);


                if (checkedNodes.Contains(toCheck.Position))
                    continue;

                toCheck.CalculateNeighbours(ref checkedNodes, ref uncheckedNodes, ref _nodes, end);
                checkedNodes.Add(toCheck.Position);
                if (toCheck.Position == end)
                    endNode = toCheck;
                uncheckedNodes.Sort((x, y) => x.CompareTo(y));
            }

            float distance = 0;
            Debug.Log("End Node is " + endNode?.Position);
            var path = endNode?.GetPath(out distance);
            path?.Reverse();
            return path;
        }

        private List<AstarPath> GetAllPathsBetweenExits(List<Vector2Int> starts, List<Vector2Int> ends)
        {
            List<AstarPath> paths = new List<AstarPath>();
            foreach (Vector2Int start in starts)
            {
                foreach (Vector2Int end in ends)
                {
                    AstarPath path = GetPathbetweenExits(start, end);
                    if (path != null)
                    {
                        paths.Add(path);
                    }
                }
            }

            return paths.Count > 0 ? paths : null;
        }

        private AstarPath GetPathbetweenExits(Vector2Int start, Vector2Int end)
        {
            if (start == end)
            {
                return new AstarPath(_dungeon, new List<Vector2Int> {end});
            }

            HashSet<Vector2Int> checkedNodes = new HashSet<Vector2Int>();
            List<AstarNode> uncheckedNodes = new List<AstarNode>();

            var toCheck = _featureExits[start].Clone();
            toCheck.CalculateNeighbours(ref checkedNodes, ref uncheckedNodes, ref _featureExits, end);
            checkedNodes.Add(start);
            uncheckedNodes.Sort((x, y) => x.CompareTo(y));

            AstarNode endNode = null;
            while (uncheckedNodes.Count > 0 && endNode == null)
            {
                toCheck = uncheckedNodes[0];
                uncheckedNodes.RemoveAt(0);

                if (checkedNodes.Contains(toCheck.Position))
                    continue;

                toCheck.CalculateNeighbours(ref checkedNodes, ref uncheckedNodes, ref _featureExits, end);
                checkedNodes.Add(toCheck.Position);

                if (toCheck.Position == end) endNode = toCheck;
                uncheckedNodes.Sort((x, y) => x.CompareTo(y));
            }

            AstarPath path = new AstarPath(_dungeon, new List<Vector2Int>());
            if (endNode != null)
            {
                var pathFromNode = endNode.GetPath(out path.Distance);
                pathFromNode.Reverse();
                path.RoomPoints.AddRange(pathFromNode);
            }

            uncheckedNodes = null;
            checkedNodes = null;
            return endNode != null ? path : null;
        }
    }
}