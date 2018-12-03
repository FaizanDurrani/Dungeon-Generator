using System.Collections;
using System.Collections.Generic;
using Rendering;
using Singletons;
using Structs;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DungeonGeneration
{
    public class DungeonGenerator : Singleton<DungeonGenerator>
    {
        [SerializeField] private int _features;
        [SerializeField] private Vector3IntMinMax _roomSize;
        [SerializeField] private Tilemap _tilemap;

        public List<Dungeon> DungeonLayers;
        public int CurrentLayer;
        public Dungeon CurrentDungeon => DungeonLayers[CurrentLayer];

        private void Start()
        {
            DungeonLayers = new List<Dungeon>();
            GenerateNewDungeonLayer();
        
            Player.Instance.Spawn(DungeonLayers[CurrentLayer].StartPos);
        }

        private void GenerateNewDungeonLayer()
        {
            var dungeon = new Dungeon(_roomSize, _features, _tilemap);
            dungeon.GenerateDungeon();
            DungeonLayers.Add(dungeon);
        }
    }
}