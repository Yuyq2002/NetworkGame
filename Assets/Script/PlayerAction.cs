using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class PlayerAction : NetworkBehaviour
{
    [SerializeField] private GameObject m_bullet_pref;
    [SerializeField] private float m_spawn_distance;
    [SerializeField] private GameObject m_emote_pref;
    [SerializeField] private float m_emote_cooldown;
    private Camera m_camera;
    private NetworkVariable<bool> m_can_emote = new(true);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        m_camera = Camera.main;
    }

    void OnAttack()
    {
        Vector2 world_space_mouse_pos = m_camera.ScreenToWorldPoint(Input.mousePosition);

        Vector2 dir = world_space_mouse_pos - (Vector2)transform.position;

        SpawnBulletServerRpc(dir.normalized);
    }

    void OnJump()
    { 
        if (!m_can_emote.Value) return;

        SpawnEmoteRpc();
    }

    [Rpc(SendTo.Server)]
    void SpawnBulletServerRpc(Vector2 p_direction)
    {
        GameObject bullet = Instantiate(m_bullet_pref, transform.position + (Vector3)p_direction * m_spawn_distance, Quaternion.identity);
        bullet.GetComponent<BulletMovement>().SetDirection(p_direction);
        bullet.GetComponent<NetworkObject>().Spawn();
    }

    [Rpc(SendTo.Server)]
    void SpawnEmoteRpc()
    {
        m_can_emote.Value = false;

        GameObject emote_instance = Instantiate(m_emote_pref);
        emote_instance.GetComponent<EmoteMovement>().SetFollowingPlayer(gameObject.transform);
        emote_instance.GetComponent<NetworkObject>().Spawn();

        StartCoroutine(EmoteCooldown());
    }

    IEnumerator EmoteCooldown()
    {
        yield return new WaitForSeconds(m_emote_cooldown);

        m_can_emote.Value = true;
    }
}
