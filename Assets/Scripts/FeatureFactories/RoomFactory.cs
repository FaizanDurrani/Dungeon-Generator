using System.Collections.Generic;
using Interfaces;
using UnityEngine;

namespace FeatureFactories
{
    public class RoomFactory : IFeatureFactory
    {
        public void CreateFeature(Vector2Int postion, Vector2Int size, out int featureCount)
        {
            throw new System.NotImplementedException();
        }

        public bool CanCreateFeature(Vector2Int postion, Vector2Int size, HashSet<Vector2Int> existingTiles)
        {
            throw new System.NotImplementedException();
        }
    }
}