using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private float m_intermission_time;
    [SerializeField] private List<Transform> m_spawn_points;

    private int m_available_spawn_point = 0;

    private Dictionary<ulong, int> m_scoreboard = new();
    private List<ulong> m_survivor = new();
    private List<ServerSidePlayerCollision> m_player_ref = new();

    public static GameManager Instance;
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
        m_player_ref.Add(NetworkManager.Singleton.ConnectedClients[p_client_id].PlayerObject.gameObject.GetComponent<ServerSidePlayerCollision>());
        NetworkManager.Singleton.ConnectedClients[p_client_id].PlayerObject.transform.position = m_spawn_points[m_available_spawn_point].position;
        m_available_spawn_point++;
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
            StartCoroutine(Intermission());
        }
    }

    [Rpc(SendTo.Everyone)]
    void OnScoreChangeRpc(int[] p_score)
    {
        OnScoreChanged.Invoke(p_score);
    }

    private IEnumerator Intermission()
    {
        yield return new WaitForSeconds(m_intermission_time);

        Debug.Log("Restart");

        foreach (ServerSidePlayerCollision player in m_player_ref) player.EnablePlayerRpc();
        m_survivor.Clear();
        foreach(var client_id in m_scoreboard.Keys)
        {
            m_survivor.Add(client_id);
        }
    }
}
