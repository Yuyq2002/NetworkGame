using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClientSideMovement : NetworkBehaviour
{
    [SerializeField] private float m_speed;
    [SerializeField] private PlayerInput m_player_input;

    private Vector2 m_movement_vector;
    private Vector2 m_mouse_position;
    private Camera m_camera;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    private void Awake()
    {
        m_camera = Camera.main;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner)
        {
            m_player_input.enabled = false;
            this.enabled = false;
        }
    }

    private void FixedUpdate()
    {
        transform.position += (Vector3)m_movement_vector * m_speed;
        transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.up, m_mouse_position - (Vector2)transform.position));
    }

    void OnMove(InputValue input)
    {
        m_movement_vector = input.Get<Vector2>();
    }

    void OnLook(InputValue input)
    {
        m_mouse_position = m_camera.ScreenToWorldPoint(input.Get<Vector2>());
    }
}
