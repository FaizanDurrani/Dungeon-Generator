using Boo.Lang;
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
        List<CellData> GetCells();
        
        /// <summary>
        /// Number of features this feature can make
        /// </summary>
        /// <returns></returns>
        int GetNewFeatureCount();
        
        /// <summary>
        /// Get a random position for a new feature
        /// </summary>
        /// <returns></returns>
        Vector2Int GetNewFeaturePosition();
    }
}