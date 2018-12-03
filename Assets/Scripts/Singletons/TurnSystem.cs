using System;
using System.Collections;

namespace Singletons
{
    public class TurnSystem : Singleton<TurnSystem>
    {
        public bool PlayerTurn;
        public int TurnCount;
    }
}