using System.Collections.Generic;
using UnityEngine;

namespace ProceduralRooms
{
    [CreateAssetMenu(menuName = "Procedural Rooms/Create Room", fileName = "New Room")]
    public class RoomScriptableObject : ScriptableObject
    {
        public GameObject RoomPrefab;

        public float RoomHeight;
        public float RoomWidth;

        public List<ExitLocation> ExitLocations;

        public bool SingleExitOnly;
        public bool LockExitsOnEntry;

        [HideInInspector]
        public int TotalExits;

        private void OnEnable()
        {
            LoadExitLocations();
        }

        private void LoadExitLocations()
        {
            ExitLocations = new List<ExitLocation>();
            GameObject go = Instantiate(RoomPrefab);
            
            Debug.Log("Loaded rooM!");
            var exitsParent = go.transform.Find("RoomBG/Exits");

            if (exitsParent != null)
            {
                for (int i = 0; i < exitsParent.childCount; i++)
                {
                    Transform exitTransform = exitsParent.GetChild(i);
                    ExitLocation el = new ExitLocation()
                    {
                        Position = exitTransform.localPosition,
                        WorldPositionOffset = exitTransform.position,
                        Rotation = exitTransform.rotation
                    };
                    if (exitTransform.localRotation == Quaternion.Euler(0, 0, 0))
                    {
                        el.Direction = ExitDir.Right;
                    }
                    else if (exitTransform.localRotation == Quaternion.Euler(0, 0, 90))
                    {
                        el.Direction = ExitDir.Up;
                    }
                    else if (exitTransform.localRotation == Quaternion.Euler(0, 0, 180))
                    {
                        el.Direction = ExitDir.Left;
                    }
                    else if (exitTransform.localRotation == Quaternion.Euler(0, 0, -90))
                    {
                        el.Direction = ExitDir.Down;
                    }
                    ExitLocations.Add(el);
                }
            }

            DestroyImmediate(go);
        }

        public List<ExitLocation> GetExitLocationsFor(ExitDir direction)
        {
            List<ExitLocation> exitLocs = new List<ExitLocation>();

            foreach (var el in ExitLocations)
            {
                if (el.Direction == direction)
                {
                    exitLocs.Add(el);
                }
            }

            return exitLocs;
        }

    }
}