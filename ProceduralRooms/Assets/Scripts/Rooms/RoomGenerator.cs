using System.Collections.Generic;
using UnityEngine;

namespace ProceduralRooms
{
    public class RoomGenerator : MonoBehaviour
    {
        private const string MASK_ROOM = "Room";
        private const string MASK_EXIT = "Builder";

        [SerializeField]
        private RoomPool _roomPool;

        [SerializeField] private RoomScriptableObject _startingRoomSO;

        [SerializeField] private int _roomsToGenerate;

        [SerializeField] private GameObject _exitPrefab;

        [SerializeField] private GameObject _newExitPrefab;

        [SerializeField] private GameObject _playerPrefab;

        [SerializeField] private Key _bossKeyPrefab;

        private List<ExitPoint> ExitPoints = new List<ExitPoint>();

        private List<Vector2> Rooms = new List<Vector2>();

        [SerializeField]
        private int RandomSeed;

        private int ExitNumber = 1;

        private Room _startingRoom;

        [Header("Event Channels")]
        [SerializeField]
        private EventChannelSO _generationComplete;

        [SerializeField]
        private InitPlayerEventChannelSO _initPlayerEvent;

        [SerializeField]
        private SpawnPlayerEventChannelSO _spawnPlayerEvent;

        private Exit _bossExit;

        void Start()
        {
            Random.InitState(RandomSeed);

            SetupStartingRoom();
            GenerateRooms();

            _generationComplete.RaiseEvent();

            var playerObj = Instantiate(_playerPrefab);
            Player player = playerObj.GetComponent<Player>();

            player.InitPlayer();

            _initPlayerEvent.RaiseEvent(player);
            _spawnPlayerEvent.RaiseEvent(Vector2.zero, _startingRoom);

            _startingRoom.ToggleCamera();
        }

        private void SetupStartingRoom()
        {
            RoomScriptableObject startingRoomSO = _roomPool.GetStartingRoom();

            var roomObj = Instantiate(startingRoomSO.RoomPrefab, Vector2.zero, Quaternion.identity);          
            _startingRoom = roomObj.GetComponent<Room>();

            foreach (var exitLocation in startingRoomSO.ExitLocations)
            {
                Exit newExit = Instantiate(_newExitPrefab, exitLocation.WorldPositionOffset, exitLocation.Rotation, _startingRoom.transform).GetComponent<Exit>();
                newExit.gameObject.name = $"Exit {ExitNumber}";
                ExitNumber++;

                var exitWorldPos = transform.TransformPoint(exitLocation.Position);
                newExit.Init(_startingRoom, exitWorldPos, exitLocation.Direction);


                ExitPoint ep = new ExitPoint(exitLocation.WorldPositionOffset, exitLocation.Direction);
                ep.LinkedExit = newExit;
                ExitPoints.Add(ep);
            }
        }

        private Exit CheckForExitOverlap(Vector2 exitPosition)
        {
            var result = Physics2D.OverlapBox(exitPosition, Vector2.one, 0, LayerMask.GetMask("Builder"));

            if (result != null)
            {
                Exit adjExit = result.gameObject.GetComponentInParent<Exit>();
                Debug.Log("Found adjacent exit! " + adjExit.gameObject.name);
                return adjExit;

            }

            return null;
        }

        private void SetupRoom(Room room, List<ExitLocation> exitLocations, Transform roomTransform, bool singleExitOnly, Exit currentExit)
        {
            foreach (var exitLocation in exitLocations)
            {
                var potentialExitPositiion = (Vector2)roomTransform.position + exitLocation.WorldPositionOffset;

                Exit existingExit = CheckForExitOverlap(potentialExitPositiion);
                if (existingExit != null && (!singleExitOnly || existingExit == currentExit))
                {
                    existingExit.SetLinkedExit(room);
                }
                else
                {
                    GameObject exitGO = Instantiate(_newExitPrefab, potentialExitPositiion, exitLocation.Rotation, roomTransform);
                    Exit newExit = exitGO.GetComponent<Exit>();
                    
                    newExit.gameObject.name = $"Exit {ExitNumber}";
                    newExit.Init(room, exitLocation.WorldPositionOffset, exitLocation.Direction);
                    ExitNumber++;

                    if (newExit.CheckForRoomOverlap())
                    {
                        newExit.BlockExit();
                    }
                    else
                    {
                        if (singleExitOnly)
                        {
                            newExit.BlockExit();
                        }
                        else
                        {
                            var exitWorldPos = transform.TransformPoint(exitLocation.Position);
                            ExitPoint ep = new ExitPoint(potentialExitPositiion, exitLocation.Direction);
                            ep.LinkedExit = newExit;
                            ExitPoints.Add(ep);
                        }
                    }

                }
            }
        }

