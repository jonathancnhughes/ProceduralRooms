using UnityEngine;
using UnityEngine.Events;

namespace ProceduralRooms
{
    [CreateAssetMenu(menuName = "Procedural Rooms/Events/Exit Trigger Event Channel")]

    public class ExitTriggerEventChannelSO : ScriptableObject
    {
        public UnityAction<int> OnEventRaised;

        public void RaiseEvent(int exitID)
        {
            OnEventRaised?.Invoke(exitID);
        }
    }
}