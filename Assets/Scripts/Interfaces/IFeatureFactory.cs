using System.Collections.Generic;
using Types;
using UnityEngine;

namespace Interfaces
{
    public interface IFeatureFactory
    {
        bool TryCreateFeature(FeaturePosition postion, Vector2Int size, HashSet<Vector2Int> existingTiles, ref IFeature feature);
        bool CanCreateFeature(FeaturePosition postion, Vector2Int size, HashSet<Vector2Int> existingTiles);
        List<CellData> GetCellsFromAllFeatures(HashSet<Vector2Int> existingTiles);
    }
}