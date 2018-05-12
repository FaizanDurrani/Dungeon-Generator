using System.Collections.Generic;
using DungeonGeneration.Structs;
using UnityEngine;

namespace DungeonGeneration.Interfaces
{
    public interface IFeature
    {
        /// <summary>
        /// Get all the cells this feature will occupy
        /// </summary>
        /// <returns></returns>
        List<CellData> GetCells(HashSet<Vector2Int> existingTiles);

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

        bool Contains(Vector2Int point);

        List<Vector2Int> GetExits();
        void AddExit(Vector2Int exit);
        
        void SetPosition(Vector2Int position);
        Vector2Int GetPosition();
    }
}