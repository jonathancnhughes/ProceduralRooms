using UnityEngine;
using UnityEngine.Events;

namespace ProceduralRooms
{
    [CreateAssetMenu(menuName = "Procedural Rooms/Events/Spawn Player Event Channel")]
    public class SpawnPlayerEventChannelSO : ScriptableObject
    {
        public UnityAction<Vector2, Room> OnEventRaised;

        public void RaiseEvent(Vector2 spawnPosition, Room room)
        {
            OnEventRaised?.Invoke(spawnPosition, room);
        }
    }
}