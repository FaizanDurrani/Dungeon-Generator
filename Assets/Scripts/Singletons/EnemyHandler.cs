using System;
using System.Collections;

namespace Singletons
{
    public class EnemyHandler : Singleton<EnemyHandler>
    {
        private void Start()
        {
            TurnSystem.Instance.EnemyTurn += OnEnemyTurn;
        }

        private IEnumerator OnEnemyTurn()
        {
            yield return null;
            TurnSystem.Instance.EndEnemyTurn();
        }
    }
}