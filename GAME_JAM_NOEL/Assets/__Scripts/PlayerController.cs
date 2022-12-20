using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    public float speedValue = 1.0f;
    private Vector2 speedDirection;

    public PlayerInputs inputs;

    private void Awake()
    {
        inputs = new PlayerInputs();
        inputs.Player.Shoot.performed += shootContext => Shoot();
        inputs.Player.Movement.performed += moveContext => Move(moveContext.ReadValue<Vector2>());
        inputs.Player.Movement.canceled += moveContext => Move(moveContext.ReadValue<Vector2>());
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        transform.position += Time.fixedDeltaTime * speedValue * new Vector3(speedDirection.x, speedDirection.y, 0.0f);
    }

    private void Move(Vector2 direction)
    {
        speedDirection = direction;
    }

    private void Shoot()
    {
    }

    private void OnEnable()
    {
        inputs.Enable();
    }
    private void OnDisable()
    {
        inputs.Disable();
    }
}
