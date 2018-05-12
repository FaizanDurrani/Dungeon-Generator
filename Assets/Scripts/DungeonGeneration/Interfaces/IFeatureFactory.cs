using System.Collections.Generic;
using DungeonGeneration.Structs;
using UnityEngine;

namespace DungeonGeneration.Interfaces
{
    public interface IFeatureFactory
    {
        bool TryCreateFeature(FeaturePosition postion, Vector2Int size, HashSet<Vector2Int> existingTiles, ref IFeature feature);
        bool CanCreateFeature(FeaturePosition postion, Vector2Int size, HashSet<Vector2Int> existingTiles);
        List<CellData> GetCellsFromAllFeatures(HashSet<Vector2Int> existingTiles);
    }
}