        // Generate Rooms
        //
        // Pick an exit at random from the exits list
        // Select a room that fits
        // Turn on/off all exits in room and add open exits to exits list
        // repeat until no rooms to generate
        // go back through exits still in list and close them
        private void GenerateRooms()
        {
            Debug.Log("Start generating");
            int roomsGenerated = 0;
            bool roomOverlaps = false;          

            List<ExitPoint> failedExits = new List<ExitPoint>();

            while (roomsGenerated < _roomsToGenerate)
            {

                if (ExitPoints.Count == 0)
                {
                    Debug.Log("Uh oh! Ran out of exits!");
                    break;
                }

                int index = Random.Range(0, ExitPoints.Count);
                if (index < 0)
                {
                    Debug.Log("Uh oh! Ran out of exits!");
                    break;
                }

                ExitPoint thisExit = ExitPoints[index];

                if (thisExit == null)
                {
                    Debug.Log("Uh oh! Ran out of exits!");
                    break;
                }

                Debug.Log($"{thisExit.LinkedExit.gameObject.name} selected!");

                GameObject roomObj = null;
                Room newRoom = null;

                int roomTries = 3;
                bool blnSuccess = true;
                Vector2 newRoomPos = Vector2.zero;
                RoomScriptableObject roomSO;

                do
                {
                    
                    // subtract 1 from the exit count to not include the exit currently being processed.
                    roomSO = _roomPool.GetRoom(thisExit.Direction, roomsGenerated, _roomsToGenerate, ExitPoints.Count - 1);

                    if (roomSO == null)
                    {
                        blnSuccess = false;
                        break;
                    }

                    List<ExitLocation> matchingExits = roomSO.GetExitLocationsFor(thisExit.Direction);

                    newRoomPos = thisExit.Position;
                    newRoomPos -= matchingExits[0].WorldPositionOffset;

                    var result = Physics2D.OverlapBox(newRoomPos, new Vector2(roomSO.RoomWidth, roomSO.RoomHeight), 0, LayerMask.GetMask("Builder2"));
                    if (result != null)
                    {
                        roomOverlaps = true;

                        roomTries--;

                        if (roomTries == 0)
                        {
                            blnSuccess = false;
                            break;
                        }

                    }
                    else
                    {
                        Debug.Log($"No overlap! Add room {roomSO.name} @ {newRoomPos}");

                        roomObj = Instantiate(roomSO.RoomPrefab, newRoomPos, Quaternion.identity);
                        newRoom = roomObj.GetComponent<Room>();
                        newRoom.Init(roomSO);
                        roomOverlaps = false;

                        _roomPool.RoomProcessed(roomSO);
                    }

                } while (roomOverlaps);

                if (blnSuccess)
                {
                    roomObj.name = $"Room {(roomsGenerated + 1)}";

                    foreach (var el in roomSO.ExitLocations)
                    {
                        var exitPos2 = (Vector2)newRoom.transform.position + el.WorldPositionOffset;
                        Debug.Log(exitPos2);
                    }

                    // Create Exits!
                    SetupRoom(newRoom, roomSO.ExitLocations, roomObj.transform, roomSO.SingleExitOnly, thisExit.LinkedExit);
                    
                    if (newRoom.IsBoss)
                    {
                        roomObj.name = $"BOSS ROOM";
                        _bossExit = thisExit.LinkedExit;
                    }
                    else
                    {
                        Rooms.Add(newRoomPos);
                    }

                    roomsGenerated++;
                }
                else
                {
                    Debug.Log("Add to failed Exits - to be blocked later! " + thisExit.LinkedExit.gameObject.name);
                    failedExits.Add(thisExit);
                }

                ExitPoints.Remove(thisExit);

            }

            foreach (var ep in ExitPoints)
            {
                ep.LinkedExit.BlockExit();
            }
            foreach (var ep in failedExits)
            {
                ep.LinkedExit.BlockExit();
            }


            SetupBossLockAndKey();
        }

        private void SetupBossLockAndKey()
        {
            // Lock Boss Room!/
            if (_bossExit != null)
            {
                var keyPos = Rooms[Random.Range(0, Rooms.Count)];
                Key key = Instantiate(_bossKeyPrefab, keyPos, Quaternion.identity);
                _bossExit.LinkKey(key);
            }
        }
    }
}