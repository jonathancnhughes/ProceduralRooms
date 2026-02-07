using UnityEngine;
using UnityEngine.Events;

namespace ProceduralRooms
{
    [CreateAssetMenu(menuName = "Procedural Rooms/Events/Init Player Event Channel")]
    public class InitPlayerEventChannelSO : ScriptableObject
    {
        public UnityAction<Player> OnEventRaised;

        public void RaiseEvent(Player p)
        {
            OnEventRaised?.Invoke(p);
        }
    }
}