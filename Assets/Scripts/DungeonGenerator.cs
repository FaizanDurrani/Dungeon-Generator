using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Features;
using PhiOS.Scripts.PhiOS;
using Types;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor.Hardware;
using UnityEngine;
using UnityScript.Steps;
using Display = PhiOS.Scripts.PhiOS.Display;
using Random = UnityEngine.Random;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private int _numberOfRooms;
    [SerializeField] private RandomVector2Int _roomSize;
    [SerializeField] private int _corridorSizeMin, _corridorSizeMax;

    // Temporary vars, cleared after they are used
    private HashSet<Vector2Int> _floorPositions;
    private List<CellData> _floorCells;

    [SerializeField] private List<CellData>[,] _allCells;
    [SerializeField] private List<RoomData> _rooms;
    [SerializeField] private Vector2Int _cameraPos;

    private bool _render;

    private IEnumerator Start()
    {
        if (GameProperties.Instance == null)
        {
            Debug.LogError("NO GAMEPROPERTIES GAME OBJECT FOUND!!");
        }

        _floorPositions = new HashSet<Vector2Int>();
        _floorCells = new List<CellData>();
        _rooms = new List<RoomData>();
        _render = false;

        // wait until display has initialized
        while (!Display.IsInitialized())
        {
            yield return null;
        }

        GenerateRoomsAndCorridors();
        GenerateWalls();
        CalculateGrid();

        Debug.Log(_floorPositions.Count);

        _floorPositions = null;
        _floorCells = null;
        _render = true;
    }

    private void CalculateGrid()
    {
        _allCells = new List<CellData>[_gridSize.x + 1, _gridSize.y + 1];

        // loop over all the tiles...
        foreach (CellData cellData in _floorCells)
        {
            var cellsOnPos = _allCells[cellData.Position.x, cellData.Position.y];

            // ...add them to the array
            if (cellsOnPos == null) // The list is initialized
                cellsOnPos = new List<CellData>();

            cellsOnPos.Add(cellData);
            
            _allCells[cellData.Position.x, cellData.Position.y] = cellsOnPos;
        }
    }

    [SerializeField] private Vector2Int _gridSize;

    private void GenerateRoomsAndCorridors()
    {
        // start top left
        Vector2Int position = new Vector2Int(2, 2);
        CorridorData lastCorridor = new CorridorData();

        for (int i = 0; i < _numberOfRooms; i++)
        {
            RoomData roomData;
            if (i == 0)
            {
                roomData = new RoomData(position, _roomSize.GetRandomValue());
                roomData.AddCells(_floorPositions, _floorCells,
                    GameProperties.Instance.RoomRepresentation,
                    GameProperties.Instance.RoomBackColor,
                    GameProperties.Instance.RoomForeColor);

                _rooms.Add(roomData);
                lastCorridor = roomData.GetCorridor(Random.Range(_corridorSizeMin, _corridorSizeMax), lastCorridor);
                lastCorridor.AddCells(_floorPositions, _floorCells,
                    GameProperties.Instance.RoomRepresentation,
                    GameProperties.Instance.RoomBackColor,
                    GameProperties.Instance.RoomForeColor);

                continue;
            }

            roomData = new RoomData(lastCorridor, _roomSize.GetRandomValue());
            _rooms.Add(roomData);

            roomData.AddCells(_floorPositions, _floorCells,
                GameProperties.Instance.RoomRepresentation,
                GameProperties.Instance.RoomBackColor,
                GameProperties.Instance.RoomForeColor);

            if (i < _numberOfRooms - 1)
            {
                lastCorridor = roomData.GetCorridor(Random.Range(_corridorSizeMin, _corridorSizeMax), lastCorridor);
                lastCorridor.AddCells(_floorPositions, _floorCells,
                    GameProperties.Instance.RoomRepresentation,
                    GameProperties.Instance.RoomBackColor,
                    GameProperties.Instance.RoomForeColor);
            }
        }
    }

    private void GenerateWalls()
    {
        // loop through all the tiles
        var temp = new List<CellData>(_floorCells);
        foreach (var cellData in temp)
        {
            // check if the current tile is of type "floor"
            if (cellData.Type == CellType.Floor)
            {
                // if there's no tile on the left side of the current tile...
                if (!TileExists(cellData.Left))
                {
                    // ...make a wall there
                    var wall = MakeWall(cellData.Left);

                    if (wall.Position.x > _gridSize.x)
                        _gridSize.x = wall.Position.x;

                    if (wall.Position.y > _gridSize.y)
                        _gridSize.y = wall.Position.y;
                }

                // if there's no tile on the Right side of the current tile...
                if (!TileExists(cellData.Right))
                {
                    // ...make a wall there
                    var wall = MakeWall(cellData.Right);

                    if (wall.Position.x > _gridSize.x)
                        _gridSize.x = wall.Position.x;

                    if (wall.Position.y > _gridSize.y)
                        _gridSize.y = wall.Position.y;
                }

                // if there's no tile on the Top side of the current tile...
                if (!TileExists(cellData.Top))
                {
                    // ...make a wall there
                    var wall = MakeWall(cellData.Top);

                    if (wall.Position.x > _gridSize.x)
                        _gridSize.x = wall.Position.x;

                    if (wall.Position.y > _gridSize.y)
                        _gridSize.y = wall.Position.y;
                }

                // if there's no tile on the Bottom side of the current tile...
                if (!TileExists(cellData.Bottom))
                {
                    // ...make a wall there
                    var wall = MakeWall(cellData.Bottom);

                    if (wall.Position.x > _gridSize.x)
                        _gridSize.x = wall.Position.x;

                    if (wall.Position.y > _gridSize.y)
                        _gridSize.y = wall.Position.y;
                }
            }

            if (cellData.Position.x > _gridSize.x)
                _gridSize.x = cellData.Position.x;

            if (cellData.Position.y > _gridSize.y)
                _gridSize.y = cellData.Position.y;
        }
    }

    private CellData MakeWall(Vector2Int position)
    {
        var wallData = new CellData(position,
            0,
            GameProperties.Instance.WallForeColor);

        //if (_cellsToRender.ContainsKey(position))
        _floorCells.Add(wallData);
        //else _cellsToRender.Add(position, new List<CellData> {wallData});

        return wallData;
    }

    private bool TileExists(Vector2Int position)
    {
        return _floorPositions.Contains(position);
    }

    private void Update()
    {
        if (!_render) return;
        if (Input.GetKeyDown(KeyCode.Space)) StartCoroutine(Start());

        if (Input.GetKey(KeyCode.RightArrow))
        {
            _cameraPos += new Vector2Int(1, 0);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            _cameraPos += new Vector2Int(-1, 0);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            _cameraPos += new Vector2Int(0, -1);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            _cameraPos += new Vector2Int(0, 1);
        }

        Vector2Int position = Vector2Int.zero;

        for (int i = 0; i < Display.GetDisplayWidth() * Display.GetDisplayHeight(); i++)
        {
            if (_cameraPos.x + position.x < _allCells.GetLength(0) &&
                _cameraPos.y + position.y < _allCells.GetLength(1) &&
                _cameraPos.x + position.x >= 0 && _cameraPos.y + position.y >= 0)
            {
                var cells = _allCells[_cameraPos.x + position.x, _cameraPos.y + position.y];
                if (cells != null)
                {
                    CellData cellData = cells.Last();
                    Cell c = Display.CellAt(0, position.x, position.y);
                    c?.SetContent(
                        cellData.Representation,
                        cellData.BackColor,
                        cellData.ForeColor);
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
}