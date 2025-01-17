using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class BulletMovement : NetworkBehaviour
{
    [SerializeField] private ProjectileStats m_projectile_stats;

    private Vector2 m_direction;
    private NetworkObject m_network_object;
    private float m_life_time;

    public void SetDirection(Vector3 p_direction)
    {
        m_direction = p_direction;
    }

    public override void OnNetworkSpawn()
    {
        if(!IsHost)
        {
            enabled = false;
        }
        else
        {
            m_network_object = GetComponent<NetworkObject>();   
        }
    }

    private void Update()
    {
        if(m_projectile_stats.life_time < m_life_time)
        {
            m_network_object.Despawn();
        }
        else
        {
            m_life_time += Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        transform.position += (Vector3)m_direction * m_projectile_stats.speed;
    }

}
