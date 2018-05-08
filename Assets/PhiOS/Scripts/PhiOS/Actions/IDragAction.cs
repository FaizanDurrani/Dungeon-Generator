using UnityEngine;

namespace PhiOS.Scripts.PhiOS.Actions
{
	public interface IDragAction {
		void OnDragStart();
		void OnDragDelta (Vector2 delta);
	}
}
