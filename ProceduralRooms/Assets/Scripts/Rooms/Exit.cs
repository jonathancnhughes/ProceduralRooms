using UnityEngine;
using UnityEngine.Events;

namespace ProceduralRooms
{
    public class Exit : MonoBehaviour
    {
        [SerializeField]
        private Transform _exitA;
        [SerializeField]
        private Transform _exitB;

        private Room _roomA;
        private Room _roomB;

        private Vector2 _worldPos;
        public Vector2 WorldPos
        {
            get { return _worldPos; }
        }
        private ExitDir _direction;

        [SerializeField]
        private GameObject _blocker;

        [SerializeField]
        private BoxCollider2D _adjacentRoomTrigger;

        [SerializeField]
        private BoxCollider2D _adjacentExitTrigger;

        [SerializeField]
        private SpawnPlayerEventChannelSO _spawnPlayerTriggered;

        [Header("Door")]
        [SerializeField]
        private GameObject _doors;
        [SerializeField]
        private GameObject[] _locks;

        private UnityAction _unlockEvent;

        public void Init(Room room, Vector2 worldPos, ExitDir direction)
        {
            _roomA = room;
            _roomA.AddExit(this);

            _worldPos = worldPos;
            switch (direction)
            {
                case ExitDir.Up:
                    _direction = ExitDir.Down;
                    foreach(var l in _locks)
                    {
                        l.transform.Rotate(0, 0, -90);
                        l.transform.localScale = new Vector3(0.1111111f, 1, 1);
                    }
                    break;
                case ExitDir.Down:
                    _direction = ExitDir.Up;
                    foreach (var l in _locks)
                    {
                        l.transform.Rotate(0, 0, 90);
                        l.transform.localScale = new Vector3(0.1111111f, 1, 1);
                    }
                    break;
                case ExitDir.Left:
                    _direction = ExitDir.Right;
                    foreach (var l in _locks)
                    {
                        l.transform.Rotate(0, 0, 180);
                    }
                    break;
                case ExitDir.Right:
                    _direction = ExitDir.Left;

                    break;
            }

            _blocker.SetActive(false);
        }

        private void OnDestroy()
        {
            _unlockEvent -= Unlock;
        }

        public void SetLinkedExits(Room roomA, Room roomB)
        {
            _roomA = roomA;
            _roomB = roomB;
        }

        public void SetLinkedExit(Room roomB)
        {
            _roomB = roomB;
            _roomB.AddExit(this);
        }

        [ContextMenu("Room Overlap")]
        public bool CheckForRoomOverlap()
        {
            Collider2D[] collider2Ds = new Collider2D[2];
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(LayerMask.GetMask("Builder2"));
            contactFilter.useTriggers = true;
            int colliderCount = Physics2D.OverlapCollider(_adjacentRoomTrigger, contactFilter, collider2Ds);


            if (colliderCount > 0)
            {
                Debug.Log("Adjacent Room found!");
                return true;
            }

            return false;
        }

        [ContextMenu("Exit Overlap")]
        public Exit CheckForOverlap()
        {

            Collider2D[] result = new Collider2D[1];
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.SetLayerMask(LayerMask.GetMask("Builder"));
            contactFilter.useTriggers = true;
            int colliderCount = Physics2D.OverlapCollider(_adjacentExitTrigger, contactFilter, result);

            if (colliderCount > 0)
            {
                Exit adjExit = result[0].gameObject.GetComponentInParent<Exit>();
                Debug.Log("Found adjacent exit! " + adjExit.gameObject.name);
                return adjExit;
            }

            return null;
        }

        public void BlockExit()
        {
            if (_roomA == null || _roomB == null)   // final check, might have connected the rooms up indirectly.
            {
                if (_roomA != null)
                {
                    _roomA.RemoveExit(this);
                }
                if (_roomB != null)
                {
                    _roomB.RemoveExit(this);
                }
                _blocker.SetActive(true);
            }           
        }

        public void LinkKey(Key key)
        {
            key.Pickup += Unlock;
            Lock();
        }

        public void Lock()
        {
            _doors.SetActive(true);
        }

        public void Unlock()
        {
            _doors.SetActive(false);
        }


        public void LeaveRoomA()
        {
            _roomB.EnableCamera();
            _roomA.DisableCamera();
            
            _spawnPlayerTriggered.RaiseEvent(_exitB.position, _roomB);
        }

        public void LeaveRoomB()
        {
            _roomA.EnableCamera();
            _roomB.DisableCamera();

            _spawnPlayerTriggered.RaiseEvent(_exitA.position, _roomA);
        }
    }
}