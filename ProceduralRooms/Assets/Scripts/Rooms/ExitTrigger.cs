using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralRooms
{
    public class ExitTrigger : MonoBehaviour
    {

        //[SerializeField]
        //private ExitTriggerEventChannelSO _exitRoomTriggerred;

        [SerializeField]
        private Exit _exit;

        [SerializeField]
        private bool _isRoomA;

        private int _id;

        public void Init(int id)
        {
            _id = id;
        }

        private BoxCollider2D _collider;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                _collider.enabled = false;

                if (_isRoomA)
                {
                    Debug.Log("Leave Room A!");
                    _exit.LeaveRoomA();
                }
                else
                {
                    Debug.Log("Leave Room B!");
                    _exit.LeaveRoomB();
                }

                //StartCoroutine(ReenableExitTrigger());
            }        
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                _collider.enabled = true;
            }
        }

        private IEnumerator ReenableExitTrigger()
        {
            yield return new WaitForFixedUpdate();

            _collider.enabled = true;

        }

    }
}