using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralRooms
{
    public class Room : MonoBehaviour
    {
        [SerializeField]
        private bool _isSpecial;
        [SerializeField]
        private bool _isBoss;

        [SerializeField]
        private CinemachineVirtualCamera _cam;

        [Header("Room Events")]
        [SerializeField]
        private InitPlayerEventChannelSO _initPlayerEventChannel;
        [SerializeField]
        private EnterRoomEventChannelSO _enterRoomEvent;

        private bool _visited;

        public bool IsSpecial
        {
            get { return _isSpecial; }
        }

        public bool IsBoss
        {
            get { return _isBoss; }
        }

        public bool Visited
        {
            get { return _visited; }
            set { _visited = value; }
        }


        [SerializeField] private Collider2D _roomCollider;

        public float RoomHeight
        {
            get
            {
                return _roomCollider.bounds.size.y;
            }
        }

        public float RoomWidth
        {
            get
            {
                return _roomCollider.bounds.size.x;
            }
        }

        [SerializeField]
        private PolygonCollider2D _cameraBoundsCollider;
        public PolygonCollider2D CameraBoundsCollider
        {
            get { return _cameraBoundsCollider; }
        }

        private List<Exit> _exits = new List<Exit>();

        private bool _lockRoomOnEntry;


        private void OnEnable()
        {
            _initPlayerEventChannel.OnEventRaised += SetCameraTarget;
            _enterRoomEvent.OnEventRaised += OnEnterRoom;
        }

        private void OnDisable()
        {
            _initPlayerEventChannel.OnEventRaised -= SetCameraTarget;
            _enterRoomEvent.OnEventRaised -= OnEnterRoom;
        }

        public void Init(RoomScriptableObject roomSO)
        {
            // set room parameters based on Scriptable Object
            _lockRoomOnEntry = roomSO.LockExitsOnEntry;

        }

        private void OnEnterRoom(Room room)
        {
            if (room != this)
                return;

            if (_lockRoomOnEntry)
            {
                Debug.Log("LOCK DOORS!");
                LockAllExits();
            }

        }

        private void SetCameraTarget(Player p)
        {
            _cam.Follow = p.transform;
        }

        public void DisableCamera()
        {
            _cam.enabled = false;
        }

        public void EnableCamera()
        {
            _cam.enabled = true;
        }

        public void ToggleCamera()
        {
            _cam.enabled = !_cam.enabled;
        }

        [ContextMenu("Check for Overlap")]
        public bool CheckForOverlap()
        {
            Collider2D[] result = new Collider2D[1];
            var cf = new ContactFilter2D();
            cf.SetLayerMask(LayerMask.GetMask("Builder2"));
            cf.useTriggers = true;

            int colliderCount = Physics2D.OverlapCollider(_roomCollider, cf, result);

            if (colliderCount > 0)
            {
                string output = "";
                foreach (var found in result)
                {
                    
                    output += found.transform.parent.gameObject.name + "\n";
                }

                Debug.Log("Found adjacent room!\n" + output);
                return true;
            }
            else
            {
                Debug.Log("No overlap!");
            }

            return false;
        }


        public void AddExit(Exit exit)
        {
            if (!_exits.Contains(exit))
            {
                _exits.Add(exit);
            }
        }
        public void RemoveExit(Exit exit)
        {
            if (_exits.Contains(exit))
            {
                _exits.Remove(exit);
            }
        }

        public void LockAllExits()
        {
            foreach (var ex in _exits)
            {
                ex.Lock();
            }
        }

        public void UnlockAllExits()
        {
            foreach (var ex in _exits)
            {
                ex.Unlock();
            }
        }
    }
}