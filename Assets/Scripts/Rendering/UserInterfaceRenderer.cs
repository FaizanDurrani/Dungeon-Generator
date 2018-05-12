using Singletons;
using UnityEngine;
using UnityEngine.UI;

namespace Rendering
{
    public class UserInterfaceRenderer : Singleton<UserInterfaceRenderer>
    {
        [SerializeField] private Text _turnCount;

        private void Update()
        {
            _turnCount.text = $"Turn Count: {TurnSystem.Instance.TurnCount}";
        }
    }
}