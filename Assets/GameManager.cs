using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    private Dictionary<ulong, int> m_scoreboard = new();
    private List<ulong> m_survivor = new();

    public UnityAction<int[]> OnScoreChanged;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsHost) return;

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (!IsHost) return;

        base.OnDestroy();

        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }

    void OnClientConnected(ulong p_client_id)
    {
        m_scoreboard.Add(p_client_id, 0);
        m_survivor.Add(p_client_id);
        Debug.Log("New client");
    }

    void OnClientDisconnected(ulong p_client_id)
    {
        m_scoreboard.Remove(p_client_id);
    }

    public void PlayerDied(ulong p_client_id)
    {
        m_survivor.Remove(p_client_id);

        if(m_survivor.Count <= 0)
        {
            // Draw
        }
        else if(m_survivor.Count <= 1)
        {
            m_scoreboard[m_survivor[0]]++;
            OnScoreChangeRpc(m_scoreboard.Values.ToArray());
        }
    }

    [Rpc(SendTo.Everyone)]
    void OnScoreChangeRpc(int[] p_score)
    {
        OnScoreChanged.Invoke(p_score);
    }
}
