using Cinemachine;
using System.Collections;
using UnityEngine;

namespace ProceduralRooms
{
    public class CamHandler : MonoBehaviour
    {
        private CinemachineBrain _camBrain;

        [SerializeField]
        private BoolEventChannelSO _toggleInputEvent;

        private void Awake()
        {
            _camBrain = GetComponent<CinemachineBrain>();
            _camBrain.m_CameraActivatedEvent.AddListener(ToggleInput);
        }

        private void ToggleInput(ICinemachineCamera arg0, ICinemachineCamera arg1)
        {
            StartCoroutine(PauseInputWhileBlendingCoroutine());
        }

        private IEnumerator PauseInputWhileBlendingCoroutine()
        {
            _toggleInputEvent.RaiseEvent(true);

            yield return new WaitForSeconds(_camBrain.m_DefaultBlend.BlendTime);

            _toggleInputEvent.RaiseEvent(false);

        }

        private void OnDestroy()
        {
            _camBrain.m_CameraActivatedEvent.RemoveListener(ToggleInput);
        }
    }
}