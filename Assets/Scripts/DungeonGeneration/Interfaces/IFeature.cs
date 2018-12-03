using System.Collections.Generic;
using DungeonGeneration.Structs;
using UnityEngine;

namespace DungeonGeneration.Interfaces
{
    public interface IFeature
    {
        /// <summary>
        /// Number of features this feature can make
        /// </summary>
        /// <returns></returns>
        int GetNewFeatureCount();

        /// <summary>
        /// Get a random position for a new feature
        /// </summary>
        /// <returns></returns>
        FeaturePosition GetNewFeaturePosition();

        /// <summary>
        /// Check if we can add a new feature branching from this feature
        /// </summary>
        /// <returns></returns>
        bool CanMakeNewFeature();

        bool Contains(Vector3Int point);

        List<Vector3Int> GetExits();
        void AddExit(Vector3Int exit);
    }
}