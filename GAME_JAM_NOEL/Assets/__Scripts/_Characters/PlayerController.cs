using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : Character
{
    public PlayerInputs inputs;
    
    [SerializeField]
    private Vector2 speedDirection;
    private MouseDetection mouseDetection;

    [Header("Weapon")] 
    [SerializeField] 
    private GameObject shovelWeaponPrefab;
    [SerializeField] 
    private Weapon playerWeapon;
    [SerializeField] 
    private Vector2 crossPosition;
    [SerializeField] 
    private Weapon nearbyWeapon;
    
    private void Awake()
    {
        mouseDetection = FindObjectOfType<MouseDetection>();
        inputs = new PlayerInputs();

        inputs.Player.Shoot.performed += shootContext => Shoot();
        inputs.Player.Movement.performed += moveContext => Move(moveContext.ReadValue<Vector2>());
        inputs.Player.Movement.canceled += moveContext => Move(moveContext.ReadValue<Vector2>());
        inputs.Player.Interact.performed += interactContext => EquipWeapon(nearbyWeapon);
    }

    private void Start()
    {
        playerWeapon = Instantiate(shovelWeaponPrefab, shovelWeaponPrefab.transform.position, Quaternion.identity,transform).GetComponent<Weapon>();
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
        if (!IsOwner) return;
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
        if(!IsOwner) return;
        AttackServerRpc(crossPosition,transform.position);
    }

    private void OnEnable()
    {
        inputs.Enable();
    }
    private void OnDisable()
    {
        inputs.Disable();
    }
    
    [ServerRpc(RequireOwnership = false)]
    public override void AttackServerRpc(Vector2 crossPosition, Vector2 playerPos)
    {
        Vector2 fireDir = crossPosition - playerPos;
        playerWeapon.LaunchBaseProjectile(fireDir.normalized);
    }

    private void SkillAttack()
    {
        Debug.Log("LauchSkillAttack");
    }

    public void EquipWeapon(Weapon weapon)
    {
        if (!IsOwner || !weapon) return;
        Debug.Log("EquipWeapon " + weapon.name);
        playerWeapon = weapon;
        //TODO Update UI Sprite
    }
}
