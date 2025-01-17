using Unity.Netcode;
using UnityEngine;

public class PlayerShooting : NetworkBehaviour
{
    [SerializeField] private GameObject m_bullet_pref;
    [SerializeField] private float m_spawn_distance;
    private Camera m_camera;

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

    [Rpc(SendTo.Server)]
    void SpawnBulletServerRpc(Vector2 p_direction)
    {
        GameObject bullet = Instantiate(m_bullet_pref, transform.position + (Vector3)p_direction * m_spawn_distance, Quaternion.identity);
        bullet.GetComponent<BulletMovement>().SetDirection(p_direction);
        bullet.GetComponent<NetworkObject>().Spawn();
    }
}
