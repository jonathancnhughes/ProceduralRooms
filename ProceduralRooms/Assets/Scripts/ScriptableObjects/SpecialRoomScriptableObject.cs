using System.Collections.Generic;
using UnityEngine;

namespace ProceduralRooms
{
    [CreateAssetMenu(menuName = "Procedural Rooms/Create Special Room", fileName = "New Room")]
    public class SpecialRoomScriptableObject : RoomScriptableObject
    {
        public List<float> SelectionChanceAfterSpawningXRooms;
        public bool ResetAfterAddingOtherSpecailRoom;
    }
}