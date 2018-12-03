using System.Collections.Generic;
using System.Linq;
using DungeonGeneration;
using DungeonGeneration.Interfaces;
using DungeonGeneration.Structs;
using DungeonGeneration.Tiles;
using Singletons;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.XR.WSA.Persistence;

namespace Pathfinding
{
    public class Astar
    {
        private Dungeon _dungeon;

//        private AstarNode[,] _nodes;
        private HashSet<AstarNode> _checked;
        private SortedList<Vector3Int, AstarNode> _toCheck;

        private Dictionary<Vector3Int, AstarNode> _featureExits, _nodes;

        public void Initialize(Dungeon dungeon)
        {
            _dungeon = dungeon;
            _nodes = new Dictionary<Vector3Int, AstarNode>();
            _featureExits = new Dictionary<Vector3Int, AstarNode>();

            // loop through all the tiles
            Vector3Int tilepos = dungeon.Tilemap.origin;

            for (int i = 0; i < dungeon.Tilemap.size.x * dungeon.Tilemap.size.y; i++)
            {
                FloorTile tile = dungeon.Tilemap.GetTile<FloorTile>(tilepos);

                if (tile != null)
                {
                    var node = new AstarNode(tilepos, null, dungeon);

                    foreach (Vector3Int neighbour in dungeon.GetNeighboursByPosition(tilepos))
                    {
                        FloorTile neighbourTile = dungeon.Tilemap.GetTile<FloorTile>(neighbour);
                        if (neighbourTile != null)
                        {
                            node.Neighbours.Add(neighbour);
                        }
                    }

                    _nodes.Add(tilepos, node);
                }

                if (tilepos.x == dungeon.Tilemap.origin.x + dungeon.Tilemap.size.x - 1)
                {
                    tilepos.x = dungeon.Tilemap.origin.x;
                    tilepos.y++;
                }
                else tilepos.x++;
            }

            foreach (IFeature feature in dungeon.AllFeatures)
            {
                var exits = feature.GetExits();
                foreach (Vector3Int exit in exits)
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

                    _featureExits[exit].Neighbours = _featureExits[exit].Neighbours.Distinct().ToList();
                }
            }
        }

        public AstarPath GetPath(Vector3Int start, Vector3Int end)
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
                if (startFeature != endFeature)
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
                else
                {
                    
                }
            }


            return path;
        }

        public List<Vector3Int> GetPathBetweenPoints(Vector3Int start, Vector3Int end)
        {
            HashSet<Vector3Int> checkedNodes = new HashSet<Vector3Int>();
            List<AstarNode> uncheckedNodes = new List<AstarNode>();
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

        private List<AstarPath> GetAllPathsBetweenExits(List<Vector3Int> starts, List<Vector3Int> ends)
        {
            List<AstarPath> paths = new List<AstarPath>();
            foreach (Vector3Int start in starts)
            {
                foreach (Vector3Int end in ends)
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

        private AstarPath GetPathbetweenExits(Vector3Int start, Vector3Int end)
        {
            if (start == end)
            {
                return new AstarPath(_dungeon, new List<Vector3Int> {end});
            }

            HashSet<Vector3Int> checkedNodes = new HashSet<Vector3Int>();
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

            AstarPath path = new AstarPath(_dungeon, new List<Vector3Int>());
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