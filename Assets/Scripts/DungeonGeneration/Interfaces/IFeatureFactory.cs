using System.Collections.Generic;
using DungeonGeneration.Structs;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace DungeonGeneration.Interfaces
{
    public interface IFeatureFactory
    {
        bool TryCreateFeature(FeaturePosition postion, Vector2Int size, Tilemap tilemap, ref IFeature feature);
        bool CanCreateFeature(FeaturePosition postion, Vector2Int size, Tilemap existingTiles);
    }
}