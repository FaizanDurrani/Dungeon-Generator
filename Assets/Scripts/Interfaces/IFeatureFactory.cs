using System.Collections.Generic;
using UnityEngine;

namespace Interfaces
{
    public interface IFeatureFactory
    {
        void CreateFeature(Vector2Int postion, Vector2Int size, out int featureCount);
        bool CanCreateFeature(Vector2Int postion, Vector2Int size, HashSet<Vector2Int> existingTiles);
    }
}