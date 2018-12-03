using DungeonGeneration;
using Rendering;
using Structs;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Singletons
{
    public class GameSettings : Singleton<GameSettings>
    {
        public RenderInfo FloorTileData, WallTileData;
        public Color HiddenColor, DiscoveredColor;
    }
}