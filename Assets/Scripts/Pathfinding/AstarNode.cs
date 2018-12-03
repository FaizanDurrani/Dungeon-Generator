using System;
using System.Collections.Generic;
using DungeonGeneration;
using DungeonGeneration.Structs;
using DungeonGeneration.Tiles;
using Singletons;
using UnityEngine;

namespace Pathfinding
{
    public class AstarNode
    {
        public Vector3Int Position;
        public AstarNode Parent;
        public float DistanceToStart;
        public float DistanceToEnd;
        public float TotalCost => DistanceToEnd + DistanceToStart;

        public List<Vector3Int> Neighbours;
        private Dungeon _dungeon;

        public AstarNode(Vector3Int position, AstarNode parent, Dungeon dungeon)
        {
            Position = position;
            Parent = parent;
            Neighbours = new List<Vector3Int>();
            _dungeon = dungeon;
        }

        public void CalculateNeighbours(ref HashSet<Vector3Int> checkedNodes, ref List<AstarNode> uncheckedNodes,
            ref Dictionary<Vector3Int, AstarNode> nodes, Vector3Int end)
        {
            if (Neighbours == null) return;
            foreach (var neighbourPos in Neighbours)
            {
                if (neighbourPos == Position ||
                    _dungeon.Tilemap.GetTile<FloorTile>(neighbourPos) == null ||
                    checkedNodes.Contains(Position))
                    continue;

                var neighbour = nodes[neighbourPos].Clone();
                if (neighbour.Parent != null)
                {
                    var temp = DistanceToStart + Vector3Int.Distance(Position, neighbour.Position);
                    if (neighbour.DistanceToStart > temp &&
                        neighbour.TotalCost >= temp + neighbour.DistanceToEnd)
                    {
                        neighbour.Parent = this;
                        neighbour.DistanceToStart = temp;
                        uncheckedNodes.Add(neighbour);
                    }
                }

                else if (neighbour.Parent == null)
                {
                    neighbour.Parent = this;
                    neighbour.DistanceToEnd = Vector3Int.Distance(neighbour.Position, end);
                    neighbour.DistanceToStart = DistanceToStart +
                                                Vector3Int.Distance(Position, neighbour.Position);
                    uncheckedNodes.Add(neighbour);
                }
            }
        }

        public List<Vector3Int> GetPath(out float distance)
        {
            var path = new List<Vector3Int> {Position};
            if (Parent != null)
            {
                path.AddRange(Parent.GetPath(out distance));
            }

            distance = DistanceToStart;
            return path;
        }

        public int CompareTo(AstarNode node)
        {
            if (Mathf.Abs(node.TotalCost - TotalCost) > Mathf.Epsilon)
            {
                return (TotalCost < node.TotalCost) ? -1 : 1;
            }

            return (DistanceToEnd < node.DistanceToEnd) ? -1 : 1;
        }

        public AstarNode Clone()
        {
            return new AstarNode(Position, Parent, _dungeon)
            {
                Neighbours = Neighbours,
                DistanceToEnd = DistanceToEnd,
                DistanceToStart = DistanceToStart
            };
        }
    }
}