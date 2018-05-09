using System.Collections;
using System.Collections.Generic;
using FeatureFactories;
using Interfaces;
using Types;
using UnityEngine;
using Display = PhiOS.Scripts.PhiOS.Display;
using Random = UnityEngine.Random;

public class DungeonGenerator : Singleton<DungeonGenerator>
{
    [SerializeField] private int _features;
    [SerializeField] private Vector2IntMinMax _roomSize;

    public List<Dungeon> DungeonLayers;
    public int CurrentLayer;
    public Dungeon CurrentDungeon => DungeonLayers[CurrentLayer];

    private IEnumerator Start()
    {
        if (GameSettings.Instance == null)
        {
            Debug.LogError("NO GameSettings FOUND!!");
            yield break;
        }

        if (CellRenderer.Instance == null)
        {
            Debug.LogError("NO CellRenderer FOUND!!");
            yield break;
        }

        while (!Display.IsInitialized())
        {
            yield return null;
        }
        
        DungeonLayers = new List<Dungeon>();
        GenerateNewDungeonLayer();
        
        Player.Instance.Position = DungeonLayers[CurrentLayer].StartPos;
        Player.Instance.CanControl = true;
        CellRenderer.Instance.Render = true;
    }

    private void GenerateNewDungeonLayer()
    {
        var dungeon = new Dungeon(_roomSize, _features);
        dungeon.GenerateDungeon();
        DungeonLayers.Add(dungeon);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) StartCoroutine(Start());
    }
}