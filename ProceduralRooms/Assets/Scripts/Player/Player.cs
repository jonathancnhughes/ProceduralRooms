using UnityEngine;
using UnityEngine.InputSystem;

namespace ProceduralRooms
{
    public class Player : MonoBehaviour
    {
        private Rigidbody2D _rigidBody;
        private Transform _transform;

        [SerializeField]
        private float _moveSpeed = 1f;

        private Vector2 _moveInput;

        [Header("Event Channels")]
        [SerializeField]
        private SpawnPlayerEventChannelSO _spawnPlayerTriggered;
        [SerializeField]
        private BoolEventChannelSO _toggleInput;
        [SerializeField]
        private EnterRoomEventChannelSO _enterRoomEvent;

        private bool _inputBlocked;

        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _transform = GetComponent<Transform>();           
        }

        private void OnEnable()
        {
            _spawnPlayerTriggered.OnEventRaised += MovePlayer;
            _toggleInput.OnEventRaised += ToggleInput;
        }

        private void OnDisable()
        {
            _spawnPlayerTriggered.OnEventRaised -= MovePlayer;
            _toggleInput.OnEventRaised -= ToggleInput;
        }

        private void MovePlayer(Vector2 spawnPosition, Room room)
        {
            _rigidBody.position = spawnPosition;
            _rigidBody.linearVelocity = Vector2.zero;
            if (_enterRoomEvent != null)
            {
                _enterRoomEvent.RaiseEvent(room);
            }
        }

        public void InitPlayer()
        {
            _transform.position = Vector3.zero;
            _rigidBody.linearVelocity = Vector2.zero;

        }

        private void FixedUpdate()
        {
            if (!_inputBlocked)
            {
                _rigidBody.MovePosition(_rigidBody.position + (_moveSpeed * Time.fixedDeltaTime * _moveInput));
            }            
        }

        private void ToggleInput(bool blockInput)
        {
            _inputBlocked = blockInput;
        }

        void OnMove(InputValue value)
        {
            _moveInput = value.Get<Vector2>();
        }
    }
}