using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace XRMultiplayer
{
    public class CharacterResetter : MonoBehaviour
    {
        [SerializeField] Vector2 m_MinMaxHeight = new Vector2(-2.5f, 25.0f);
        [SerializeField] float m_ResetDistance = 5000f;
        [SerializeField] Transform onlineSpawnPoint;
        [SerializeField] Transform offlineSpawnPoint;

        private TeleportationProvider m_TeleportationProvider;
        private Vector3 m_ResetPosition;

        private void Start()
        {
            XRINetworkGameManager.Connected.Subscribe(UpdateResetPosition);
            m_TeleportationProvider = GetComponentInChildren<TeleportationProvider>();

            m_ResetPosition = offlineSpawnPoint.position;
            ResetPlayer();
        }

        void UpdateResetPosition(bool connected)
        {
            if (connected)
                m_ResetPosition = onlineSpawnPoint.position;
            else
            {
                m_ResetPosition = offlineSpawnPoint.position;
                ResetPlayer();
            }
        }

        void Update()
        {
            Vector3 pos = transform.position;
            if (pos.y < m_MinMaxHeight.x || pos.y > m_MinMaxHeight.y ||
                Mathf.Abs(pos.x) > m_ResetDistance || Mathf.Abs(pos.z) > m_ResetDistance)
            {
                ResetPlayer();
            }
        }

        public void ResetPlayer()
        {
            ResetPlayer(m_ResetPosition);
        }

        void ResetPlayer(Vector3 destination)
        {
            TeleportRequest teleportRequest = new TeleportRequest
            {
                destinationPosition = destination,
                destinationRotation = Quaternion.identity
            };

            if (!m_TeleportationProvider.QueueTeleportRequest(teleportRequest))
            {
                Utils.LogWarning("Failed to queue teleport request");
            }
        }

        [ContextMenu("Set Player To Online Position")]
        void SetPlayerToOnlinePosition()
        {
            ResetPlayer(onlineSpawnPoint.position);
        }

        [ContextMenu("Set Player To Offline Position")]
        void SetPlayerToOfflinePosition()
        {
            ResetPlayer(offlineSpawnPoint.position);
        }
    }
}
