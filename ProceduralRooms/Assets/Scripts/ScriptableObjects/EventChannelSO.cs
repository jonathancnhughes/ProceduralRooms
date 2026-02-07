using UnityEngine;
using UnityEngine.Events;

namespace ProceduralRooms
{
    [CreateAssetMenu(menuName = "Procedural Rooms/Events/Event Channel")]
    public class EventChannelSO : ScriptableObject
    {
        public UnityAction OnEventRaised;

        public void RaiseEvent()
        {
            OnEventRaised?.Invoke();
        }
    }
}