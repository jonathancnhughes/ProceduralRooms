using UnityEngine;

namespace ProceduralRooms
{
    public class DebugOverlap : MonoBehaviour
    {
        [ContextMenu("Test")]
        void MyCollisions()
        {
            //Use the OverlapBox to detect if there are any other colliders within this box area.
            //Use the GameObject's centre, half the size (as a radius) and rotation. This creates an invisible box around your GameObject.
            //Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position, transform.localScale, Quaternion.identity, LayerMask.NameToLayer("Builder"));

            Collider2D _collider = GetComponent<BoxCollider2D>();
            Collider2D[] collider2Ds = new Collider2D[2];
            ContactFilter2D contactFilter = new ContactFilter2D();
            LayerMask mask = LayerMask.GetMask("Builder");

            contactFilter.SetLayerMask(mask);
            int colliderCount = Physics2D.OverlapCollider(_collider, contactFilter, collider2Ds);

            Debug.Log(colliderCount);
        }

        //Draw the Box Overlap as a gizmo to show where it currently is testing. Click the Gizmos button to see this
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }
    }
}