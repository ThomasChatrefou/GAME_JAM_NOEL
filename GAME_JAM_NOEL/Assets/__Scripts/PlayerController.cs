using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;

    public float speedValue = 1.0f;
    private Vector2 speedDirection;

    public PlayerInputs inputs;

    public UnityEvent OnPlayerDeath;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputs = new PlayerInputs();
        inputs.Player.Shoot.performed += shootContext => Shoot();
        inputs.Player.Movement.performed += moveContext => Move(moveContext.ReadValue<Vector2>());
        inputs.Player.Movement.canceled += moveContext => Move(moveContext.ReadValue<Vector2>());
    }

    private void FixedUpdate()
    {
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
