using DungeonGeneration;
using Singletons;

namespace Enemies
{
    public class EnemyHandler : Singleton<EnemyHandler>
    {
        
        
        private void Update()
        {
            if (TurnSystem.Instance.PlayerTurn) return;

            TurnSystem.Instance.PlayerTurn = true;
        }

        public void SpawnEnemies()
        {
            var dungeon = DungeonGenerator.Instance.CurrentDungeon;
            
            
        }
    }
}