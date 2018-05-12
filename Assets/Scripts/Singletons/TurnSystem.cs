using System;
using System.Collections;

namespace Singletons
{
    public class TurnSystem : Singleton<TurnSystem>
    {
        public event Func<IEnumerator> EnemyTurn, PlayerTurn;
        public int TurnCount;

        public void Initialize()
        {
            StartCoroutine(PlayerTurn?.Invoke());
        }

        public void EndPlayerTurn()
        {
            StartCoroutine(EnemyTurn?.Invoke());
        }

        public void EndEnemyTurn()
        {
            StartCoroutine(PlayerTurn?.Invoke());
            TurnCount++;
        }
    }
}