using UnityEngine;
using UnityEngine.Events;

namespace ProceduralRooms
{
    [CreateAssetMenu(fileName = "New Enter Room Event", menuName = "Procedural Rooms/Events/Enter Room Event Channel")]
    public class EnterRoomEventChannelSO : ScriptableObject
    {
        public UnityAction<Room> OnEventRaised;

        public void RaiseEvent(Room roomEnter)
        {
            OnEventRaised?.Invoke(roomEnter);
        }
    }
}
