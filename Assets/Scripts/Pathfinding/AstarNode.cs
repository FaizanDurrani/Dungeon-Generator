using System;
using System.Collections.Generic;
using DungeonGeneration;
using DungeonGeneration.Structs;
using Singletons;
using UnityEngine;

namespace Pathfinding
{
    public class AstarNode
    {
        public Vector2Int Position;
        public AstarNode Parent;
        public float DistanceToStart;
        public float DistanceToEnd;
        public float TotalCost => DistanceToEnd + DistanceToStart;

        public List<Vector2Int> Neighbours;
        private Dungeon _dungeon;

        public AstarNode(Vector2Int position, AstarNode parent, Dungeon dungeon)
        {
            Position = position;
            Parent = parent;
            Neighbours = new List<Vector2Int>();
            _dungeon = dungeon;
        }

        public void CalculateNeighbours(ref HashSet<Vector2Int> checkedNodes, ref List<AstarNode> uncheckedNodes,
            ref Dictionary<Vector2Int, AstarNode> nodes, Vector2Int end)
        {
            if (Neighbours == null) return;
            foreach (var neighbourPos in Neighbours)
            {
                if (neighbourPos == Position ||
                    neighbourPos.x < 0 ||
                    neighbourPos.y < 0 ||
                    !_dungeon.CellExists(neighbourPos) ||
                    !_dungeon.GetCell(neighbourPos).CanWalkOn ||
                    checkedNodes.Contains(Position))
                    continue;

                var neighbour = nodes[neighbourPos].Clone();
                if (neighbour.Parent != null)
                {
                    var temp = DistanceToStart + Vector2Int.Distance(Position, neighbour.Position);
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
                    neighbour.DistanceToEnd = Vector2Int.Distance(neighbour.Position, end);
                    neighbour.DistanceToStart = DistanceToStart +
                                                Vector2Int.Distance(Position, neighbour.Position);
                    uncheckedNodes.Add(neighbour);
                }
            }
        }

        public List<Vector2Int> GetPath(out float distance)
        {
            var path = new List<Vector2Int> {Position};
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