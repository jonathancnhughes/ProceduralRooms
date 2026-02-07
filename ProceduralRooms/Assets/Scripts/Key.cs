using UnityEngine;

namespace ProceduralRooms
{
    public class Key : MonoBehaviour
    {
        public delegate void PickupEvent();

        public event PickupEvent Pickup;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                Pickup?.Invoke();

                Destroy(gameObject, 0.2f);
            }
        }
    }
}