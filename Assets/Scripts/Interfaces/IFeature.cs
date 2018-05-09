using System.Collections.Generic;
using Types;
using UnityEngine;

namespace Interfaces
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
    }
}