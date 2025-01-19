using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class EmoteMovement : NetworkBehaviour
{
    [SerializeField] private Vector3 m_offset;
    [SerializeField] float m_despawn_timer;
    private Transform m_following_player;

    public override void OnNetworkSpawn()
    {
        if (!IsHost)
        {
            enabled = false;
            return;
        }

        StartCoroutine(DespawnEmote());
    }

    private void Update()
    {
        transform.position = m_following_player.position + m_offset;
    }

    public void SetFollowingPlayer(Transform p_player_transform)
    {
        m_following_player = p_player_transform;
        transform.position = m_following_player.position + m_offset;
    }

    private IEnumerator DespawnEmote()
    {
        yield return new WaitForSeconds(m_despawn_timer);

        GetComponent<NetworkObject>().Despawn();
    }
}
