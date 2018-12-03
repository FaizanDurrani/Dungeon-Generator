using System.Collections;
using System.Collections.Generic;
using DungeonGeneration;
using DungeonGeneration.Structs;
using DungeonGeneration.Tiles;
using Enums;
using Extensions;
using Pathfinding;
using Rendering;
using Structs;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Singletons
{
    public class Player : Singleton<Player>
    {
        public bool CanControl;
        public int FieldOfView;

        private AstarPath _path;
        private HashSet<Vector3Int> _visibleTiles;

        public void Spawn(Vector3Int pos)
        {
            transform.position = pos;
            CanControl = true;
            SetFieldOfView(DungeonGenerator.Instance.CurrentDungeon.Tilemap.WorldToCell(transform.position),
                FieldOfView, true);
        }

        public bool InLineOfSight(Vector3Int position)
        {
            return _visibleTiles.Contains(position);
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _path = DungeonGenerator.Instance.CurrentDungeon.Astar.GetPath(
                    DungeonGenerator.Instance.CurrentDungeon.Tilemap.WorldToCell(transform.position),
                    DungeonGenerator.Instance.CurrentDungeon.Tilemap.WorldToCell(
                        Camera.main.ScreenToWorldPoint(Input.mousePosition)));
            }

            if (_path == null || _path.Completed || !TurnSystem.Instance.PlayerTurn)
                return;

            SetFieldOfView(DungeonGenerator.Instance.CurrentDungeon.Tilemap.WorldToCell(transform.position),
                FieldOfView, false);

            transform.position = _path.GetNextPoint();

            SetFieldOfView(DungeonGenerator.Instance.CurrentDungeon.Tilemap.WorldToCell(transform.position),
                FieldOfView, true);

            TurnSystem.Instance.PlayerTurn = false;
        }

        private void SetFieldOfView(Vector3Int position, int radius, bool visible)
        {
            Dungeon currentDungeon = DungeonGenerator.Instance.CurrentDungeon;
            currentDungeon.Tilemap.SetColor(position,
                visible ? Color.white : GameSettings.Instance.HiddenColor);

            currentDungeon.Tilemap.SetTileFlags(position, TileFlags.None);

            int numRays = 100;
            for (int r = 0; r < numRays; r++)
            {
                float dirX = Mathf.Sin(2 * Mathf.PI * r / numRays);
                float dirY = Mathf.Cos(2 * Mathf.PI * r / numRays);
                Vector2 direction = new Vector2(dirX, dirY);
                for (int d = 1; d < radius; d++)
                {
                    Vector2 relative = position.ToVector2Int() + direction * d;

                    Vector3Int tilePos = currentDungeon.Tilemap.WorldToCell(relative);
                    if (!currentDungeon.Tilemap.HasTile(tilePos)) continue;

                    currentDungeon.Tilemap.SetColor(tilePos,
                        visible ? Color.white : GameSettings.Instance.HiddenColor);
                    currentDungeon.Tilemap.SetTileFlags(tilePos, TileFlags.None);

                    if (visible)
                    {
                        if (!_visibleTiles.Contains(tilePos))
                            _visibleTiles.Add(tilePos);
                    }
                    else
                    {
                        if (_visibleTiles.Contains(tilePos))
                            _visibleTiles.Remove(tilePos);
                    }

                    if (currentDungeon.Tilemap.GetTile<WallTile>(tilePos) != null)
                    {
                        break;
                    }
                }
            }
        }
    }
}