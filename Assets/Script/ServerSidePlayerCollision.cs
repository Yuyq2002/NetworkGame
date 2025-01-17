using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class ServerSidePlayerCollision : NetworkBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsHost) return;

        if(collision.gameObject.tag == "Bullet")
        {
            collision.gameObject.GetComponent<NetworkObject>().Despawn();
            gameObject.GetComponent<NetworkObject>().Despawn();
            GameManager.Instance.PlayerDied(OwnerClientId);
        }
    }
}
