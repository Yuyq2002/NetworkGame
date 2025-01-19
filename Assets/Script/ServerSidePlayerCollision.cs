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
            DisablePlayerRpc();
            GameManager.Instance.PlayerDied(OwnerClientId);
        }
    }

    [Rpc(SendTo.Everyone)]
    void DisablePlayerRpc()
    {
        gameObject.SetActive(false);
    }

    [Rpc(SendTo.Everyone)]
    public void EnablePlayerRpc()
    {
        gameObject.SetActive(true);
    }
}
