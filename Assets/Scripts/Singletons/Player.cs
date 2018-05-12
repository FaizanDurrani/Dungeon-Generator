using System.Collections;
using System.Collections.Generic;
using DungeonGeneration;
using DungeonGeneration.Structs;
using Enums;
using Extensions;
using Pathfinding;
using PhiOS.Scripts.PhiOS;
using Rendering;
using Structs;
using UnityEngine;
using Display = PhiOS.Scripts.PhiOS.Display;

namespace Singletons
{
    public class Player : Singleton<Player>
    {
        //private Vector2Int CameraPos => Position - new Vector2Int(Display.GetDisplayWidth() / 2, Display.GetDisplayHeight() / 2);
        private CellData _currCell;
        private CellData _lastCell;

        public Vector2Int Position;
        public bool CanControl;
        public float FieldOfView;

        private AstarPath _path;

        public void Spawn(Vector2Int pos)
        {
            Position = pos;
            CanControl = true;
            TurnSystem.Instance.PlayerTurn += PlayerTurn;
            UpdateFov();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _path = DungeonGenerator.Instance.CurrentDungeon.Astar.GetPath(
                    Position,
                    CellRenderer.Instance.CameraPosition + Mouse.currentCell.position.ToVector2Int());
            }
        }

        private IEnumerator PlayerTurn()
        {
            yield return new WaitUntil(() => _path != null && !_path.Completed && CanControl);
            
            Position = _path.GetNextPoint();
            UpdateFov();
            
            TurnSystem.Instance.EndPlayerTurn();
        }

        private void UpdateFov()
        {
            if (_currCell == null || _currCell.Position != Position)
            {
                if (_lastCell?.RenderInfo.Representation != null)
                {
                    SetFieldOfView(_lastCell.Position, FieldOfView, false);
                    DungeonGenerator.Instance.CurrentDungeon.SetCell(_lastCell.Position, _lastCell);
                }

                if (DungeonGenerator.Instance.CurrentDungeon.TryGetCell(Position, out _currCell))
                {
                    _lastCell = new CellData(_currCell.Position, _currCell.Type, _currCell.RenderInfo,
                        _currCell.CanWalkOn);
                    _currCell.RenderInfo = GameSettings.Instance.playerRenderInfo;
                    _currCell.Visible = true;
                    DungeonGenerator.Instance.CurrentDungeon.SetCell(_currCell.Position, _currCell);
                    SetFieldOfView(Position, FieldOfView, true);
                    Debug.Log("Updating FOV");
                }
            }
        }


        private void SetFieldOfView(Vector2Int position, float radius, bool visible)
        {
            Dungeon currentDungeon = DungeonGenerator.Instance.CurrentDungeon;
            int width = currentDungeon.GridSize.x;
            int height = currentDungeon.GridSize.y;

            Vector2 center = new Vector2(position.x + .5f,
                position.y + .5f);
            int numRays = 100;

            for (int r = 0; r < numRays; r++)
            {
                float dirX = Mathf.Sin(2 * Mathf.PI * r / numRays);
                float dirY = Mathf.Cos(2 * Mathf.PI * r / numRays);
                Vector2 direction = new Vector2(dirX, dirY);

                for (int d = 1; d < radius; d++)
                {
                    Vector2 relative = center + direction * d;

                    Vector2Int tilePos = relative.ToVector2Int();
                    CellData cell = null;
                    if (currentDungeon.TryGetCell(tilePos, out cell))
                    {
                        cell.Discovered = true;
                        cell.Visible = visible;
                        currentDungeon.SetCell(tilePos, cell);

                        if (cell.Type == CellType.Wall)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}