using UnityEngine;
using Cinemachine;

namespace ProceduralRooms
{
    public class Camera : MonoBehaviour
    {
        [SerializeField]
        private InitPlayerEventChannelSO _initPlayerEventChannel;

        private CinemachineVirtualCamera _cam;

        private void Awake()
        {
            _cam = GetComponent<CinemachineVirtualCamera>();
        }

        private void OnEnable()
        {
            _initPlayerEventChannel.OnEventRaised += SetCameraTarget;
        }

        private void OnDisable()
        {
            _initPlayerEventChannel.OnEventRaised -= SetCameraTarget;
        }

        private void SetCameraTarget(Player p)
        {
            _cam.Follow = p.transform;
        }
    }
}