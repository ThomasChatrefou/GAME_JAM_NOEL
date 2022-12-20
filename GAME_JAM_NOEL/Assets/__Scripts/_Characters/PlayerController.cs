using System;
using UnityEngine;

public class PlayerController : Character
{
    public PlayerInputs inputs;
    
    private Vector2 speedDirection;
    private MouseDetection mouseDetection;

    [Header("Weapon")] 
    [SerializeField] 
    private Weapon playerWeapon;
    [SerializeField] 
    private Vector2 crossPosition;
    

    private void Awake()
    {
        mouseDetection = FindObjectOfType<MouseDetection>();
        inputs = new PlayerInputs();
        inputs.Player.Shoot.performed += shootContext => Shoot();
        inputs.Player.Movement.performed += moveContext => Move(moveContext.ReadValue<Vector2>());
        inputs.Player.Movement.canceled += moveContext => Move(moveContext.ReadValue<Vector2>());
    }

    private void FixedUpdate()
    {
        transform.position += Time.fixedDeltaTime * speedValue * new Vector3(speedDirection.x, speedDirection.y, 0.0f);
    }

    private void Update()
    {
        Tic();
        //TODO Faire viser le personnage où la souris est placée uniquement si c'est le perso du client 
        crossPosition = mouseDetection.MousePos;
    }

    private void Move(Vector2 direction)
    {
        if (!isDead)
        {
            speedDirection = direction;
        }
        else
        {
            speedDirection = Vector2.zero;
        }
    }

    private void Shoot()
    {
        Attack();
    }

    private void OnEnable()
    {
        inputs.Enable();
    }
    private void OnDisable()
    {
        inputs.Disable();
    }

    public override void Attack()
    {
        base.Attack();
        Vector2 fireDir = crossPosition - new Vector2(transform.position.x,transform.position.y);
        playerWeapon.LaunchBaseProjectile(fireDir.normalized);
    }

    private void SkillAttack()
    {
        Debug.Log("LauchSkillAttack");
    }

    public void EquipWeapon(Weapon weapon)
    {
        Debug.Log("EquipWeapon");
        playerWeapon = weapon;
        //TODO Update UI Sprite
    }
}
