using System.Collections.Generic;
using System.Linq;
using PhiOS.Scripts.PhiOS;
using Types;
using UnityEngine;
using Display = PhiOS.Scripts.PhiOS.Display;

public class CellRenderer : Singleton<CellRenderer>
{
    public bool Render;
    public Vector2Int CameraPosition;
    private Vector2 _dragStart;
    private Queue<CellData> _forceRenderQueue;

    private void Start()
    {
        _forceRenderQueue = new Queue<CellData>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) _dragStart = Mouse.currentCell.position;
        if (Input.GetMouseButton(1))
        {
            CameraPosition -= (Mouse.currentCell.position - _dragStart).ToVector2Int();
            _dragStart = Mouse.currentCell.position;
        }

        RenderLevel();
        ProcessRenderQueue();
    }

    private void ProcessRenderQueue()
    {
        while (_forceRenderQueue.Count > 0)
        {
            RenderCell(_forceRenderQueue.Dequeue());
        }
    }

    private void RenderLevel()
    {
        if (!Render) return;

        Vector2Int position = Vector2Int.zero;
        var allCells = DungeonGenerator.Instance.CurrentDungeon.AllCells;

        for (int i = 0; i < Display.GetDisplayWidth() * Display.GetDisplayHeight(); i++)
        {
            if (CameraPosition.x + position.x < allCells.GetLength(0) &&
                CameraPosition.y + position.y < allCells.GetLength(1) &&
                CameraPosition.x + position.x >= 0 && CameraPosition.y + position.y >= 0)
            {
                var cells = allCells[CameraPosition.x + position.x, CameraPosition.y + position.y];
                if (cells != null)
                {
                    CellData cellData = cells.Last();
                    Cell c = Display.CellAt(0, position.x, position.y);
                    c?.SetContent(
                        cellData.GetRenderInfo().Representation,
                        cellData.GetRenderInfo().BackColor,
                        cellData.GetRenderInfo().ForeColor);
                }
                else
                {
                    Display.CellAt(0, position.x, position.y)?.Clear();
                }
            }
            else
            {
                Display.CellAt(0, position.x, position.y)?.Clear();
            }


            if (position.x == Display.GetDisplayWidth() - 1)
            {
                position.x = 0;
                position.y++;
            }
            else position.x++;
        }
    }

    public void QueueCellRender(CellData cellData)
    {
        _forceRenderQueue.Enqueue(cellData);
    }

    private void RenderCell(CellData cellData)
    { 
        if (!Render) return;

        var position = cellData.Position;
        Cell c = Display.CellAt(0, -CameraPosition.x + position.x,
            -CameraPosition.y + position.y);
        c?.SetContent(cellData.RenderInfo.Representation,
            cellData.RenderInfo.BackColor,
            cellData.RenderInfo.ForeColor);
    }
}