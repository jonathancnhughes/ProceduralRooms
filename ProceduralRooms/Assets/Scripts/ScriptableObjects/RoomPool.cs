using System.Collections.Generic;
using UnityEngine;

namespace ProceduralRooms
{
    [CreateAssetMenu(fileName = "New Room Pool", menuName = "Procedural Rooms/Room Pool")]
    public class RoomPool : ScriptableObject
    {
        [SerializeField]
        private RoomScriptableObject _startingRoom;

        [SerializeField]
        private List<RoomScriptableObject> Rooms;

        [SerializeField]
        private List<SpecialRoomScriptableObject> SpecialRooms;

        private List<SpecialRoomScriptableObject> _specialRoomsRemaining;

        private void OnEnable()
        {
            _specialRoomsRemaining = new List<SpecialRoomScriptableObject>(SpecialRooms);
        }

        public RoomScriptableObject GetStartingRoom()
        {
            return _startingRoom;
        }

        public RoomScriptableObject GetRoom(ExitDir direction, int roomsAdded, int totalRooms, int exitCount)
        {
            RoomScriptableObject selectedRoom = null;
            List<RoomScriptableObject> suitableRooms = new List<RoomScriptableObject>();

            // check if a special room should be selected first...
            // If there are no special rooms remaining then skip to picking a regular room

            // CURRENT BUG - If a special room is selected but not able to be placed, it has already been
            // removed from the pool so will never be added.
            if (_specialRoomsRemaining.Count > 0)
            {
                if (_specialRoomsRemaining.Count == (totalRooms - roomsAdded))
                {
                    suitableRooms.AddRange(_specialRoomsRemaining);
                }
                else
                {
                    foreach (SpecialRoomScriptableObject spRoom in _specialRoomsRemaining)
                    {
                        float chancePct = 0;
                        if (spRoom.SelectionChanceAfterSpawningXRooms.Count > 0)
                        {
                            int index = Mathf.Min(roomsAdded, spRoom.SelectionChanceAfterSpawningXRooms.Count - 1);
                            chancePct = spRoom.SelectionChanceAfterSpawningXRooms[index];

                            if (Random.Range(0f, 1f) <= chancePct)
                            {
                                suitableRooms.Add(spRoom);
                                break;
                            }
                        }
                    }
                }

                if (suitableRooms.Count > 0)
                {
                    var index = Random.Range(0, suitableRooms.Count);
                    if (index >= 0)
                    {
                        selectedRoom = suitableRooms[index];

                        return selectedRoom;
                    }
                }
            }

            // Select from regular rooms.
            suitableRooms = new List<RoomScriptableObject>();

            foreach (var room in Rooms)
            {
                if (IsRoomSuitable(room, totalRooms - roomsAdded, exitCount, direction))
                {
                    suitableRooms.Add(room);
                }
            }

            if (suitableRooms.Count > 0)
            {
                var index = Random.Range(0, suitableRooms.Count);
                if (index >= 0)
                {
                    selectedRoom = suitableRooms[index];
                }
            }

            return selectedRoom;
        }

        public void RoomProcessed(RoomScriptableObject roomSO)
        {
            // After placing a specific room type, there may be a need to lower
            // the likelihood of it being selected again, or removed from the pool altogether
            if (roomSO is SpecialRoomScriptableObject)
            {
                var specialRoom = roomSO as SpecialRoomScriptableObject;

                if (_specialRoomsRemaining.Contains(specialRoom))
                {
                    _specialRoomsRemaining.Remove(specialRoom);
                }

            }
        }

        private bool IsRoomSuitable(RoomScriptableObject room, int roomsToGenerate, int exitCount, ExitDir direction)
        {
            bool hasMatchingDirection = false;

            foreach(var el in room.ExitLocations)
            {
                if (direction == el.Direction)
                {
                    hasMatchingDirection = true;
                    break;
                }
            }

            if (hasMatchingDirection)
            {
                // subtract one exit to exclude the exit that the room is being initially connected to.
                if (exitCount + (room.TotalExits - 1) == 0)
                {
                    Debug.Log("Not enough exits! choose another room!");
                    return false;
                }
                return true;
            }

            return false;
        }

    }
}